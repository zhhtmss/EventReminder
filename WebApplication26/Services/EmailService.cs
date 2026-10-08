using MimeKit;
using MailKit.Net.Smtp;
namespace WebApplication26.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(
            string recipient,
            string subject,
            string message)
        {
            var emailSettings = _configuration
                .GetSection("EmailSettings");

            var email = new MimeMessage();

            email.From.Add(new MailboxAddress(
                emailSettings["SenderName"],
                emailSettings["SenderEmail"]));

            email.To.Add(MailboxAddress.Parse(recipient));

            email.Subject = subject;

            email.Body = new TextPart("plain")
            {
                Text = message
            };

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(
                emailSettings["SmtpServer"],
                int.Parse(emailSettings["Port"]!),
                true);

            await smtp.AuthenticateAsync(
                emailSettings["Username"],
                emailSettings["Password"]);

            await smtp.SendAsync(email);

            await smtp.DisconnectAsync(true);
        }
    }
}
