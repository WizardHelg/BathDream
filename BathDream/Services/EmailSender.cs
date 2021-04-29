using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BathDream.Services
{
    public class EmailSender
    {
        public async void Send(string email, string subject, string message, List<string> attachments = null)
        {
            var email_message = new MimeMessage();
            email_message.From.Add(new MailboxAddress("BathDream", "order@bath-dream.ru"));
            email_message.To.Add(new MailboxAddress("", email));
            email_message.Subject = subject;

            var builder = new BodyBuilder
            {
                HtmlBody = message
            };
            if(attachments != null)
                foreach (var item in attachments)
                    builder.Attachments.Add(item);

            email_message.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync("smtp.yandex.ru", 465, true);
            await smtp.AuthenticateAsync("order@bath-dream.ru", "BathDream2021");
            await smtp.SendAsync(email_message);
            await smtp.DisconnectAsync(true);
        }
    }
}
