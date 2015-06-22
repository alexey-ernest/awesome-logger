namespace AwesomeLogger.Subscriptions.Api.Infrastructure.Configuration
{
    public interface IConfigurationProvider
    {
        string Get(string name);
    }
}