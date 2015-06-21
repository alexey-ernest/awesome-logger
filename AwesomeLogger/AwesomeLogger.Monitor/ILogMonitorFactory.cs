namespace AwesomeLogger.Monitor
{
    internal interface ILogMonitorFactory
    {
        ILogMonitor Create(string machineName, string logPath, string pattern, string email);
    }
}