using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using cron.net.Utils.Logging;
using cron.net.Utils.Mailing;

namespace cron.net
{
    public class CronWorker
    {

        private static class Consts
        {
            public const string MailTo = "MAILTO";
            public const string From = "cron@example.com";
            public const string Subject = "Cron notify";
        }

        private static string CronTabPath =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), "cron.txt");

        private readonly IEnumerable<CronCommandLine> _lines;

        private readonly ILogger _logger;
        private readonly EmailSender _sender;

        private readonly string _mailTo;
        private readonly Dictionary<string, string> _parameters;

        public CronWorker(SmtpServerSettings mailSenderSettings, ILogger logger = null)
        {
            _logger = logger ?? new ConsoleLogger();
            var lines = File.ReadAllLines(CronTabPath).ToList();
            _lines = ParseCommands(lines);
            _parameters = ParseParameters(lines);
            if (_parameters.ContainsKey(Consts.MailTo))
            {
                _mailTo = _parameters[Consts.MailTo];
                _sender = new EmailSender(mailSenderSettings);
            }
        }

        private IEnumerable<CronCommandLine> ParseCommands(IList<string> lines)
        {
            var list = new List<CronCommandLine>();
            foreach (var line in lines.Where(l => !l.Trim().StartsWith("#")))
            {
                try
                {
                    list.Add(new CronCommandLine(line));
                }
                catch (Exception e)
                {
                    _logger.Log($"Error in line {lines.IndexOf(line)}: {e.Message}");
                }
            }
            return list;
        }

        private static Dictionary<string, string> ParseParameters(IEnumerable<string> lines)
        {
            var dictionary = new Dictionary<string, string>();
            foreach (var line in lines.Where(l => l.Contains('=')))
            {
                var items = line.Split('=');
                dictionary[items[0].ToUpper()] = items[1];
            }
            return dictionary;
        }

        public void Run()
        {
            Parallel.ForEach(_lines.Where(l => l.CheckDateTime()), line =>
            {
                try
                {
                    _logger.Log($"Run command: {line.Command}");
                    var info = new ProcessStartInfo
                    {
                        CreateNoWindow = true,
                        FileName = "cmd",
                        Arguments = $"/c {line.Command}",
                        UseShellExecute = false,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true
                    };
                    var process = Process.Start(info);
                    if (_sender != null && process != null)
                    {
                        process.WaitForExit();
                        var result = process.StandardOutput.ReadToEnd();
                        result += process.StandardError.ReadToEnd();
                        if (!string.IsNullOrWhiteSpace(result))
                        {
                            try
                            {
                                _sender.Send(Consts.From, _mailTo, Consts.Subject,
                                    $"Command '{line.Command}' executed. Results:{Environment.NewLine}{result}");
                            }
                            catch (Exception e)
                            {
                                _logger.Log($"Error mail sending: {e.Message}");
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.Log($"Process execption: {e.Message}");
                }
            });
        }

    }
}