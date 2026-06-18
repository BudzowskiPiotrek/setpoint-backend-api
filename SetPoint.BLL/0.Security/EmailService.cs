using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using MimeKit.Text;

namespace SetPoint.BLL._0.Security
{
    public class EmailService : IEmailService
    {
        #region Fields
        private readonly ILogger<EmailService> _logger;
        private readonly IConfiguration _config;
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _senderEmail;
        private readonly string _senderPassword;
        #endregion


        #region Constructors
        public EmailService(IConfiguration config, ILogger<EmailService> logger)
        {
            _config = config;
            _logger = logger;

            _smtpServer = _config["EmailSettings:Host"]
                ?? throw new InvalidOperationException("Email Host not found in configuration.");

            _smtpPort = int.TryParse(_config["EmailSettings:Port"], out var port) ? port : 587;

            _senderEmail = _config["EmailSettings:Username"]
                ?? throw new InvalidOperationException("Email Username not found in configuration.");

            _senderPassword = _config["EmailSettings:Password"]
                ?? throw new InvalidOperationException("Email Password not found in configuration.");
        }
        #endregion


        #region Methods
        public async Task<bool> SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            var email = new MimeMessage();

            email.From.Add(new MailboxAddress("SetPoint App", _senderEmail));

            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html)
            {
                Text = htmlBody
            };

            using (var smtp = new SmtpClient())
            {
                try
                {
                    await smtp.ConnectAsync(_smtpServer, _smtpPort, SecureSocketOptions.StartTls);
                    await smtp.AuthenticateAsync(_senderEmail, _senderPassword);
                    await smtp.SendAsync(email);
                    await smtp.DisconnectAsync(true);
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send email to {ToEmail}", toEmail);
                    return false;
                }
            }
        }
        #endregion
    }
}
