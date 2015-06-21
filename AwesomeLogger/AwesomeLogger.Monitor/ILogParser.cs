using System;
using System.Threading.Tasks;

namespace AwesomeLogger.Monitor
{
    internal interface ILogParser: IDisposable
    {
        Task ParseAsync();
    }
}