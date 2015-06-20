using System;
using System.Diagnostics;
using AwesomeLogger.Monitor.Configuration;
using Microsoft.ServiceBus;

namespace AwesomeLogger.Monitor.Initializers
{
    internal class ServiceBusInitializer : IServiceBusInitializer
    {
        private readonly IConfigurationProvider _config;
        private const string AllMessages = "AllMessages";

        public ServiceBusInitializer(IConfigurationProvider config)
        {
            _config = config;
        }

        public void Initialize()
        {
            try
            {
                var namespaceManager = NamespaceManager.CreateFromConnectionString(_config.Get(SettingNames.ServiceBusConnectionString));

                var subscriptionTopic = _config.Get(SettingNames.ServiceBusSubscriptionTopic);
                if (!namespaceManager.TopicExists(subscriptionTopic))
                {
                    namespaceManager.CreateTopic(subscriptionTopic);
                }
                var subscriptionChannel = _config.GetMachineName();
                if (!namespaceManager.SubscriptionExists(subscriptionTopic, subscriptionChannel))
                {
                    namespaceManager.CreateSubscription(subscriptionTopic, subscriptionChannel);
                }

                var errorTopic = _config.Get(SettingNames.ServiceBusErrorTopic);
                if (!namespaceManager.TopicExists(errorTopic))
                {
                    namespaceManager.CreateTopic(errorTopic);
                }
                if (!namespaceManager.SubscriptionExists(errorTopic, AllMessages))
                {
                    namespaceManager.CreateSubscription(errorTopic, AllMessages);
                }

                var notifyTopic = _config.Get(SettingNames.ServiceBusNotifyTopic);
                if (!namespaceManager.TopicExists(notifyTopic))
                {
                    namespaceManager.CreateTopic(notifyTopic);
                }
                if (!namespaceManager.SubscriptionExists(notifyTopic, AllMessages))
                {
                    namespaceManager.CreateSubscription(notifyTopic, AllMessages);
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Could not initialize service bus: {0}", e);
            }
        }
    }
}