using System.Configuration;
using AwesomeLogger.NotificationService.Configuration;

namespace AwesomeLogger.ErrorHandlingService.Configuration
{
    public class ConfigurationProvider : IConfigurationProvider
    {
        public string Get(string name)
        {
            return ConfigurationManager.AppSettings[name];
        }
    }
}