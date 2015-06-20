using System.ComponentModel;
using System.Configuration.Install;

namespace AwesomeLogger.NotificationService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        private void ClickberryServiceInstaller_AfterInstall(object sender, InstallEventArgs e)
        {
        }

        private void serviceProcessInstaller1_AfterInstall(object sender, InstallEventArgs e)
        {

        }
    }
}