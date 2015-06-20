using System.Threading.Tasks;

namespace AwesomeLogger.Monitor
{
    internal interface ILogParser
    {
        Task ParseAsync();
    }
}