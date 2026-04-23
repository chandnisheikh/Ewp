using MailKit.Net.Smtp;
using MimeKit;

namespace Ewp.Services
{
    public class EmailService
    {
        public void SendEmail(string toEmail, string subject, string body)
        {
            var email = new MimeMessage();

            // ✅ YOUR REAL GMAIL
            email.From.Add(MailboxAddress.Parse("yourrealemail@gmail.com"));

            email.To.Add(MailboxAddress.Parse(toEmail));

            email.Subject = subject;

            email.Body = new TextPart("html")
            {
                Text = body
            };

            using var smtp = new SmtpClient();

            // ✅ Gmail SMTP
            smtp.Connect("smtp.gmail.com", 587, false);

            // 🔥 IMPORTANT: USE APP PASSWORD (NOT NORMAL PASSWORD)
            smtp.Authenticate("yourrealemail@gmail.com", "your_app_password_here");

            smtp.Send(email);

            smtp.Disconnect(true);
        }
    }
}