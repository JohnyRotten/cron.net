using System;

namespace cron.net.Utils.Logging
{
    public class ConsoleLogger : ILogger
    {
        public void Log(string message)
        {
            Console.WriteLine($"{DateTime.Now} {message}");
        }
    }
}