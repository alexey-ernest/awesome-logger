using System.ServiceProcess;

namespace AwesomeLogger.Monitor
{
    public partial class MonitorService : ServiceBase
    {
        public MonitorService()
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