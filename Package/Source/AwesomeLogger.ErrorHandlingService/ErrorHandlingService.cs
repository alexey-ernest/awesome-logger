using System.ServiceProcess;

namespace AwesomeLogger.ErrorHandlingService
{
    public partial class ErrorHandlingService : ServiceBase
    {
        public ErrorHandlingService()
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