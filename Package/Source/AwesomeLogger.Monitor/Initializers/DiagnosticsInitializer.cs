using System.Diagnostics;

namespace AwesomeLogger.Monitor.Initializers
{
    internal class DiagnosticsInitializer : IDiagnosticsInitializer
    {
        private const string EventSourceName = "Monitor";
        private const string LogName = "AwesomeLogger";

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