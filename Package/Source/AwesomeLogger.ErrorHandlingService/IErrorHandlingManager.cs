using System;

namespace AwesomeLogger.ErrorHandlingService
{
    internal interface IErrorHandlingManager : IDisposable
    {
        void Start();
    }
}