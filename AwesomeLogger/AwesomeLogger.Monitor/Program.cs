using System;
using AwesomeLogger.Monitor.Subscriptions;

namespace AwesomeLogger.Monitor
{
    internal class Program
    {
        private static void Main()
        {
            var client = new SubscriptionServiceClient();
            var parameters = client.GetParamsAsync("Alexey-PC").Result;
            Console.ReadKey();
        }
    }
}