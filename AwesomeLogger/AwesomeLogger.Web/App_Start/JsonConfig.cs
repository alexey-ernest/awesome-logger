using System.Web.Http;
using Newtonsoft.Json.Serialization;

namespace AwesomeLogger.Web
{
    public static class JsonConfig
    {
        public static void Configure(HttpConfiguration configuration)
        {
            var json = configuration.Formatters.JsonFormatter;
            json.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }
    }
}