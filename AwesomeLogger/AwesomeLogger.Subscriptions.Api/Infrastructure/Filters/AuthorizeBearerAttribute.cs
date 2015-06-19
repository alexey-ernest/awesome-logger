using System;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;

namespace AwesomeLogger.Subscriptions.Api.Infrastructure.Filters
{
    public class AuthorizeBearerAttribute : Attribute, IAuthenticationFilter
    {
        public AuthorizeBearerAttribute(string token)
        {
            Token = token;
        }

        public string Token { get; set; }

        public bool AllowMultiple
        {
            get { return true; }
        }

        public virtual Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            var authorization = context.Request.Headers.Authorization;
            if (authorization == null)
            {
                return Task.FromResult(0);
            }

            if (string.Equals(authorization.Scheme, "Bearer", StringComparison.OrdinalIgnoreCase) &&
                authorization.Parameter == Token)
            {
                context.Principal = new GenericPrincipal(new GenericIdentity(Token), null);
            }

            return Task.FromResult(0);
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }
    }
}