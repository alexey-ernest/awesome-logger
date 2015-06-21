using System;
using System.Diagnostics;
using AwesomeLogger.ErrorHandlingService.Configuration;
using AwesomeLogger.NotificationService.Configuration;
using Microsoft.ServiceBus.Messaging;

namespace AwesomeLogger.ErrorHandlingService
{
    internal class ErrorHandlingManager : IErrorHandlingManager
    {
        private readonly IConfigurationProvider _config;
        private SubscriptionClient _client;

        public ErrorHandlingManager(IConfigurationProvider config)
        {
            _config = config;
        }

        public void Start()
        {
            // Process messages
            _client =
                SubscriptionClient.CreateFromConnectionString(_config.Get(SettingNames.ServiceBusConnectionString),
                    _config.Get(SettingNames.ServiceBusErrorTopic),
                    _config.Get(SettingNames.ServiceBusErrorChannel));

            _client.OnMessage(message =>
            {
                try
                {
                    object machineName;
                    message.Properties.TryGetValue("MachineName", out machineName);

                    object error;
                    message.Properties.TryGetValue("Error", out error);

                    // Logging
                    var body = string.Format("AwesomeLogger Error.\n\n" +
                                             "Machine: {0}\n" +
                                             "Error: {1}\n", machineName, error);
                    Trace.TraceInformation(body);

                    message.Complete();
                }
                catch (Exception e)
                {
                    // Could not process message
                    message.Abandon();

                    var msg = string.Format("Failed to process message: {0}", e);
                    Trace.TraceError(msg);
                }
            });
        }

        public void Dispose()
        {
            _client.Close();
        }
    }
}