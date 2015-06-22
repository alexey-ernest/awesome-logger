using System;
using System.Configuration;

namespace AwesomeLogger.Monitor.Configuration
{
    public class ConfigurationProvider : IConfigurationProvider
    {
        public string Get(string name)
        {
            return ConfigurationManager.AppSettings[name];
        }

        public string GetMachineName()
        {
            return Environment.MachineName;
        }
    }
}