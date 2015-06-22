using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AwesomeLogger.Monitor.Configuration;

namespace AwesomeLogger.Monitor.Subscriptions
{
    internal class SubscriptionServiceClient : ISubscriptionServiceClient
    {
        private readonly IConfigurationProvider _config = new ConfigurationProvider();

        public async Task<List<SubscriptionParams>> GetParamsAsync(string machineName)
        {
            using (var client = new HttpClient())
            {
                // set auth header
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
                    _config.Get(SettingNames.AccessToken));

                // building request
                var request = new HttpRequestMessage
                {
                    RequestUri =
                        new Uri(string.Format("{0}{1}", _config.Get(SettingNames.SubscriptionsUri), machineName)),
                    Method = HttpMethod.Get
                };
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // make request
                using (var response = await client.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();

                    // read response
                    var subParams = await response.Content.ReadAsAsync<IEnumerable<SubscriptionParams>>();
                    return subParams.ToList();
                }
            }
        }
    }
}