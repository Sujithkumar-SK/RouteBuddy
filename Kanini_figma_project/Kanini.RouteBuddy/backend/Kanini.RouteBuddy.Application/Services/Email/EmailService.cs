using System.Net;
using System.Net.Mail;
using System.Reflection;
using Kanini.RouteBuddy.Application.Services.Pdf;
using Kanini.RouteBuddy.Common;
using Kanini.RouteBuddy.Common.Utility;
using Kanini.RouteBuddy.Data.Repositories.Email;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

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

            // Try to get connecting booking data first
            var connectingBookingData = await _emailRepository.GetConnectingBookingDetailsForEmailAsync(bookingId);
            if (connectingBookingData != null)
            {
                // Use connecting booking PDF generation
                var pdfResult = await _pdfService.GenerateConnectingBookingTicketAsync(connectingBookingData);
                if (pdfResult.IsFailure)
                {
                    _logger.LogError(
                        MagicStrings.LogMessages.PdfGenerationFailed,
                        bookingId,
                        pdfResult.Error.Description
                    );
                    return Result.Failure<string>(pdfResult.Error);
                }

                var htmlContent = await GenerateConnectingEmailHtmlAsync(connectingBookingData);
                var subject = string.Format(
                    MagicStrings.EmailTemplates.ConnectingBookingConfirmationSubject,
                    connectingBookingData.PNRNo
                );

                var emailResult = await SendEmailAsync(
                    connectingBookingData.CustomerEmail,
                    subject,
                    htmlContent,
                    pdfResult.Value
                );
                if (emailResult.IsFailure)
                {
                    return Result.Failure<string>(emailResult.Error);
                }
            }
            else
            {
                // Fallback to regular booking
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
                    MagicStrings.EmailTemplates.ConnectingBookingConfirmationSubject,
                    bookingData.PNRNo
                );

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

    public async Task<Result<string>> SendVendorRejectionEmailAsync(string vendorEmail, string vendorName, string rejectionReason)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.EmailSendingStarted, "VendorRejection");

            var subject = MagicStrings.EmailTemplates.VendorRejectionSubject;
            var body = string.Format(MagicStrings.EmailTemplates.VendorRejectionTemplate, vendorName, rejectionReason);

            var result = await SendEmailAsync(vendorEmail, subject, body);
            if (result.IsFailure)
            {
                return Result.Failure<string>(result.Error);
            }

            _logger.LogInformation(MagicStrings.LogMessages.EmailSendingCompleted, "VendorRejection");
            return Result.Success(MagicStrings.SuccessMessages.EmailSentSuccessfully);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.EmailSendingFailed, "VendorRejection", ex.Message);
            return Result.Failure<string>(
                Error.Failure(MagicStrings.ErrorCodes.EmailSendingFailed, ex.Message)
            );
        }
    }

    public async Task<Result<string>> SendVendorApprovalEmailAsync(string vendorEmail, string vendorName)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.EmailSendingStarted, "VendorApproval");

            var subject = "🎉 Vendor Account Approved - RouteBuddy";
            var body = $@"
            <html><body style='font-family: Arial, sans-serif;'>
            <h2 style='color: #28a745;'>🎉 Congratulations! Your Vendor Account is Approved</h2>
            <p>Dear {vendorName},</p>
            <p>We are pleased to inform you that your vendor account has been <strong>approved</strong> by our admin team.</p>
            <div style='background: #d4edda; border: 1px solid #c3e6cb; padding: 15px; margin: 20px 0; border-radius: 5px;'>
                <h3 style='color: #155724; margin-top: 0;'>What's Next?</h3>
                <ul style='color: #155724;'>
                    <li>You can now log in to your vendor dashboard</li>
                    <li>Add your buses and routes</li>
                    <li>Start managing your bookings</li>
                    <li>Access analytics and reports</li>
                </ul>
            </div>
            <p>Thank you for choosing RouteBuddy as your partner!</p>
            <p>Best regards,<br/>RouteBuddy Admin Team</p>
            </body></html>";

            var result = await SendEmailAsync(vendorEmail, subject, body);
            if (result.IsFailure)
            {
                return Result.Failure<string>(result.Error);
            }

            _logger.LogInformation(MagicStrings.LogMessages.EmailSendingCompleted, "VendorApproval");
            return Result.Success(MagicStrings.SuccessMessages.EmailSentSuccessfully);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.EmailSendingFailed, "VendorApproval", ex.Message);
            return Result.Failure<string>(
                Error.Failure(MagicStrings.ErrorCodes.EmailSendingFailed, ex.Message)
            );
        }
    }

    public async Task<Result<string>> SendBusNotificationEmailAsync(string vendorEmail, string vendorName, string message)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.EmailSendingStarted, "BusNotification");

            var subject = MagicStrings.EmailTemplates.BusNotificationSubject;
            var body = string.Format(MagicStrings.EmailTemplates.BusNotificationTemplate, vendorName, message);

            var result = await SendEmailAsync(vendorEmail, subject, body);
            if (result.IsFailure)
            {
                return Result.Failure<string>(result.Error);
            }

            _logger.LogInformation(MagicStrings.LogMessages.EmailSendingCompleted, "BusNotification");
            return Result.Success(MagicStrings.SuccessMessages.EmailSentSuccessfully);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.EmailSendingFailed, "BusNotification", ex.Message);
            return Result.Failure<string>(
                Error.Failure(MagicStrings.ErrorCodes.EmailSendingFailed, ex.Message)
            );
        }
    }

    public async Task<Result<string>> SendGenericEmailAsync(string toEmail, string subject, string htmlBody)
    {
        try
        {
            _logger.LogInformation("Sending generic email to: {Email}", toEmail);
            var result = await SendEmailAsync(toEmail, subject, htmlBody);
            if (result.IsFailure)
            {
                return Result.Failure<string>(result.Error);
            }
            _logger.LogInformation("Generic email sent successfully to: {Email}", toEmail);
            return Result.Success(MagicStrings.SuccessMessages.EmailSentSuccessfully);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send generic email to: {Email}", toEmail);
            return Result.Failure<string>(Error.Failure(MagicStrings.ErrorCodes.EmailSendingFailed, ex.Message));
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
            var fromEmail = _configuration[MagicStrings.ConfigKeys.EmailSenderEmail];
            var appPassword = _configuration[MagicStrings.ConfigKeys.EmailPassword];

            if (string.IsNullOrEmpty(fromEmail) || string.IsNullOrEmpty(appPassword))
            {
                _logger.LogError(MagicStrings.LogMessages.SmtpConnectionFailed, "Email configuration missing");
                return Result.Failure<string>(
                    Error.Failure(MagicStrings.ErrorCodes.SmtpConnectionFailed, "Email configuration missing")
                );
            }

            using var client = new System.Net.Mail.SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(fromEmail, appPassword)
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail, _configuration[MagicStrings.ConfigKeys.EmailSenderName] ?? "RouteBuddy"),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            if (pdfAttachment != null)
            {
                var attachment = new Attachment(
                    new MemoryStream(pdfAttachment),
                    MagicStrings.EmailAttachments.TicketFileName,
                    MagicStrings.EmailAttachments.PdfContentType
                );
                mailMessage.Attachments.Add(attachment);
            }

            await client.SendMailAsync(mailMessage);
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

    private async Task<string> GenerateConnectingEmailHtmlAsync(ConnectingBookingEmailData bookingData)
    {
        try
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Kanini.RouteBuddy.Application.ConnectingBookingConfirmationEmail.html";

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
            _logger.LogError(ex, "Failed to load connecting email template: {Error}", ex.Message);
            return GenerateFallbackConnectingEmailHtml(bookingData);
        }
    }

    private static string PopulateConnectingEmailTemplate(string template, ConnectingBookingEmailData bookingData)
    {
        var customerName = $"{bookingData.FirstName} {bookingData.LastName}".Trim();
        var segmentRows = string.Join("", bookingData.Segments.Select(s =>
            $"<tr><td>{s.SegmentOrder}</td><td>{s.BusName}</td><td>{s.Source} → {s.Destination}</td><td>{s.DepartureTime:HH:mm} - {s.ArrivalTime:HH:mm}</td><td>{string.Join(", ", s.SeatNumbers)}</td><td>₹{s.SegmentAmount:F2}</td></tr>"
        ));

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

    private static string GenerateFallbackConnectingEmailHtml(ConnectingBookingEmailData bookingData)
    {
        var customerName = $"{bookingData.FirstName} {bookingData.LastName}".Trim();
        var segmentDetails = string.Join("<br/>", bookingData.Segments.Select(s =>
            $"Segment {s.SegmentOrder}: {s.BusName} ({s.Source} → {s.Destination}) - Seats: {string.Join(", ", s.SeatNumbers)}"
        ));

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
        <p>Thank you for choosing RouteBuddy!</p>
        </body></html>";
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
