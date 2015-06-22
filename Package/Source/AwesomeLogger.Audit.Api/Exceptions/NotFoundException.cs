namespace AwesomeLogger.Audit.Api.Exceptions
{
    public class NotFoundException : AuditApiException
    {
        public NotFoundException(string message)
            : base(message)
        {
        }
    }
}