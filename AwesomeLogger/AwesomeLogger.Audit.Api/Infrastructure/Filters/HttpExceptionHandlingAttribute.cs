using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using AwesomeLogger.Audit.Api.Exceptions;

namespace AwesomeLogger.Audit.Api.Infrastructure.Filters
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
            else if (exception is ConflictException)
            {
                context.Response = new HttpResponseMessage(HttpStatusCode.Conflict);
            }
            else
            {
                // 500
                Trace.TraceError("Internal Server Error: {0}", exception);

                context.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("Internal Server Error"),
                    ReasonPhrase = "Error"
                };
            }
        }
    }
}