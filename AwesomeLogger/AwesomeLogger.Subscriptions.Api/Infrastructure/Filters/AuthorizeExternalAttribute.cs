using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using AwesomeLogger.Subscriptions.Api.Infrastructure.Configuration;

namespace AwesomeLogger.Subscriptions.Api.Infrastructure.Filters
{
    public class AuthorizeExternalAttribute : AuthorizeBearerAttribute
    {
        private readonly IConfigurationProvider _config = new ConfigurationProvider();

        public AuthorizeExternalAttribute() : base(null)
        {
        }

        public override Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            Token = _config.Get(SettingNames.ExternalAccessToken);
            return base.AuthenticateAsync(context, cancellationToken);
        }
    }
}