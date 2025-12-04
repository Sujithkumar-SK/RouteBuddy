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

public class SmartEmailService : ISmartEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SmartEmailService> _logger;
    private readonly IEmailRepository _emailRepository;
    private readonly IPdfService _pdfService;

    public SmartEmailService(
        IConfiguration configuration,
        ILogger<SmartEmailService> logger,
        IEmailRepository emailRepository,
        IPdfService pdfService
    )
    {
        _configuration = configuration;
        _logger = logger;
        _emailRepository = emailRepository;
        _pdfService = pdfService;
    }

    public async Task<Result<string>> SendConnectingBookingConfirmationAsync(int bookingId)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.SmartEmailSendingStarted, bookingId);

            var connectingBookingData =
                await _emailRepository.GetConnectingBookingDetailsForEmailAsync(bookingId);
            if (connectingBookingData == null)
            {
                _logger.LogWarning(
                    MagicStrings.LogMessages.SmartBookingEmailDataRetrievalFailed,
                    bookingId,
                    MagicStrings.ErrorMessages.ConnectingBookingDataNotFound
                );
                return Result.Failure<string>(
                    Error.NotFound(
                        MagicStrings.ErrorCodes.ConnectingBookingDataNotFound,
                        MagicStrings.ErrorMessages.ConnectingBookingDataNotFound
                    )
                );
            }

            var pdfResult = await _pdfService.GenerateConnectingBookingTicketAsync(
                connectingBookingData
            );
            if (pdfResult.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.SmartPdfGenerationFailed,
                    bookingId,
                    pdfResult.Error.Description
                );
                return Result.Failure<string>(pdfResult.Error);
            }

            var htmlContent = await GenerateConnectingEmailHtmlAsync(connectingBookingData);
            var subject = string.Format(
                MagicStrings.EmailTemplates.SmartBookingConfirmationSubject,
                connectingBookingData.PNRNo
            );

            var emailResult = await SendSmartEmailAsync(
                connectingBookingData.CustomerEmail,
                subject,
                htmlContent,
                pdfResult.Value
            );
            if (emailResult.IsFailure)
            {
                return Result.Failure<string>(emailResult.Error);
            }

            _logger.LogInformation(MagicStrings.LogMessages.SmartEmailSendingCompleted, bookingId);
            return Result.Success(MagicStrings.SuccessMessages.SmartEmailSentSuccessfully);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                MagicStrings.LogMessages.SmartEmailSendingFailed,
                bookingId,
                ex.Message
            );
            return Result.Failure<string>(
                Error.Failure(MagicStrings.ErrorCodes.SmartEmailSendingFailed, ex.Message)
            );
        }
    }

    private async Task<Result<string>> SendSmartEmailAsync(
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
                    MagicStrings.EmailAttachments.SmartTicketFileName,
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

            return Result.Success(MagicStrings.SuccessMessages.SmartEmailSentSuccessfully);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.SmartSmtpConnectionFailed, ex.Message);
            return Result.Failure<string>(
                Error.Failure(MagicStrings.ErrorCodes.SmartSmtpConnectionFailed, ex.Message)
            );
        }
    }

    private async Task<string> GenerateConnectingEmailHtmlAsync(
        ConnectingBookingEmailData bookingData
    )
    {
        try
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName =
                "Kanini.RouteBuddy.Application.ConnectingBookingConfirmationEmail.html";

            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                var templatePath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "ConnectingBookingConfirmationEmail.html"
                );
                var fileTemplate = await File.ReadAllTextAsync(templatePath);
                return PopulateConnectingEmailTemplate(fileTemplate, bookingData);
            }

            using var reader = new StreamReader(stream);
            var resourceTemplate = await reader.ReadToEndAsync();
            return PopulateConnectingEmailTemplate(resourceTemplate, bookingData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.SmartEmailTemplateLoadFailed, ex.Message);
            return GenerateFallbackConnectingEmailHtml(bookingData);
        }
    }

    private static string PopulateConnectingEmailTemplate(
        string template,
        ConnectingBookingEmailData bookingData
    )
    {
        var customerName = $"{bookingData.FirstName} {bookingData.LastName}".Trim();

        var segmentRows = string.Join(
            "",
            bookingData.Segments.Select(s =>
                $@"<tr>
                <td>{s.SegmentOrder}</td>
                <td>{s.BusName}</td>
                <td>{s.Source} → {s.Destination}</td>
                <td>{s.DepartureTime:hh\:mm} - {s.ArrivalTime:hh\:mm}</td>
                <td>{string.Join(", ", s.SeatNumbers)}</td>
                <td>₹{s.SegmentAmount:F2}</td>
            </tr>"
            )
        );

        return template
            .Replace("{{CustomerName}}", customerName)
            .Replace("{{PNR}}", bookingData.PNRNo)
            .Replace("{{OverallSource}}", bookingData.OverallSource)
            .Replace("{{OverallDestination}}", bookingData.OverallDestination)
            .Replace("{{TravelDate}}", bookingData.TravelDate.ToString("dd MMM yyyy"))
            .Replace("{{SegmentRows}}", segmentRows)
            .Replace("{{TotalAmount}}", bookingData.TotalAmount.ToString("F2"))
            .Replace("{{TotalSegments}}", bookingData.Segments.Count.ToString());
    }

    private static string GenerateFallbackConnectingEmailHtml(
        ConnectingBookingEmailData bookingData
    )
    {
        var customerName = $"{bookingData.FirstName} {bookingData.LastName}".Trim();
        var segmentDetails = string.Join(
            "<br/>",
            bookingData.Segments.Select(s =>
                $"Segment {s.SegmentOrder}: {s.BusName} ({s.Source} → {s.Destination}) - Seats: {string.Join(", ", s.SeatNumbers)}"
            )
        );

        return $@"
        <html><body style='font-family: Arial, sans-serif;'>
        <h2>🎉 Connecting Route Booking Confirmed!</h2>
        <p>Dear {customerName},</p>
        <p>Your connecting route booking has been confirmed.</p>
        <div style='background: #f5f5f5; padding: 15px; margin: 20px 0;'>
            <h3>Booking Details</h3>
            <p><strong>PNR:</strong> {bookingData.PNRNo}</p>
            <p><strong>Journey:</strong> {bookingData.OverallSource} → {bookingData.OverallDestination}</p>
            <p><strong>Travel Date:</strong> {bookingData.TravelDate:dd MMM yyyy}</p>
            <p><strong>Total Segments:</strong> {bookingData.Segments.Count}</p>
            <p><strong>Segments:</strong><br/>{segmentDetails}</p>
            <p><strong>Total Amount:</strong> ₹{bookingData.TotalAmount:F2}</p>
        </div>
        <p>Thank you for choosing RouteBuddy Smart Engine!</p>
        </body></html>";
    }
}
