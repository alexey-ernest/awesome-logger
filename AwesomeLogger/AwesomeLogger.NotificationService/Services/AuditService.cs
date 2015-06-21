using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AwesomeLogger.NotificationService.Configuration;
using AwesomeLogger.NotificationService.Exceptions;
using AwesomeLogger.NotificationService.Models;
using Newtonsoft.Json;

namespace AwesomeLogger.NotificationService.Services
{
    internal class AuditService : IAuditService
    {
        private readonly IConfigurationProvider _config = new ConfigurationProvider();

        public async Task AddAsync(PatternMatchModel match)
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(_config.Get(SettingNames.AuditUri)),
                Method = HttpMethod.Post,
                Content = new StringContent(JsonConvert.SerializeObject(match), Encoding.UTF8, "application/json")
            };

            var response = await MakeRequestAsync(request);
            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                throw new ConflictException("Match already processed.");
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
                    _config.Get(SettingNames.AuditAccessToken));

                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // make request
                var response = await client.SendAsync(request);
                return response;
            }
        }

        #endregion
    }
}