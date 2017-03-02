namespace cron.net.Utils.Logging
{
    public class LoggerComposer : ILogger
    {
        private readonly ILogger[] _loggers;

        public LoggerComposer(params ILogger[] loggers)
        {
            _loggers = loggers;
        }

        public void Log(string message)
        {
            foreach (var logger in _loggers)
            {
                logger.Log(message);
            }
        }
    }
}