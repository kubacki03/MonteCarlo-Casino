using System.Net.Mail;
using System.Net;
using System.Net.Mime;

namespace MonteCarlo.NET.Services
{
    public class EmailService
    {

        string apiKey = Environment.GetEnvironmentVariable("GMAIL_KEY");

        public void SendEmail(string email, int code, long payout)
        {
            try
            {
                string baseUrl = "https://localhost:7168";
                string reportUrl = $"{baseUrl}/api/payout/report?email={email}";

                MailMessage mail = new MailMessage
                {
                    From = new MailAddress("polichronowe@gmail.com"),
                    Subject = "Potwierdzenie wypłaty z MonteCarlo",
                    Body = $@"
                <html>
                    <body>
                        <p>Aby potwierdzić wypłatę, podaj na stronie ten kod: <b>{code}</b>.</p>
                        <p>Jeśli to nie Ty, kliknij w poniższy przycisk, aby wysłać zgłoszenie:</p>
                        <a href='{reportUrl}' style='background-color: #c95908; color: white; padding: 15px 20px; text-align: center; text-decoration: none; display: inline-block; font-size: 16px; border-radius: 5px;'>Zgłoś nieautoryzowaną wypłatę</a>
                        <br/><br/>
                        <img src='cid:image1' alt='Obrazek' />
                    </body>
                </html>",
                    IsBodyHtml = true
                };

                mail.To.Add(email);

               
                string imagePath = "C:\\Users\\Kuba\\source\\repos\\montecarlo.net\\MonteCarlo.NET\\wwwroot\\images\\dukeFight.jpg";

                Attachment inlineImage = new Attachment(imagePath);
                inlineImage.ContentId = "image1";  
                inlineImage.ContentDisposition.Inline = true;
                inlineImage.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                mail.Attachments.Add(inlineImage);

                
                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587)
                {
                    EnableSsl = true,
                    Credentials = new NetworkCredential("polichronowe@gmail.com", apiKey)
                };

                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Wystąpił błąd podczas wysyłania e-maila: {ex.Message}");
            }
        }

    }
}
