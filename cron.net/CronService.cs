using System;
using System.IO;
using System.ServiceProcess;
using System.Threading;
using cron.net.Configs;
using cron.net.Utils.Logging;
using cron.net.Utils.Serialization;
using Timer = System.Timers.Timer;

namespace cron.net
{
    public sealed class CronService : ServiceBase
    {
        private readonly Timer _timer = new Timer(60000);
        private readonly ILogger _logger;

        private string ServiceFolder => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments),
            ".cron");

        private string LogPath => Path.Combine(ServiceFolder, "cron.log");
        private string SettingsPath => Path.Combine(ServiceFolder, "cron.xml");
        private ISerializer<CronSettings> SettingsSerializer =>
            new XmlSerializer<CronSettings>(SettingsPath);

        public CronService()
        {
            InitializeComponent();
            if (!Directory.Exists(ServiceFolder)) Directory.CreateDirectory(ServiceFolder);
            _logger = new LoggerComposer(
                //new EventLogger(EventLog, "CronService", EventLogType.Application),
                new StreamLogger(() => new FileStream(LogPath, FileMode.Append))
            );
            var settings = SettingsSerializer.Get();
            var worker = new CronWorker(settings.SmtpSettings, _logger);
            _timer.AutoReset = true;
            _timer.Elapsed += (sender, args) => worker.Run();
        }

        protected override void OnStart(string[] args)
        {
            ThreadPool.QueueUserWorkItem(o =>
            {
                while (DateTime.Now.Second != 0)
                {
                }
                _timer.Start();
            });
            _logger.Log("Cron started.");
        }

        protected override void OnStop()
        {
            _timer.Stop();
            _logger.Log("Cron stopped.");
        }

        private void InitializeComponent()
        {
            ServiceName = "CronService";
        }
    }
}