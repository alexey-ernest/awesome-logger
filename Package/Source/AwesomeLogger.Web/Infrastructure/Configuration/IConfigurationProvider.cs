namespace AwesomeLogger.Web.Infrastructure.Configuration
{
    public interface IConfigurationProvider
    {
        string Get(string name);
    }
}