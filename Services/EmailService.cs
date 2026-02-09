using Microsoft.Extensions.Configuration;
using ServicesAbstraction;
using Shared.Dtos.User;
using System.Net;
using System.Net.Mail;

namespace Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(EmailDto email)
        {
            var smtpSettings = _configuration.GetSection("SmtpSettings");

            var client = new SmtpClient(
                smtpSettings["Server"],
                int.Parse(smtpSettings["Port"])
            )
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(
                    smtpSettings["Username"],
                    smtpSettings["Password"]
                )
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(smtpSettings["FromEmail"]),
                Subject = email.Subject,
                Body = email.Body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(email.To);
            await client.SendMailAsync(mailMessage);
        }
    }
}
