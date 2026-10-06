using System.Net;
using System.Net.Mail;

namespace MonteCarlo.NET.Services
{
    public interface IEmailService
    {
        void SendPayoutConfirmation(string email, int code, string reportUrl);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public void SendPayoutConfirmation(string email, int code, string reportUrl)
        {
            var from = _configuration["Email:From"];
            var password = _configuration["Email:Password"];
            if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(password))
            {
                _logger.LogWarning("Email:From / Email:Password not configured - payout confirmation e-mail was not sent.");
                return;
            }

            try
            {
                using var mail = new MailMessage
                {
                    From = new MailAddress(from),
                    Subject = "Potwierdzenie wypłaty z MonteCarlo",
                    Body = $@"
                <html>
                    <body>
                        <p>Aby potwierdzić wypłatę, podaj na stronie ten kod: <b>{code}</b>.</p>
                        <p>Jeśli to nie Ty, kliknij w poniższy przycisk, aby wysłać zgłoszenie:</p>
                        <a href='{reportUrl}' style='background-color: #c95908; color: white; padding: 15px 20px; text-align: center; text-decoration: none; display: inline-block; font-size: 16px; border-radius: 5px;'>Zgłoś nieautoryzowaną wypłatę</a>
                    </body>
                </html>",
                    IsBodyHtml = true
                };
                mail.To.Add(email);

                using var smtp = new SmtpClient(_configuration["Email:SmtpHost"] ?? "smtp.gmail.com", 587)
                {
                    EnableSsl = true,
                    Credentials = new NetworkCredential(from, password)
                };
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send payout confirmation e-mail.");
            }
        }
    }
}
