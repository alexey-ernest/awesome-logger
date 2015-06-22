using System;

namespace AwesomeLogger.NotificationService.Exceptions
{
    public class NotificationServiceException : Exception
    {
        public NotificationServiceException(string message)
            : base(message)
        {
        }
    }
}