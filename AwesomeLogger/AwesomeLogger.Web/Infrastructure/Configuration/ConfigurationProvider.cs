using System.Configuration;

namespace AwesomeLogger.Web.Infrastructure.Configuration
{
    public class ConfigurationProvider : IConfigurationProvider
    {
        public string Get(string name)
        {
            return ConfigurationManager.AppSettings[name];
        }
    }
}