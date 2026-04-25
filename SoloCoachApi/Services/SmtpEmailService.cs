using System.Net;
using System.Net.Mail;

namespace SoloCoachApi.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public SmtpEmailService(IConfiguration config) => _config = config;


        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            using var client = new SmtpClient(_config["Smtp:Host"], int.Parse(_config["Smtp:Port"]))
            {
                Credentials = new NetworkCredential(_config["Smtp:User"], _config["Smtp:Password"]),
                EnableSsl = true
            };

            var mail = new MailMessage
            {
                From = new MailAddress(_config["Smtp:User"], "SoloCoach"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            mail.To.Add(toEmail);
            await client.SendMailAsync(mail);
        }
    }
}
