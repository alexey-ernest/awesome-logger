using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using AwesomeLogger.Subscriptions.Api.Exceptions;

namespace AwesomeLogger.Subscriptions.Api.Infrastructure.Filters
{
    /// <summary>
    ///     Exception handling for Api controllers.
    /// </summary>
    public class HttpExceptionHandlingAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            var exception = context.Exception;
            if (exception is NotFoundException)
            {
                context.Response = new HttpResponseMessage(HttpStatusCode.NotFound);
            }
            else
            {
                // 500
                context.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("Internal Server Error"),
                    ReasonPhrase = "Error"
                };
            }
        }
    }
}