using System;

namespace AwesomeLogger.Monitor
{
    internal interface IMonitorManager : IDisposable
    {
        void Start();
    }
}