using System;
using System.Diagnostics;
using AwesomeLogger.ErrorHandlingService.Configuration;
using AwesomeLogger.NotificationService.Configuration;
using Microsoft.ServiceBus;

namespace AwesomeLogger.ErrorHandlingService.Initializers
{
    internal class ServiceBusInitializer : IServiceBusInitializer
    {
        private readonly IConfigurationProvider _config;

        public ServiceBusInitializer(IConfigurationProvider config)
        {
            _config = config;
        }

        public void Initialize()
        {
            try
            {
                var namespaceManager = NamespaceManager.CreateFromConnectionString(_config.Get(SettingNames.ServiceBusConnectionString));

                var errorTopic = _config.Get(SettingNames.ServiceBusErrorTopic);
                if (!namespaceManager.TopicExists(errorTopic))
                {
                    namespaceManager.CreateTopic(errorTopic);
                }

                var errorChannel = _config.Get(SettingNames.ServiceBusErrorChannel);
                if (!namespaceManager.SubscriptionExists(errorTopic, errorChannel))
                {
                    namespaceManager.CreateSubscription(errorTopic, errorChannel);
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Could not initialize service bus: {0}", e);
            }
        }
    }
}