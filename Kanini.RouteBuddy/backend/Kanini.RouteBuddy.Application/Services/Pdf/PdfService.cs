using Kanini.RouteBuddy.Common;
using Kanini.RouteBuddy.Common.Utility;
using Kanini.RouteBuddy.Data.Repositories.Email;
using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Kanini.RouteBuddy.Application.Services.Pdf;

public class PdfService : IPdfService
{
    private readonly ILogger<PdfService> _logger;

    public PdfService(ILogger<PdfService> logger)
    {
        _logger = logger;
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public async Task<Result<byte[]>> GenerateBookingTicketAsync(BookingEmailData bookingData)
    {
        try
        {
            _logger.LogInformation(
                MagicStrings.LogMessages.PdfGenerationStarted,
                bookingData.BookingId
            );

            var pdfBytes = await Task.Run(() =>
                Document
                    .Create(container =>
                    {
                        container.Page(page =>
                        {
                            page.Size(PageSizes.A4);
                            page.Margin(2, Unit.Centimetre);
                            page.DefaultTextStyle(x => x.FontSize(10));

                            page.Header()
                                .Text("ROUTEBUDDY E-TICKET")
                                .SemiBold()
                                .FontSize(20)
                                .FontColor(Colors.Blue.Medium)
                                .AlignCenter();

                            page.Content()
                                .PaddingVertical(1, Unit.Centimetre)
                                .Column(column =>
                                {
                                    column
                                        .Item()
                                        .Row(row =>
                                        {
                                            row.RelativeItem()
                                                .Text($"PNR: {bookingData.PNRNo}")
                                                .SemiBold()
                                                .FontSize(14);
                                            row.RelativeItem()
                                                .Text(
                                                    $"Booking Date: {bookingData.BookedAt:dd-MMM-yyyy}"
                                                )
                                                .AlignRight();
                                        });

                                    column
                                        .Item()
                                        .PaddingTop(0.5f, Unit.Centimetre)
                                        .LineHorizontal(1);

                                    // Journey Details
                                    column
                                        .Item()
                                        .PaddingTop(0.5f, Unit.Centimetre)
                                        .Text("JOURNEY DETAILS")
                                        .SemiBold()
                                        .FontSize(12);

                                    column
                                        .Item()
                                        .PaddingTop(0.2f, Unit.Centimetre)
                                        .Row(row =>
                                        {
                                            row.RelativeItem().Text($"From: {bookingData.Source}");
                                            row.RelativeItem()
                                                .Text($"To: {bookingData.Destination}")
                                                .AlignRight();
                                        });

                                    column
                                        .Item()
                                        .Row(row =>
                                        {
                                            row.RelativeItem()
                                                .Text(
                                                    $"Travel Date: {bookingData.TravelDate:dd-MMM-yyyy}"
                                                );
                                            row.RelativeItem()
                                                .Text($"Bus: {bookingData.BusName}")
                                                .AlignRight();
                                        });

                                    column
                                        .Item()
                                        .Row(row =>
                                        {
                                            row.RelativeItem()
                                                .Text(
                                                    $"Departure: {bookingData.DepartureTime:hh\\:mm}"
                                                );
                                            row.RelativeItem()
                                                .Text($"Arrival: {bookingData.ArrivalTime:hh\\:mm}")
                                                .AlignRight();
                                        });

                                    if (!string.IsNullOrEmpty(bookingData.BoardingStopName))
                                    {
                                        column
                                            .Item()
                                            .Row(row =>
                                            {
                                                row.RelativeItem()
                                                    .Text(
                                                        $"Boarding: {bookingData.BoardingStopName}"
                                                    );
                                                row.RelativeItem()
                                                    .Text(
                                                        $"Dropping: {bookingData.DroppingStopName}"
                                                    )
                                                    .AlignRight();
                                            });
                                    }

                                    column
                                        .Item()
                                        .PaddingTop(0.5f, Unit.Centimetre)
                                        .LineHorizontal(1);

                                    // Passenger Details
                                    column
                                        .Item()
                                        .PaddingTop(0.5f, Unit.Centimetre)
                                        .Text("PASSENGER DETAILS")
                                        .SemiBold()
                                        .FontSize(12);

                                    column
                                        .Item()
                                        .PaddingTop(0.2f, Unit.Centimetre)
                                        .Table(table =>
                                        {
                                            table.ColumnsDefinition(columns =>
                                            {
                                                columns.RelativeColumn(2);
                                                columns.RelativeColumn(1);
                                                columns.RelativeColumn(1);
                                                columns.RelativeColumn(1);
                                            });

                                            table.Header(header =>
                                            {
                                                header
                                                    .Cell()
                                                    .Element(CellStyle)
                                                    .Text("Passenger Name")
                                                    .SemiBold();
                                                header
                                                    .Cell()
                                                    .Element(CellStyle)
                                                    .Text("Age")
                                                    .SemiBold();
                                                header
                                                    .Cell()
                                                    .Element(CellStyle)
                                                    .Text("Gender")
                                                    .SemiBold();
                                                header
                                                    .Cell()
                                                    .Element(CellStyle)
                                                    .Text("Seat")
                                                    .SemiBold();
                                            });

                                            foreach (var passenger in bookingData.Passengers)
                                            {
                                                table
                                                    .Cell()
                                                    .Element(CellStyle)
                                                    .Text(passenger.PassengerName);
                                                table
                                                    .Cell()
                                                    .Element(CellStyle)
                                                    .Text(passenger.PassengerAge.ToString());
                                                table
                                                    .Cell()
                                                    .Element(CellStyle)
                                                    .Text(GetGenderText(passenger.PassengerGender));
                                                table
                                                    .Cell()
                                                    .Element(CellStyle)
                                                    .Text(passenger.SeatNumber);
                                            }
                                        });

                                    column
                                        .Item()
                                        .PaddingTop(0.5f, Unit.Centimetre)
                                        .LineHorizontal(1);

                                    // Payment Details
                                    column
                                        .Item()
                                        .PaddingTop(0.5f, Unit.Centimetre)
                                        .Text("PAYMENT DETAILS")
                                        .SemiBold()
                                        .FontSize(12);

                                    column
                                        .Item()
                                        .PaddingTop(0.2f, Unit.Centimetre)
                                        .Row(row =>
                                        {
                                            row.RelativeItem()
                                                .Text(
                                                    $"Total Amount: ₹{bookingData.TotalAmount:F2}"
                                                )
                                                .SemiBold();
                                            row.RelativeItem()
                                                .Text("Status: Confirmed")
                                                .FontColor(Colors.Green.Medium)
                                                .AlignRight();
                                        });

                                    if (!string.IsNullOrEmpty(bookingData.TransactionId))
                                    {
                                        column
                                            .Item()
                                            .Text($"Transaction ID: {bookingData.TransactionId}");
                                    }

                                    column
                                        .Item()
                                        .PaddingTop(0.5f, Unit.Centimetre)
                                        .LineHorizontal(1);

                                    column
                                        .Item()
                                        .PaddingTop(0.5f, Unit.Centimetre)
                                        .AlignCenter()
                                        .Width(100)
                                        .Height(100)
                                        .Background(Colors.Grey.Lighten3)
                                        .AlignCenter()
                                        .AlignMiddle()
                                        .Text($"QR CODE\n{bookingData.PNRNo}")
                                        .FontSize(8)
                                        .AlignCenter();
                                });

                            page.Footer()
                                .AlignCenter()
                                .Text("Thank you for choosing RouteBuddy! Have a safe journey.")
                                .FontSize(8);
                        });
                    })
                    .GeneratePdf()
            );

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

    private static IContainer CellStyle(IContainer container)
    {
        return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
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
