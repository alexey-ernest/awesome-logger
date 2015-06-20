namespace AwesomeLogger.Monitor.Events
{
    internal class MatchEventEmitter : ServiceBusEventEmitter, IErrorEventEmitter
    {
        public MatchEventEmitter(string connectionString, string topic)
            : base(connectionString, topic)
        {
        }
    }
}