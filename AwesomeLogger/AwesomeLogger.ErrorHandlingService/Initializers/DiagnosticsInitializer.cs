using System.Diagnostics;

namespace AwesomeLogger.ErrorHandlingService.Initializers
{
    internal class DiagnosticsInitializer : IDiagnosticsInitializer
    {
        private const string EventSourceName = "Error-Handling Service";
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