using System;

namespace AwesomeLogger.Subscriptions.Api.Exceptions
{
    public class SubscriptionApiException : Exception
    {
        public SubscriptionApiException(string message)
            : base(message)
        {
        }
    }
}