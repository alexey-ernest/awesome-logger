namespace AwesomeLogger.Monitor
{
    internal interface ILogParserFactory
    {
        ILogParser Create(string machineName, string searchPath, string filePath, string pattern, string email);
    }
}