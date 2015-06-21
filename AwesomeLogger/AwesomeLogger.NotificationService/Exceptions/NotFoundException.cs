namespace AwesomeLogger.NotificationService.Exceptions
{
    public class ConflictException : NotificationServiceException
    {
        public ConflictException(string message)
            : base(message)
        {
        }
    }
}