namespace AwesomeLogger.Monitor.Events
{
    internal class ErrorEventEmitter : ServiceBusEventEmitter, IErrorEventEmitter
    {
        public ErrorEventEmitter(string connectionString, string topic) : base(connectionString, topic)
        {
        }
    }
}