using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace AwesomeLogger.Subscriptions.Api.Events
{
    internal class ServiceBusEventEmitter : IEventEmitter
    {
        private readonly string _connectionString;
        private readonly string _topic;

        public ServiceBusEventEmitter(string connectionString, string topic)
        {
            _connectionString = connectionString;
            _topic = topic;
        }

        public Task EmitAsync(Dictionary<string, string> properties)
        {
            return EmitAsync(null, properties);
        }

        public async Task EmitAsync(string message, Dictionary<string, string> properties)
        {
            var client = TopicClient.CreateFromConnectionString(_connectionString, _topic);
            var msg = new BrokeredMessage(message);
            if (properties != null)
            {
                foreach (var property in properties)
                {
                    msg.Properties[property.Key] = property.Value;
                }
            }

            await client.SendAsync(msg);
        }
    }
}