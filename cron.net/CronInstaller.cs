using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace cron.net
{
    [RunInstaller(true)]
    public class CronInstaller : ServiceInstaller
    {
        private ServiceProcessInstaller _processInstaller;
        private ServiceInstaller _serviceInstaller;

        public CronInstaller()
        {
            ServiceName = nameof(CronService);
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            _processInstaller = new ServiceProcessInstaller();
            _serviceInstaller = new ServiceInstaller();

            _processInstaller.Account = ServiceAccount.LocalSystem;

            _serviceInstaller.ServiceName = nameof(CronService);
            _serviceInstaller.StartType = ServiceStartMode.Automatic;

            Installers.AddRange(new Installer[] {
                _processInstaller,
                _serviceInstaller});
        }
    }
}