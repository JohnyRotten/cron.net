using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using cron.net.Utils.Logging;

namespace cron.net
{
    public class CronWorker
    {

        private volatile bool _running;

        private static string CronTabPath =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), "cron.txt");

        private readonly IEnumerable<CronCommandLine> _lines;

        private readonly ILogger _logger;

        public CronWorker(ILogger logger = null)
        {
            _logger = logger ?? new ConsoleLogger();
            var lines = File.ReadAllLines(CronTabPath);
            _lines = lines.Where(l => l.Trim().StartsWith("#"))
                .Select(l => new CronCommandLine(l))
                .ToList();
        }

        public void Start()
        {
            _running = true;
            _logger.Log("Cron started.");
            while (_running)
            {
                _lines.Where(l => l.CheckDateTime()).AsParallel().ForAll(line =>
                {
                    _logger.Log($"Run command: {line.Command}");
                    Process.Start(new ProcessStartInfo
                    {
                        CreateNoWindow = true,
                        FileName = "cmd",
                        Arguments = $"/c {line.Command}",
                        WindowStyle = ProcessWindowStyle.Hidden
                    });
                });
                Thread.Sleep(TimeSpan.FromMinutes(1));
            }
            _logger.Log("Cron ended.");
        }

        public void Stop()
        {
            _running = false;
        }

    }
}