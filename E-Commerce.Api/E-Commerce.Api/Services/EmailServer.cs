using DotNetEnv;
using E_Commerce.Api.Services.Interfaces;
using System.Net;
using System.Net.Mail;

namespace E_Commerce.Api.Services
{
    public class EmailServer : IEmailService
    {
        public Task SendEmailAsync(string toEmail, string subject, string body, bool isBodyHtml = false)
        {
            Env.Load();
            string? smtpServer = Environment.GetEnvironmentVariable("SMTP_SERVER");
            int? smtpPort = int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT"));
            string? senderName = Environment.GetEnvironmentVariable("SENDER_NAME");
            string? senderEmail = Environment.GetEnvironmentVariable("SENDER_EMAIL");
            string? password = Environment.GetEnvironmentVariable("PASSWORD");

            var client = new SmtpClient(smtpServer, (int)smtpPort)
            {
                // Set the credentials (email and password) for the SMTP server.
                Credentials = new NetworkCredential(senderEmail, password),
                // Enable SSL for secure email communication.
                EnableSsl = true,
            };

            MailAddress fromAddress = new MailAddress(senderEmail, senderName);
            MailMessage Message = new MailMessage
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = isBodyHtml,
                From = fromAddress
            };
            Message.To.Add(toEmail);
            return client.SendMailAsync(Message);
        }
    }
}