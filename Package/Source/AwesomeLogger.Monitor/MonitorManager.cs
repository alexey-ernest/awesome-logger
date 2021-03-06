﻿using System;
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
        private readonly ILogMonitorFactory _monitorFactory;
        private readonly List<ILogMonitor> _monitors = new List<ILogMonitor>();
        private readonly ISubscriptionServiceClient _service;
        private SubscriptionClient _client;

        public MonitorManager(ISubscriptionServiceClient service, IConfigurationProvider config,
            IErrorEventEmitter errorEventEmitter, ILogMonitorFactory monitorFactory)
        {
            _service = service;
            _config = config;
            _errorEventEmitter = errorEventEmitter;
            _monitorFactory = monitorFactory;
        }

        public void Start()
        {
            try
            {
                // Start monitoring
                StartMonitoring();

                // Process messages
                _client =
                    SubscriptionClient.CreateFromConnectionString(_config.Get(SettingNames.ServiceBusConnectionString),
                        _config.Get(SettingNames.ServiceBusSubscriptionTopic),
                        _config.GetMachineName());

                _client.OnMessage(message =>
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
                            // skipping messages not for us (only in our channel)
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
                _errorEventEmitter.EmitAsync(new Dictionary<string, string>
                {
                    {"MachineName", _config.GetMachineName()},
                    {"Error", msg}
                }).Wait();

                throw;
            }
        }

        public void Dispose()
        {
            if (_client != null)
            {
                _client.Close();    
            }
            
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
                _monitors.Add(_monitorFactory.Create(_config.GetMachineName(), param.LogPath, param.Pattern, param.Email));
            }

            // Start monitors in parallel
            Task.Run(() =>
            {
                try
                {
                    foreach (var monitor in _monitors)
                    {
                        monitor.Start();
                    }
                }
                catch (Exception e)
                {
                    var msg = string.Format("Failed to monitor logs: {0}", e);
                    Trace.TraceError(msg);

                    _errorEventEmitter.EmitAsync(new Dictionary<string, string>
                    {
                        {"MachineName", _config.GetMachineName()},
                        {"Error", msg}
                    }).Wait();
                }
            });
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