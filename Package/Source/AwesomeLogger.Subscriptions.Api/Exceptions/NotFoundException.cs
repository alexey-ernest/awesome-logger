namespace AwesomeLogger.Subscriptions.Api.Exceptions
{
    public class NotFoundException : SubscriptionApiException
    {
        public NotFoundException(string message)
            : base(message)
        {
        }
    }
}