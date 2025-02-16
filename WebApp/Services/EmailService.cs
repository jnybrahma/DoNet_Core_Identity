﻿using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Options;
using WebApp.Settings;

namespace WebApp.Services
{
    public class EmailService : IEmailService
    {
        private readonly IOptions<SmtpSetting> smtpSetting;

        public EmailService(IOptions<SmtpSetting> smtpSetting)
        {
            this.smtpSetting = smtpSetting;
        }
        public async Task SendAsync(string from, string to, string subject, string body)
        {
            var message = new MailMessage(from, to, subject, body);
            using (var emailClient = new SmtpClient(smtpSetting.Value.Host, smtpSetting.Value.Port))
            {
                emailClient.EnableSsl = true;
                emailClient.Credentials = new NetworkCredential(smtpSetting.Value.User, smtpSetting.Value.Password);

                await emailClient.SendMailAsync(message);
            }
        }
    }
}
