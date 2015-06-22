using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AwesomeLogger.Web.Exceptions;
using AwesomeLogger.Web.Infrastructure.Configuration;
using AwesomeLogger.Web.Models;
using Newtonsoft.Json;

namespace AwesomeLogger.Web.Services
{
    internal class SubscriptionService : ISubscriptionService
    {
        private readonly IConfigurationProvider _config = new ConfigurationProvider();

        public async Task<List<SubscriptionModel>> GetAllAsync()
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(_config.Get(SettingNames.SubscriptionsUri)),
                Method = HttpMethod.Get
            };

            var response = await MakeRequestAsync(request);
            response.EnsureSuccessStatusCode();

            var subParams = await response.Content.ReadAsAsync<IEnumerable<SubscriptionModel>>();
            return subParams.ToList();
        }

        public async Task<SubscriptionModel> GetAsync(int id)
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(string.Format("{0}{1}", _config.Get(SettingNames.SubscriptionsUri), id)),
                Method = HttpMethod.Get
            };

            var response = await MakeRequestAsync(request);
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new NotFoundException("Subscription does not exist.");
            }
            response.EnsureSuccessStatusCode();

            var sub = await response.Content.ReadAsAsync<SubscriptionModel>();
            return sub;
        }

        public async Task<SubscriptionModel> AddAsync(SubscriptionModel sub)
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(_config.Get(SettingNames.SubscriptionsUri)),
                Method = HttpMethod.Post,
                Content = new StringContent(JsonConvert.SerializeObject(sub), Encoding.UTF8, "application/json")
            };

            var response = await MakeRequestAsync(request);
            response.EnsureSuccessStatusCode();

            sub = await response.Content.ReadAsAsync<SubscriptionModel>();
            return sub;
        }

        public async Task<SubscriptionModel> UpdateAsync(SubscriptionModel sub)
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(string.Format("{0}{1}", _config.Get(SettingNames.SubscriptionsUri), sub.Id)),
                Method = HttpMethod.Put,
                Content = new StringContent(JsonConvert.SerializeObject(sub), Encoding.UTF8, "application/json")
            };

            var response = await MakeRequestAsync(request);
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new NotFoundException("Subscription does not exist.");
            }
            response.EnsureSuccessStatusCode();

            sub = await response.Content.ReadAsAsync<SubscriptionModel>();
            return sub;
        }

        public async Task DeleteAsync(int id)
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(string.Format("{0}{1}", _config.Get(SettingNames.SubscriptionsUri), id)),
                Method = HttpMethod.Delete
            };

            var response = await MakeRequestAsync(request);
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new NotFoundException("Subscription does not exist.");
            }
            response.EnsureSuccessStatusCode();
        }

        #region helpers

        private async Task<HttpResponseMessage> MakeRequestAsync(HttpRequestMessage request)
        {
            using (var client = new HttpClient())
            {
                // set auth header
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
                    _config.Get(SettingNames.SubscriptionsAccessToken));

                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // make request
                var response = await client.SendAsync(request);
                return response;
            }
        }

        #endregion
    }
}