using System.Net;
using System.Net.Mail;

namespace cron.net.Utils.Mailing
{
    public class EmailSender
    {
        private readonly SmtpClient _client;

        public EmailSender(SmtpServerSettings settings)
        {
            _client = new SmtpClient
            {
                Host = settings.Host,
                Port = settings.Port,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                EnableSsl = settings.EnableSsl,
                Credentials = new NetworkCredential(settings.Username, settings.Password)
            };
        }

        public void Send(string from, string to, string subject, string message)
        {
            _client.Send(new MailMessage(from, to)
            {
                Subject = subject,
                Body = message
            });
        }
    }
}