using System.ServiceProcess;

namespace cron.net
{
    internal class Program
    {
        public static void Main() => ServiceBase.Run(new ServiceBase[] { new CronService() });
    }
}