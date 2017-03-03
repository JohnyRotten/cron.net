using System.ComponentModel;
using System.ServiceProcess;

namespace cron.net
{
    [RunInstaller(true)]
    public class CronInstaller : ServiceInstaller
    {
        public CronInstaller()
        {
            var spi = new ServiceProcessInstaller();
            var si = new ServiceInstaller();

            spi.Account = ServiceAccount.LocalSystem;
            spi.Username = null;
            spi.Password = null;

            si.DisplayName = CronService.DisplayName;
            si.ServiceName = CronService.DisplayName;
            si.StartType = ServiceStartMode.Automatic;

            Installers.Add(spi);
            Installers.Add(si);
        }
    }
}