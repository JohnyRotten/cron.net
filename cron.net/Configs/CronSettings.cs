using System;
using cron.net.Utils.Mailing;

namespace cron.net.Configs
{
    [Serializable]
    public class CronSettings
    {
        public SmtpServerSettings SmtpSettings { get; set; }
    }
}