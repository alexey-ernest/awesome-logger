using AwesomeLogger.Monitor.Events;

namespace AwesomeLogger.Monitor
{
    internal class LogMonitorFactory : ILogMonitorFactory
    {
        private readonly IErrorEventEmitter _errorEventEmitter;
        private readonly ILogParserFactory _logParserFactory;

        public LogMonitorFactory(IErrorEventEmitter errorEventEmitter, ILogParserFactory logParserFactory)
        {
            _errorEventEmitter = errorEventEmitter;
            _logParserFactory = logParserFactory;
        }

        public ILogMonitor Create(string machineName, string logPath, string pattern, string email)
        {
            return new LogMonitor(machineName, logPath, pattern, email,
                _errorEventEmitter, _logParserFactory);
        }
    }
}