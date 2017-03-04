using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using cron.net.Configs;
using cron.net.Utils.Logging;
using cron.net.Utils.Serialization;
using NUnit.Framework;

namespace cron.net.Tests
{
    [TestFixture]
    public class CronTests
    {
        [Test]
        public void CronLineParserTest()
        {
            var testDictionary = new Dictionary<string, Func<CronCommandLine, bool>>
            {
                {
                    "5 0 * * * $HOME/bin/daily.job >> $HOME/log/daily 2>&1",
                    c => c.Minutes.All(i => i == 5)
                },
                {
                    "* * * * * dir",
                    c => c.CheckDateTime()
                },
                {
                    "23 */2 * * * echo \"Выполняется в 0:23, 2:23, 4:23 и т. д.\"",
                    c => c.CheckDateTime(new DateTime(2017, 12, 1, 0, 23, 0)) && !c.CheckDateTime(new DateTime(2017, 12, 1, 1, 23, 0))
                },
                {
                    "0-59/2 * * * * echo \"Выполняется по четным минутам\"",
                    c => c.Minutes.All(i => i%2 == 0)
                },
                {
                    "1-59/2 * * * * echo \"Выполняется по нечетным минутам\"",
                    c => c.Minutes.All(i => i%2 == 1)
                }
            };
            foreach (var pair in testDictionary)
            {
                Assert.IsTrue(pair.Value(new CronCommandLine(pair.Key)));
            }
        }

        [Test]
        public void RunTest()
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments),
                ".cron", "cron.xml");
            var settings = new XmlSerializer<CronSettings>(path).Get();
            var worker = new CronWorker(settings.SmtpSettings, new ConsoleLogger());
            worker.Run();
        }
    }
}