using System.Diagnostics;

namespace AwesomeLogger.Monitor.Initializers
{
    internal class DiagnosticsInitializer : IDiagnosticsInitializer
    {
        private const string EventSourceName = "AwesomeLogger";
        private const string LogName = "Monitor";

        public void Initialize()
        {
            // Event log setup
            if (!EventLog.SourceExists(EventSourceName))
            {
                EventLog.CreateEventSource(EventSourceName, LogName);
            }

            Trace.Listeners.Add(new EventLogTraceListener(EventSourceName));
            Trace.Listeners.Add(new ConsoleTraceListener());
        }
    }
}