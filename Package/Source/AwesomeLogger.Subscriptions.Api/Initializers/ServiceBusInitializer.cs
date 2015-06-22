using AwesomeLogger.Subscriptions.Api.Infrastructure.Configuration;
using Microsoft.ServiceBus;

namespace AwesomeLogger.Subscriptions.Api.Initializers
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
            var namespaceManager =
                NamespaceManager.CreateFromConnectionString(_config.Get(SettingNames.ServiceBusConnectionString));

            var subscriptionTopic = _config.Get(SettingNames.ServiceBusSubscriptionTopic);
            if (!namespaceManager.TopicExists(subscriptionTopic))
            {
                namespaceManager.CreateTopic(subscriptionTopic);
            }
        }
    }
}