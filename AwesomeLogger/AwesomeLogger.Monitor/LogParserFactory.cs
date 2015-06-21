using AwesomeLogger.Monitor.Events;

namespace AwesomeLogger.Monitor
{
    internal class LogParserFactory : ILogParserFactory
    {
        private readonly IMatchEventEmitter _matchEventEmitter;

        public LogParserFactory(IMatchEventEmitter matchEventEmitter)
        {
            _matchEventEmitter = matchEventEmitter;
        }

        public ILogParser Create(string machineName, string searchPath, string filePath, string pattern, string email)
        {
            return new LogParser(machineName, searchPath, filePath, pattern, email, _matchEventEmitter);
        }
    }
}