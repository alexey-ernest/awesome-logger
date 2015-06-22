namespace AwesomeLogger.Audit.Api.Exceptions
{
    public class ConflictException : AuditApiException
    {
        public ConflictException(string message)
            : base(message)
        {
        }
    }
}