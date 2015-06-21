using System;

namespace AwesomeLogger.NotificationService
{
    internal interface INotificationManager: IDisposable
    {
        void Start();
    }
}