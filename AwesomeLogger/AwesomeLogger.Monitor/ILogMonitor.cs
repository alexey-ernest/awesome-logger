using System;
using System.Threading.Tasks;

namespace AwesomeLogger.Monitor
{
    internal interface ILogMonitor: IDisposable
    {
        Task StartAsync();
    }
}