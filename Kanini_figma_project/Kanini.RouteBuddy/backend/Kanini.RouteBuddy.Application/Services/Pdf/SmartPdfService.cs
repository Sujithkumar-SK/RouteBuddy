using Kanini.RouteBuddy.Common;
using Kanini.RouteBuddy.Common.Utility;
using Kanini.RouteBuddy.Data.Repositories.Email;
using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Kanini.RouteBuddy.Application.Services.Pdf;

public class SmartPdfService : ISmartPdfService
{
    private readonly ILogger<SmartPdfService> _logger;

    public SmartPdfService(ILogger<SmartPdfService> logger)
    {
        _logger = logger;
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public async Task<Result<byte[]>> GenerateConnectingBookingTicketAsync(
        ConnectingBookingEmailData bookingData
    )
    {
        try
        {
            _logger.LogInformation(
                MagicStrings.LogMessages.SmartPdfGenerationStarted,
                bookingData.BookingId
            );

            var pdfBytes = await Task.Run(() => GenerateConnectingTicketPdf(bookingData));

            _logger.LogInformation(
                MagicStrings.LogMessages.SmartPdfGenerationCompleted,
                bookingData.BookingId
            );

            return Result.Success(pdfBytes);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                MagicStrings.LogMessages.SmartPdfGenerationFailed,
                bookingData.BookingId,
                ex.Message
            );
            return Result.Failure<byte[]>(
                Error.Failure(MagicStrings.ErrorCodes.SmartPdfGenerationFailed, ex.Message)
            );
        }
    }

    private byte[] GenerateConnectingTicketPdf(ConnectingBookingEmailData bookingData)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header()
                    .Text("RouteBuddy - Connecting Route Ticket")
                    .SemiBold()
                    .FontSize(20)
                    .FontColor(Colors.Blue.Medium);

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(x =>
                    {
                        x.Spacing(20);

                        // Booking Information
                        x.Item().Element(BookingInfoSection);

                        // Journey Overview
                        x.Item().Element(JourneyOverviewSection);

                        // Segments Details
                        x.Item().Element(SegmentsSection);

                        // Payment Information
                        x.Item().Element(PaymentSection);

                        // Important Notes
                        x.Item().Element(ImportantNotesSection);
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Generated on: ");
                        x.Span($"{DateTime.Now:dd MMM yyyy HH:mm}").SemiBold();
                        x.Span(" | Thank you for choosing RouteBuddy!");
                    });
            });
        });

        return document.GeneratePdf();

        void BookingInfoSection(IContainer container)
        {
            container
                .Border(1)
                .Padding(10)
                .Column(column =>
                {
                    column.Item().Text("Booking Information").FontSize(14).SemiBold();
                    column
                        .Item()
                        .PaddingTop(5)
                        .Row(row =>
                        {
                            row.RelativeItem()
                                .Column(col =>
                                {
                                    col.Item().Text($"PNR: {bookingData.PNRNo}").SemiBold();
                                    col.Item()
                                        .Text(
                                            $"Customer: {bookingData.FirstName} {bookingData.LastName}"
                                        );
                                    col.Item().Text($"Phone: {bookingData.CustomerPhone}");
                                });
                            row.RelativeItem()
                                .Column(col =>
                                {
                                    col.Item()
                                        .Text($"Travel Date: {bookingData.TravelDate:dd MMM yyyy}")
                                        .SemiBold();
                                    col.Item()
                                        .Text(
                                            $"Booked On: {bookingData.BookedAt:dd MMM yyyy HH:mm}"
                                        );
                                    col.Item()
                                        .Text($"Total Amount: ₹{bookingData.TotalAmount:F2}")
                                        .SemiBold();
                                });
                        });
                });
        }

        void JourneyOverviewSection(IContainer container)
        {
            container
                .Border(1)
                .Padding(10)
                .Column(column =>
                {
                    column.Item().Text("Journey Overview").FontSize(14).SemiBold();
                    column
                        .Item()
                        .PaddingTop(5)
                        .Row(row =>
                        {
                            row.RelativeItem()
                                .Text($"From: {bookingData.OverallSource}")
                                .SemiBold();
                            row.RelativeItem().AlignCenter().Text("→").FontSize(16);
                            row.RelativeItem()
                                .AlignRight()
                                .Text($"To: {bookingData.OverallDestination}")
                                .SemiBold();
                        });
                    column
                        .Item()
                        .PaddingTop(5)
                        .Text($"Total Segments: {bookingData.Segments.Count}")
                        .SemiBold();
                });
        }

        void SegmentsSection(IContainer container)
        {
            container
                .Border(1)
                .Padding(10)
                .Column(column =>
                {
                    column.Item().Text("Segment Details").FontSize(14).SemiBold();
                    column
                        .Item()
                        .PaddingTop(10)
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(40); // Segment
                                columns.RelativeColumn(2); // Bus
                                columns.RelativeColumn(2); // Route
                                columns.RelativeColumn(1); // Time
                                columns.RelativeColumn(1); // Seats
                                columns.RelativeColumn(1); // Amount
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("Seg").SemiBold();
                                header.Cell().Element(CellStyle).Text("Bus Name").SemiBold();
                                header.Cell().Element(CellStyle).Text("Route").SemiBold();
                                header.Cell().Element(CellStyle).Text("Time").SemiBold();
                                header.Cell().Element(CellStyle).Text("Seats").SemiBold();
                                header.Cell().Element(CellStyle).Text("Amount").SemiBold();
                            });

                            foreach (
                                var segment in bookingData.Segments.OrderBy(s => s.SegmentOrder)
                            )
                            {
                                table
                                    .Cell()
                                    .Element(CellStyle)
                                    .Text(segment.SegmentOrder.ToString());
                                table.Cell().Element(CellStyle).Text(segment.BusName);
                                table
                                    .Cell()
                                    .Element(CellStyle)
                                    .Text($"{segment.Source} → {segment.Destination}");
                                table
                                    .Cell()
                                    .Element(CellStyle)
                                    .Text(
                                        $"{segment.DepartureTime:HH:mm}-{segment.ArrivalTime:HH:mm}"
                                    );
                                table
                                    .Cell()
                                    .Element(CellStyle)
                                    .Text(string.Join(", ", segment.SeatNumbers));
                                table
                                    .Cell()
                                    .Element(CellStyle)
                                    .Text($"₹{segment.SegmentAmount:F2}");
                            }
                        });
                });
        }

        void PaymentSection(IContainer container)
        {
            container
                .Border(1)
                .Padding(10)
                .Column(column =>
                {
                    column.Item().Text("Payment Information").FontSize(14).SemiBold();
                    column
                        .Item()
                        .PaddingTop(5)
                        .Row(row =>
                        {
                            row.RelativeItem()
                                .Text(
                                    $"Payment Method: {GetPaymentMethodText(bookingData.PaymentMethod)}"
                                );
                            row.RelativeItem().Text($"Transaction ID: {bookingData.TransactionId}");
                            row.RelativeItem()
                                .Text($"Payment Date: {bookingData.PaymentDate:dd MMM yyyy HH:mm}");
                        });
                });
        }

        void ImportantNotesSection(IContainer container)
        {
            container
                .Border(1)
                .Padding(10)
                .Column(column =>
                {
                    column.Item().Text("Important Notes").FontSize(14).SemiBold();
                    column
                        .Item()
                        .PaddingTop(5)
                        .Column(notes =>
                        {
                            notes.Item().Text("• Please carry a valid ID proof during travel");
                            notes
                                .Item()
                                .Text("• Report to boarding point 15 minutes before departure");
                            notes
                                .Item()
                                .Text(
                                    "• This is a connecting route ticket - ensure you board the correct bus for each segment"
                                );
                            notes.Item().Text("• Keep this ticket safe throughout your journey");
                            notes.Item().Text("• For support, contact: support@routebuddy.com");
                        });
                });
        }

        static IContainer CellStyle(IContainer container)
        {
            return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
        }
    }

    private static string GetPaymentMethodText(int paymentMethod)
    {
        return paymentMethod switch
        {
            1 => "Mock Payment",
            2 => "UPI",
            3 => "Credit/Debit Card",
            4 => "Net Banking",
            _ => "Unknown",
        };
    }
}
