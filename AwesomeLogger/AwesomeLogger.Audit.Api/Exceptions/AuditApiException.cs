using System;

namespace AwesomeLogger.Audit.Api.Exceptions
{
    public class AuditApiException : Exception
    {
        public AuditApiException(string message)
            : base(message)
        {
        }
    }
}