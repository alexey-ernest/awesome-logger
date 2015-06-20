using System;
using System.Diagnostics;
using AwesomeLogger.NotificationService.Configuration;
using Microsoft.ServiceBus;

namespace AwesomeLogger.NotificationService.Initializers
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

                var notifyTopic = _config.Get(SettingNames.ServiceBusNotifyTopic);
                if (!namespaceManager.TopicExists(notifyTopic))
                {
                    namespaceManager.CreateTopic(notifyTopic);
                }

                var notifyChannel = _config.Get(SettingNames.ServiceBusNotifyChannel);
                if (!namespaceManager.SubscriptionExists(notifyTopic, notifyChannel))
                {
                    namespaceManager.CreateSubscription(notifyTopic, notifyChannel);
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Could not initialize service bus: {0}", e);
            }
        }
    }
}