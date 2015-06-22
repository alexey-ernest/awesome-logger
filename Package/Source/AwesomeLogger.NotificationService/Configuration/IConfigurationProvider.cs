namespace AwesomeLogger.NotificationService.Configuration
{
    public interface IConfigurationProvider
    {
        string Get(string name);
    }
}