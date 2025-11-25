using Kanini.RouteBuddy.Common;
using Kanini.RouteBuddy.Common.Utility;
using Kanini.RouteBuddy.Data.Repositories.Email;
using Microsoft.Extensions.Logging;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

namespace Kanini.RouteBuddy.Application.Services.Pdf;

public class PdfService : IPdfService
{
    private readonly ILogger<PdfService> _logger;

    public PdfService(ILogger<PdfService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<byte[]>> GenerateBookingTicketAsync(BookingEmailData bookingData)
    {
        try
        {
            _logger.LogInformation(
                MagicStrings.LogMessages.PdfGenerationStarted,
                bookingData.BookingId
            );

            var pdfBytes = await Task.Run(() => GenerateSimplePdfTicket(bookingData));

            _logger.LogInformation(
                MagicStrings.LogMessages.PdfGenerationCompleted,
                bookingData.BookingId
            );
            return Result.Success(pdfBytes);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                MagicStrings.LogMessages.PdfGenerationFailed,
                bookingData.BookingId,
                ex.Message
            );
            return Result.Failure<byte[]>(
                Error.Failure(MagicStrings.ErrorCodes.PdfGenerationFailed, ex.Message)
            );
        }
    }

    private byte[] GenerateSimplePdfTicket(BookingEmailData bookingData)
    {
        // Simple text-based PDF generation
        var content = new StringBuilder();
        content.AppendLine("=== ROUTEBUDDY E-TICKET ===");
        content.AppendLine($"PNR: {bookingData.PNRNo}");
        content.AppendLine($"Booking Date: {bookingData.BookedAt:dd-MMM-yyyy}");
        content.AppendLine();
        content.AppendLine("JOURNEY DETAILS:");
        content.AppendLine($"From: {bookingData.Source}");
        content.AppendLine($"To: {bookingData.Destination}");
        content.AppendLine($"Travel Date: {bookingData.TravelDate:dd-MMM-yyyy}");
        content.AppendLine($"Bus: {bookingData.BusName}");
        content.AppendLine($"Departure: {bookingData.DepartureTime:hh\\:mm}");
        content.AppendLine($"Arrival: {bookingData.ArrivalTime:hh\\:mm}");
        
        if (!string.IsNullOrEmpty(bookingData.BoardingStopName))
        {
            content.AppendLine($"Boarding: {bookingData.BoardingStopName}");
            content.AppendLine($"Dropping: {bookingData.DroppingStopName}");
        }
        
        content.AppendLine();
        content.AppendLine("PASSENGER DETAILS:");
        foreach (var passenger in bookingData.Passengers)
        {
            content.AppendLine($"{passenger.PassengerName} | Age: {passenger.PassengerAge} | Gender: {GetGenderText(passenger.PassengerGender)} | Seat: {passenger.SeatNumber}");
        }
        
        content.AppendLine();
        content.AppendLine("PAYMENT DETAILS:");
        content.AppendLine($"Total Amount: ₹{bookingData.TotalAmount:F2}");
        content.AppendLine("Status: Confirmed");
        
        if (!string.IsNullOrEmpty(bookingData.TransactionId))
        {
            content.AppendLine($"Transaction ID: {bookingData.TransactionId}");
        }
        
        content.AppendLine();
        content.AppendLine("Thank you for choosing RouteBuddy! Have a safe journey.");
        
        // Convert to bytes (simple text-based approach)
        return Encoding.UTF8.GetBytes(content.ToString());
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
