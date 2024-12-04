using System.Net.Mail;

namespace TarikhMaghribi.Services
{
    public class Mail
    {
        public class EmailService : ISenderEmail
        {
            private readonly IConfiguration _configuration;

            public EmailService(IConfiguration configuration)
            {
                _configuration = configuration;
            }

            public async Task SendEmailAsync(string to, string subject, string body)
            {
                var smtpClient = new SmtpClient
                {
                    Host = _configuration["SMTP:Host"],
                    Port = int.Parse(_configuration["SMTP:Port"]),
                    EnableSsl = true,
                    Credentials = new System.Net.NetworkCredential(
                        _configuration["SMTP:UserName"],
                        _configuration["SMTP:Password"]
                    )
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_configuration["SMTP:From"]),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(to);

                await smtpClient.SendMailAsync(mailMessage);
            }
        }

        public interface ISenderEmail
        {
            Task SendEmailAsync(string to, string subject, string body);
        }
    }
}
