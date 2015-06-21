namespace AwesomeLogger.Audit.Api.Infrastructure.Configuration
{
    public interface IConfigurationProvider
    {
        string Get(string name);
    }
}