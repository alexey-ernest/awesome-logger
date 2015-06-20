using System;

namespace AwesomeLogger.Monitor
{
    internal interface ILogMonitor: IDisposable
    {
        void Start();
    }
}