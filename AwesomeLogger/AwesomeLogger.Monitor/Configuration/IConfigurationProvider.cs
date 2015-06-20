namespace AwesomeLogger.Monitor.Configuration
{
    public interface IConfigurationProvider
    {
        string Get(string name);
    }
}