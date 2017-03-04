using System;
using System.Diagnostics;

namespace cron.net.Utils.Logging
{
    public class EventLogger : ILogger
    {
        private readonly EventLog _log;

        public EventLogger(EventLog log, string source, EventLogType type)
        {
            _log = log;
            _log.Source = source;
            _log.Log = type.ToString();
            _log.BeginInit();
            if (!EventLog.SourceExists(_log.Source))
            {
                EventLog.CreateEventSource(_log.Source, _log.Log);
            }
            _log.EndInit();
        }

        public void Log(string message)
        {
            _log.WriteEntry($"{DateTime.Now} {message}", EventLogEntryType.Information);
        }
    }

    public enum EventLogType
    {
        Application
    }
}