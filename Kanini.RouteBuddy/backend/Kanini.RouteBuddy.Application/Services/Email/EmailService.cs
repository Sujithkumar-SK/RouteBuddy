using System.Reflection;
using Kanini.RouteBuddy.Application.Services.Pdf;
using Kanini.RouteBuddy.Common;
using Kanini.RouteBuddy.Common.Utility;
using Kanini.RouteBuddy.Data.Repositories.Email;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace Kanini.RouteBuddy.Application.Services.Email;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;
    private readonly IEmailRepository _emailRepository;
    private readonly IPdfService _pdfService;

    public EmailService(
        IConfiguration configuration,
        ILogger<EmailService> logger,
        IEmailRepository emailRepository,
        IPdfService pdfService
    )
    {
        _configuration = configuration;
        _logger = logger;
        _emailRepository = emailRepository;
        _pdfService = pdfService;
    }

    public async Task<Result<string>> SendBookingConfirmationAsync(int bookingId)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.EmailSendingStarted, bookingId);

            var bookingData = await _emailRepository.GetBookingDetailsForEmailAsync(bookingId);
            if (bookingData == null)
            {
                _logger.LogWarning(
                    MagicStrings.LogMessages.BookingEmailDataRetrievalFailed,
                    bookingId,
                    MagicStrings.ErrorMessages.BookingDataNotFound
                );
                return Result.Failure<string>(
                    Error.NotFound(
                        MagicStrings.ErrorCodes.BookingDataNotFound,
                        MagicStrings.ErrorMessages.BookingDataNotFound
                    )
                );
            }

            var pdfResult = await _pdfService.GenerateBookingTicketAsync(bookingData);
            if (pdfResult.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.PdfGenerationFailed,
                    bookingId,
                    pdfResult.Error.Description
                );
                return Result.Failure<string>(pdfResult.Error);
            }

            var htmlContent = await GenerateEmailHtmlAsync(bookingData);
            var subject = string.Format(
                MagicStrings.EmailTemplates.BookingConfirmationSubject,
                bookingData.PNRNo
            );

            // Send email with PDF attachment
            var emailResult = await SendEmailAsync(
                bookingData.CustomerEmail,
                subject,
                htmlContent,
                pdfResult.Value
            );
            if (emailResult.IsFailure)
            {
                return Result.Failure<string>(emailResult.Error);
            }

            _logger.LogInformation(MagicStrings.LogMessages.EmailSendingCompleted, bookingId);
            return Result.Success(MagicStrings.SuccessMessages.EmailSentSuccessfully);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                MagicStrings.LogMessages.EmailSendingFailed,
                bookingId,
                ex.Message
            );
            return Result.Failure<string>(
                Error.Failure(MagicStrings.ErrorCodes.EmailSendingFailed, ex.Message)
            );
        }
    }

    public async Task<Result<string>> SendConnectingBookingConfirmationAsync(int bookingId)
    {
        try
        {
            _logger.LogInformation(
                MagicStrings.LogMessages.ConnectingEmailSendingStarted,
                bookingId
            );

            // Get connecting booking data from repository (same method works for both)
            var bookingData = await _emailRepository.GetBookingDetailsForEmailAsync(bookingId);
            if (bookingData == null)
            {
                _logger.LogWarning(
                    MagicStrings.LogMessages.BookingEmailDataRetrievalFailed,
                    bookingId,
                    MagicStrings.ErrorMessages.BookingDataNotFound
                );
                return Result.Failure<string>(
                    Error.NotFound(
                        MagicStrings.ErrorCodes.BookingDataNotFound,
                        MagicStrings.ErrorMessages.BookingDataNotFound
                    )
                );
            }

            // Generate PDF ticket for connecting route
            var pdfResult = await _pdfService.GenerateBookingTicketAsync(bookingData);
            if (pdfResult.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.PdfGenerationFailed,
                    bookingId,
                    pdfResult.Error.Description
                );
                return Result.Failure<string>(pdfResult.Error);
            }

            // Generate HTML email content
            var htmlContent = await GenerateEmailHtmlAsync(bookingData);
            var subject = string.Format(
                MagicStrings.EmailTemplates.ConnectingBookingConfirmationSubject,
                bookingData.PNRNo
            );

            // Send email with PDF attachment
            var emailResult = await SendEmailAsync(
                bookingData.CustomerEmail,
                subject,
                htmlContent,
                pdfResult.Value
            );
            if (emailResult.IsFailure)
            {
                return Result.Failure<string>(emailResult.Error);
            }

            _logger.LogInformation(
                MagicStrings.LogMessages.ConnectingEmailSendingCompleted,
                bookingId
            );
            return Result.Success(MagicStrings.SuccessMessages.EmailSentSuccessfully);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                MagicStrings.LogMessages.ConnectingEmailSendingFailed,
                bookingId,
                ex.Message
            );
            return Result.Failure<string>(
                Error.Failure(MagicStrings.ErrorCodes.EmailSendingFailed, ex.Message)
            );
        }
    }

    private async Task<Result<string>> SendEmailAsync(
        string toEmail,
        string subject,
        string htmlBody,
        byte[]? pdfAttachment = null
    )
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(
                new MailboxAddress(
                    _configuration[MagicStrings.ConfigKeys.EmailSenderName],
                    _configuration[MagicStrings.ConfigKeys.EmailSenderEmail]
                )
            );
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = htmlBody };

            if (pdfAttachment != null)
            {
                bodyBuilder.Attachments.Add(
                    MagicStrings.EmailAttachments.TicketFileName,
                    pdfAttachment,
                    ContentType.Parse(MagicStrings.EmailAttachments.PdfContentType)
                );
            }

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(
                _configuration[MagicStrings.ConfigKeys.SmtpServer],
                int.Parse(_configuration[MagicStrings.ConfigKeys.SmtpPort] ?? "587"),
                SecureSocketOptions.StartTls
            );

            await client.AuthenticateAsync(
                _configuration[MagicStrings.ConfigKeys.EmailUsername],
                _configuration[MagicStrings.ConfigKeys.EmailPassword]
            );

            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            return Result.Success(MagicStrings.SuccessMessages.EmailSentSuccessfully);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.SmtpConnectionFailed, ex.Message);
            return Result.Failure<string>(
                Error.Failure(MagicStrings.ErrorCodes.SmtpConnectionFailed, ex.Message)
            );
        }
    }

    private async Task<string> GenerateEmailHtmlAsync(BookingEmailData bookingData)
    {
        try
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Kanini.RouteBuddy.Application.BookingConfirmationEmail.html";

            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                // Fallback: read from file system
                var templatePath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "BookingConfirmationEmail.html"
                );
                var fileTemplate = await File.ReadAllTextAsync(templatePath);
                return PopulateEmailTemplate(fileTemplate, bookingData);
            }

            using var reader = new StreamReader(stream);
            var resourceTemplate = await reader.ReadToEndAsync();
            return PopulateEmailTemplate(resourceTemplate, bookingData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load email template: {Error}", ex.Message);
            return GenerateFallbackEmailHtml(bookingData);
        }
    }

    private static string PopulateEmailTemplate(string template, BookingEmailData bookingData)
    {
        var customerName = $"{bookingData.FirstName} {bookingData.LastName}".Trim();
        var passengerRows = string.Join(
            "",
            bookingData.Passengers.Select(p =>
                $"<tr><td>{p.PassengerName}</td><td>{p.PassengerAge}</td><td>{GetGenderText(p.PassengerGender)}</td><td>{p.SeatNumber}</td></tr>"
            )
        );

        return template
            .Replace("{{CustomerName}}", customerName)
            .Replace("{{PNR}}", bookingData.PNRNo)
            .Replace("{{Source}}", bookingData.Source)
            .Replace("{{Destination}}", bookingData.Destination)
            .Replace("{{TravelDate}}", bookingData.TravelDate.ToString("dd MMM yyyy"))
            .Replace("{{BusName}}", bookingData.BusName)
            .Replace("{{DepartureTime}}", bookingData.DepartureTime.ToString(@"hh\:mm"))
            .Replace("{{ArrivalTime}}", bookingData.ArrivalTime.ToString(@"hh\:mm"))
            .Replace("{{BoardingStop}}", bookingData.BoardingStopName)
            .Replace("{{DroppingStop}}", bookingData.DroppingStopName)
            .Replace("{{PassengerRows}}", passengerRows)
            .Replace("{{TotalAmount}}", bookingData.TotalAmount.ToString("F2"));
    }

    private static string GenerateFallbackEmailHtml(BookingEmailData bookingData)
    {
        var customerName = $"{bookingData.FirstName} {bookingData.LastName}".Trim();
        var seatNumbers = string.Join(", ", bookingData.Passengers.Select(p => p.SeatNumber));

        return $@"
        <html><body style='font-family: Arial, sans-serif;'>
        <h2>🎉 Booking Confirmed!</h2>
        <p>Dear {customerName},</p>
        <p>Your bus booking has been confirmed.</p>
        <div style='background: #f5f5f5; padding: 15px; margin: 20px 0;'>
            <h3>Booking Details</h3>
            <p><strong>PNR:</strong> {bookingData.PNRNo}</p>
            <p><strong>Route:</strong> {bookingData.Source} → {bookingData.Destination}</p>
            <p><strong>Travel Date:</strong> {bookingData.TravelDate:dd MMM yyyy}</p>
            <p><strong>Bus:</strong> {bookingData.BusName}</p>
            <p><strong>Seats:</strong> {seatNumbers}</p>
            <p><strong>Total Amount:</strong> ₹{bookingData.TotalAmount:F2}</p>
        </div>
        <p>Thank you for choosing RouteBuddy!</p>
        </body></html>";
    }

    private static string GetGenderText(int gender)
    {
        return gender switch
        {
            1 => "Male",
            2 => "Female",
            3 => "Other",
            _ => "N/A",
        };
    }
}
