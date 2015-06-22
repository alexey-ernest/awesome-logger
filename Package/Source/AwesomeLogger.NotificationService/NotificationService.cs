using System.ServiceProcess;

namespace AwesomeLogger.NotificationService
{
    public partial class NotificationService : ServiceBase
    {
        public NotificationService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Program.Start();
        }

        protected override void OnStop()
        {
            Program.Stop();
        }
    }
}