using System;
using System.IO;

namespace cron.net.Utils.Logging
{
    public class StreamLogger : ILogger
    {
        private readonly Func<Stream> _streamFactory;

        public StreamLogger(Func<Stream> streamFactory)
        {
            _streamFactory = streamFactory;
        }

        public void Log(string message)
        {
            using (var stream = _streamFactory())
            using (var writer = new StreamWriter(stream))
            {
                writer.WriteLine($"{DateTime.Now} {message}");
            }
        }
    }
}