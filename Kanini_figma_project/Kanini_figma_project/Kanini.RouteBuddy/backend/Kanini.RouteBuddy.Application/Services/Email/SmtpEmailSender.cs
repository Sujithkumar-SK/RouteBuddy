using Kanini.RouteBuddy.Application.Common;
using Kanini.RouteBuddy.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;

namespace Kanini.RouteBuddy.Application.Services.Email;

public class SmtpEmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SmtpEmailSender> _logger;

    public SmtpEmailSender(IConfiguration configuration, ILogger<SmtpEmailSender> logger)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.EmailSendingStarted, toEmail);

            var fromEmail = _configuration["EmailSettings:FromEmail"];
            var appPassword = _configuration["EmailSettings:AppPassword"];

            if (string.IsNullOrEmpty(fromEmail))
            {
                _logger.LogError("Sender email not configured in appsettings.json");
                throw new ArgumentNullException("SenderEmail", "Sender email not configured in appsettings.json");
            }

            if (string.IsNullOrEmpty(appPassword))
            {
                _logger.LogError("App password not configured in appsettings.json");
                throw new ArgumentNullException("AppPassword", "App password not configured in appsettings.json");
            }

            using var client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(fromEmail, appPassword),
                EnableSsl = true
            };

            var mail = new MailMessage
            {
                From = new MailAddress(fromEmail, "RouteBuddy Support"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mail.To.Add(toEmail);
            await client.SendMailAsync(mail);

            _logger.LogInformation(MagicStrings.LogMessages.EmailSendingCompleted, toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.EmailSendingFailed, toEmail, ex.Message);
            throw;
        }
    }
}