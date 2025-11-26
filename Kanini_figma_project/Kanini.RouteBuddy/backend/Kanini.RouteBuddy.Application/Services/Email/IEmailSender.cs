namespace Kanini.RouteBuddy.Application.Common;

public interface IEmailSender
{
    Task SendEmailAsync(string toEmail, string subject, string body);
}