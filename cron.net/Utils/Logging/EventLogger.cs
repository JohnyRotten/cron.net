using System;
using System.Diagnostics;

namespace cron.net.Utils.Logging
{
    public class EventLogger : ILogger
    {
        private readonly EventLog _log;

        public EventLogger(EventLog log)
        {
            _log = log;
        }

        public void Log(string message)
        {
            _log.WriteEntry($"{DateTime.Now} {message}", EventLogEntryType.Information);
        }
    }
}