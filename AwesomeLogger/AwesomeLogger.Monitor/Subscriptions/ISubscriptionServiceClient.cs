using System.Collections.Generic;
using System.Threading.Tasks;

namespace AwesomeLogger.Monitor.Subscriptions
{
    internal interface ISubscriptionServiceClient
    {
        Task<List<SubscriptionParams>> GetParamsAsync(string machineName);
    }
}