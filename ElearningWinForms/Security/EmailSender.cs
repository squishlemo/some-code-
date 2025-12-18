using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace ElearningWinForms.Security
{
    public static class EmailSender
    {
        private const string DefaultSystemEmail = "elearning.demoing@gmail.com";
        private const string DefaultSystemEmailAppPassword = "dkve qxvo pcyc jajy";

        public static string GetSystemEmail()
        {
            string env = Environment.GetEnvironmentVariable("ELEARNING_SYSTEM_EMAIL");
            if (!string.IsNullOrWhiteSpace(env)) return env;

            string cfg = ConfigurationManager.AppSettings["SystemEmail"];
            if (!string.IsNullOrWhiteSpace(cfg)) return cfg;

            return DefaultSystemEmail;
        }

        public static string GetSystemEmailAppPassword()
        {
            string env = Environment.GetEnvironmentVariable("ELEARNING_SYSTEM_EMAIL_APP_PASSWORD");
            if (!string.IsNullOrWhiteSpace(env)) return env;

            string cfg = ConfigurationManager.AppSettings["SystemEmailAppPassword"];
            if (!string.IsNullOrWhiteSpace(cfg)) return cfg;

            return DefaultSystemEmailAppPassword;
        }

        public static void SendHtmlEmail(string toEmail, string subject, string htmlBody)
        {
            if (string.IsNullOrWhiteSpace(toEmail))
                throw new ArgumentException("toEmail is empty");

            using (var msg = new MailMessage())
            {
                msg.From = new MailAddress(GetSystemEmail(), "Elearning System");
                msg.To.Add(toEmail);
                msg.Subject = subject;
                msg.Body = htmlBody;
                msg.IsBodyHtml = true;

                Send(msg);
            }
        }

        /// <summary>
        /// Gửi email theo dạng BCC để không lộ danh sách người nhận.
        /// Nếu danh sách lớn sẽ tự chia batch.
        /// </summary>
        public static void SendHtmlEmailBcc(IEnumerable<string> emails, string subject, string htmlBody, int batchSize = 40)
        {
            if (emails == null) throw new ArgumentNullException(nameof(emails));

            var list = emails
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (list.Count == 0) return;
            if (batchSize < 1) batchSize = 1;

            for (int i = 0; i < list.Count; i += batchSize)
            {
                var chunk = list.Skip(i).Take(batchSize).ToList();

                using (var msg = new MailMessage())
                {
                    msg.From = new MailAddress(GetSystemEmail(), "Elearning System");
                    // Gmail thường yêu cầu có ít nhất 1 "To" — dùng chính hệ thống.
                    msg.To.Add(GetSystemEmail());
                    foreach (var email in chunk)
                        msg.Bcc.Add(email);
                    msg.Subject = subject;
                    msg.Body = htmlBody;
                    msg.IsBodyHtml = true;

                    Send(msg);
                }
            }
        }

        private static void Send(MailMessage message)
        {
            using (var client = new SmtpClient("smtp.gmail.com", 587))
            {
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(GetSystemEmail(), GetSystemEmailAppPassword());
                client.Send(message);
            }
        }
    }
}
