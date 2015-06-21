using System.Configuration;

namespace AwesomeLogger.Audit.Api.Infrastructure.Configuration
{
    public class ConfigurationProvider : IConfigurationProvider
    {
        public string Get(string name)
        {
            return ConfigurationManager.AppSettings[name];
        }
    }
}