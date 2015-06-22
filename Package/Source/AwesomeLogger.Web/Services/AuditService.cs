using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AwesomeLogger.Web.Infrastructure.Configuration;
using AwesomeLogger.Web.Models;

namespace AwesomeLogger.Web.Services
{
    internal class AuditService : IAuditService
    {
        private readonly IConfigurationProvider _config = new ConfigurationProvider();

        public async Task<IEnumerable<PatternMatchModel>> SearchAsync(string machineName, string searchPath,
            string pattern, string email, int skip, int top)
        {
            var url = string.Format("{0}?m={1}&s={2}&p={3}&e={4}&$skip={5}&$top={6}&$orderby=Created+desc", _config.Get(SettingNames.AuditUri), machineName,
                searchPath, pattern, email, skip, top);
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Get
            };

            var response = await MakeRequestAsync(request);
            response.EnsureSuccessStatusCode();

            var matches = await response.Content.ReadAsAsync<IEnumerable<PatternMatchModel>>();
            return matches.ToList();
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