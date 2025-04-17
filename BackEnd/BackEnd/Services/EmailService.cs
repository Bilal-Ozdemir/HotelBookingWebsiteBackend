using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System;

namespace BackEnd.Services
{
    public class EmailSettings
    {
        public string Host     { get; set; }  
        public int    Port     { get; set; }  
        public string Username { get; set; }  
        public string Password { get; set; } 
    }

    public class EmailService : IEmailService
    {
        private readonly EmailSettings _opts;

        public EmailService(IOptions<EmailSettings> opts)
            => _opts = opts.Value;

        public async Task SendAsync(
            string fromEmail, string fromName,
            string toEmail,   string subject,
            string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(fromName, fromEmail));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;
            message.Body    = new TextPart("plain") { Text = body };

            using var client = new SmtpClient
            {
                // 10s timeout for connect/send
                Timeout = 10000
            };

            SecureSocketOptions socketOption;
            if (_opts.Port == 465)
                socketOption = SecureSocketOptions.SslOnConnect;
            else
                socketOption = SecureSocketOptions.StartTls;

            try
            {
                await client.ConnectAsync(_opts.Host, _opts.Port, socketOption);
                await client.AuthenticateAsync(_opts.Username, _opts.Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                // you can log ex.Message here
                throw new InvalidOperationException(
                    $"Email send failed: {ex.Message}", ex);
            }
        }
    }
}
