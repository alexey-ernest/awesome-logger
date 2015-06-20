namespace AwesomeLogger.Monitor.Events
{
    internal class MatchEventEmitter : ServiceBusEventEmitter, IMatchEventEmitter
    {
        public MatchEventEmitter(string connectionString, string topic)
            : base(connectionString, topic)
        {
        }
    }
}