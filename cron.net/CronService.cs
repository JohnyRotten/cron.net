using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Threading;
using cron.net.Utils.Logging;

namespace cron.net
{
    public sealed class CronService : ServiceBase
    {

        private readonly CronWorker _worker;

        private string ServiceFolder => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments),
            nameof(ServiceName));

        private string LogPath => Path.Combine(ServiceFolder, "cron.log");

        public CronService()
        {
            ServiceName = nameof(CronService);
            if (!Directory.Exists(ServiceFolder)) Directory.CreateDirectory(ServiceFolder);
            InitEventLog();
            ILogger logger = new LoggerComposer(
                new EventLogger(EventLog),
                new StreamLogger(() => new FileStream(LogPath, FileMode.Append))
            );
            _worker = new CronWorker(logger);
        }

        private void InitEventLog()
        {
            EventLog.Source = ServiceName;
            EventLog.Log = "Application";
            ((ISupportInitialize)EventLog).BeginInit();
            if (!EventLog.SourceExists(EventLog.Source))
            {
                EventLog.CreateEventSource(EventLog.Source, EventLog.Log);
            }
            ((ISupportInitialize)EventLog).EndInit();
        }

        protected override void OnStart(string[] args)
        {
            ThreadPool.QueueUserWorkItem(state => _worker.Start());
        }

        protected override void OnStop()
        {
            _worker.Stop();
        }

    }
}