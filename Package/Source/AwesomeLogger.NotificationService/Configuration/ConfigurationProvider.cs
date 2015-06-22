using System.Configuration;

namespace AwesomeLogger.NotificationService.Configuration
{
    public class ConfigurationProvider : IConfigurationProvider
    {
        public string Get(string name)
        {
            return ConfigurationManager.AppSettings[name];
        }
    }
}