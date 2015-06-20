using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using AwesomeLogger.Monitor.Configuration;
using AwesomeLogger.Monitor.Events;
using AwesomeLogger.Monitor.Subscriptions;
using Microsoft.ServiceBus.Messaging;

namespace AwesomeLogger.Monitor
{
    internal class MonitorManager : IMonitorManager
    {
        private readonly IConfigurationProvider _config;
        private readonly IErrorEventEmitter _errorEventEmitter;
        private readonly IMatchEventEmitter _matchEventEmitter;
        private readonly List<ILogMonitor> _monitors = new List<ILogMonitor>();
        private readonly ISubscriptionServiceClient _service;

        public MonitorManager(ISubscriptionServiceClient service, IConfigurationProvider config,
            IErrorEventEmitter errorEventEmitter, IMatchEventEmitter matchEventEmitter)
        {
            _service = service;
            _config = config;
            _errorEventEmitter = errorEventEmitter;
            _matchEventEmitter = matchEventEmitter;
        }

        public void Start()
        {
            try
            {
                // Start monitoring
                StartMonitoring();

                // Listen for parameter updates
                var serviceBusClient =
                    SubscriptionClient.CreateFromConnectionString(_config.Get(SettingNames.ServiceBusConnectionString),
                        _config.Get(SettingNames.ServiceBusSubscriptionTopic),
                        _config.GetMachineName());

                serviceBusClient.OnMessage(message =>
                {
                    try
                    {
                        if (!message.Properties.ContainsKey("MachineName"))
                        {
                            // invalid message
                            message.Complete();
                        }

                        var machineName = message.Properties["MachineName"].ToString();
                        if (!string.Equals(machineName, _config.GetMachineName(), StringComparison.OrdinalIgnoreCase))
                        {
                            // skipping messages not for us
                            message.Complete();
                        }

                        // check message type
                        if (!message.Properties.ContainsKey("Type"))
                        {
                            // do not know how to process message
                            message.Abandon();
                            return;
                        }

                        var type = message.Properties["Type"].ToString();
                        if (!string.Equals(type, EventTypes.Update))
                        {
                            // do not know how to process message
                            message.Abandon();
                            return;
                        }


                        // Configuration updates, we should restart monitoring
                        StopMonitoring();
                        StartMonitoring();

                        message.Complete();
                    }
                    catch (Exception e)
                    {
                        // Could not process message
                        message.Abandon();

                        var msg = string.Format("Failed to process message: {0}", e);
                        Trace.TraceError(msg);

                        _errorEventEmitter.EmitAsync(new Dictionary<string, string>
                        {
                            {"MachineName", _config.GetMachineName()},
                            {"Error", msg}
                        }).Wait();
                    }

                }, new OnMessageOptions
                {
                    AutoComplete = false
                });
            }
            catch (Exception e)
            {
                var msg = string.Format("Failed to start monitoring: {0}", e);
                Trace.TraceError(msg);

                _errorEventEmitter.EmitAsync(new Dictionary<string, string>
                {
                    {"MachineName", _config.GetMachineName()},
                    {"Error", msg}
                }).Wait();
            }
        }

        public void Dispose()
        {
            StopMonitoring();
        }

        private async Task<List<SubscriptionParams>> GetParamsAsync()
        {
            var subParams = await _service.GetParamsAsync(_config.GetMachineName());
            return subParams;
        }

        private void StartMonitoring()
        {
            // Get subscription params
            var subParams = GetParamsAsync().Result;

            // Create monitors for each param
            _monitors.Clear();
            foreach (var param in subParams)
            {
                _monitors.Add(new LogMonitor(_config.GetMachineName(), param.LogPath, param.Pattern,
                    _errorEventEmitter, _matchEventEmitter));
            }

            // Start monitors
            foreach (var monitor in _monitors)
            {
                try
                {
                    monitor.Start();
                }
                catch (Exception e)
                {
                    var msg = string.Format("Failed to monitor log: {0}", e);
                    Trace.TraceError(msg);

                    _errorEventEmitter.EmitAsync(new Dictionary<string, string>
                    {
                        {"MachineName", _config.GetMachineName()},
                        {"Error", msg}
                    }).Wait();
                }
            }
        }

        private void StopMonitoring()
        {
            foreach (var monitor in _monitors)
            {
                monitor.Dispose();
            }

            _monitors.Clear();
        }
    }
}