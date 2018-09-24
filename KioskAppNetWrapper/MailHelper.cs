using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit;
using MimeKit;

namespace KioskAppNetWrapper
{
    public static class MailHelper
    {
        public static void sendMail(string subject, string body)
        {
            try
            {

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("HoursReportNetWrapperApp Notification", "maayan.hahinuch@gmail.com"));
                message.To.Add(new MailboxAddress("Adico", "adico1@gmail.com"));
                message.Subject = subject;

                message.Body = new TextPart("plain")
                {
                    Text = body
                };

                using (var client = new SmtpClient())
                {
                    // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    client.Connect("smtp.gmail.com", 587, false);

                    // Note: only needed if the SMTP server requires authentication
                    client.Authenticate("maayan.hahinuch@gmail.com", "maayan_1234");

                    client.Send(message);
                    client.Disconnect(true);
                }
            } catch (Exception ex) { }
        }
    }
}
