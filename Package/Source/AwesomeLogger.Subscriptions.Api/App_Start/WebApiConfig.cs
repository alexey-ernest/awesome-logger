using System.Linq;
using System.Web.Http;
using AwesomeLogger.Subscriptions.Api.Infrastructure.Filters;

namespace AwesomeLogger.Subscriptions.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API routes
            config.MapHttpAttributeRoutes();

            // Remove XML serializer
            var appXmlType =
                config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "application/xml");
            config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(appXmlType);

            // General exception handling
            config.Filters.Add(
                (HttpExceptionHandlingAttribute)
                    config.DependencyResolver.GetService(typeof (HttpExceptionHandlingAttribute)));
        }
    }
}