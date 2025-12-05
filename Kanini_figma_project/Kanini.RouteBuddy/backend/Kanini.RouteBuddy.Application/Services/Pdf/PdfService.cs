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
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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

            var pdfBytes = await Task.Run(() => GenerateSimpleTicketPdf(bookingData));

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

    public async Task<Result<byte[]>> GenerateConnectingBookingTicketAsync(ConnectingBookingEmailData bookingData)
    {
        try
        {
            _logger.LogInformation(
                MagicStrings.LogMessages.PdfGenerationStarted,
                bookingData.BookingId
            );

            var pdfBytes = await Task.Run(() => GenerateConnectingTicketPdf(bookingData));

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

    private byte[] GenerateSimpleTicketPdf(BookingEmailData bookingData)
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
                    .Text("RouteBuddy - E-Ticket")
                    .SemiBold()
                    .FontSize(20)
                    .FontColor(Colors.Blue.Medium);

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(x =>
                    {
                        x.Spacing(20);

                        x.Item().Element(BookingInfoSection);
                        x.Item().Element(JourneyDetailsSection);
                        x.Item().Element(PassengerDetailsSection);
                        x.Item().Element(PaymentDetailsSection);
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
                                    col.Item().Text($"Customer: {bookingData.FirstName} {bookingData.LastName}");
                                    col.Item().Text($"Phone: {bookingData.CustomerPhone}");
                                });
                            row.RelativeItem()
                                .Column(col =>
                                {
                                    col.Item().Text($"Travel Date: {bookingData.TravelDate:dd MMM yyyy}").SemiBold();
                                    col.Item().Text($"Booked On: {bookingData.BookedAt:dd MMM yyyy HH:mm}");
                                    col.Item().Text($"Total Amount: ₹{bookingData.TotalAmount:F2}").SemiBold();
                                });
                        });
                });
        }

        void JourneyDetailsSection(IContainer container)
        {
            container
                .Border(1)
                .Padding(10)
                .Column(column =>
                {
                    column.Item().Text("Journey Details").FontSize(14).SemiBold();
                    column
                        .Item()
                        .PaddingTop(5)
                        .Column(col =>
                        {
                            col.Item().Row(row =>
                            {
                                row.RelativeItem().Text($"From: {bookingData.Source}").SemiBold();
                                row.RelativeItem().AlignCenter().Text("→").FontSize(16);
                                row.RelativeItem().AlignRight().Text($"To: {bookingData.Destination}").SemiBold();
                            });
                            col.Item().PaddingTop(5).Text($"Bus: {bookingData.BusName}");
                            col.Item().Text($"Departure: {bookingData.DepartureTime:hh\\:mm} | Arrival: {bookingData.ArrivalTime:hh\\:mm}");
                            if (!string.IsNullOrEmpty(bookingData.BoardingStopName))
                            {
                                col.Item().Text($"Boarding: {bookingData.BoardingStopName}");
                                col.Item().Text($"Dropping: {bookingData.DroppingStopName}");
                            }
                        });
                });
        }

        void PassengerDetailsSection(IContainer container)
        {
            container
                .Border(1)
                .Padding(10)
                .Column(column =>
                {
                    column.Item().Text("Passenger Details").FontSize(14).SemiBold();
                    column
                        .Item()
                        .PaddingTop(10)
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("Name").SemiBold();
                                header.Cell().Element(CellStyle).Text("Age").SemiBold();
                                header.Cell().Element(CellStyle).Text("Gender").SemiBold();
                                header.Cell().Element(CellStyle).Text("Seat").SemiBold();
                            });

                            foreach (var passenger in bookingData.Passengers)
                            {
                                table.Cell().Element(CellStyle).Text(passenger.PassengerName);
                                table.Cell().Element(CellStyle).Text(passenger.PassengerAge.ToString());
                                table.Cell().Element(CellStyle).Text(GetGenderText(passenger.PassengerGender));
                                table.Cell().Element(CellStyle).Text(passenger.SeatNumber);
                            }
                        });
                });
        }

        void PaymentDetailsSection(IContainer container)
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
                            row.RelativeItem().Text($"Total Amount: ₹{bookingData.TotalAmount:F2}").SemiBold();
                            row.RelativeItem().Text("Status: Confirmed").SemiBold();
                            if (!string.IsNullOrEmpty(bookingData.TransactionId))
                            {
                                row.RelativeItem().Text($"Transaction ID: {bookingData.TransactionId}");
                            }
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
                            notes.Item().Text("• Report to boarding point 15 minutes before departure");
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

                        x.Item().Element(BookingInfoSection);
                        x.Item().Element(JourneyOverviewSection);
                        x.Item().Element(SegmentsSection);
                        x.Item().Element(PaymentSection);
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
                                    col.Item().Text($"Customer: {bookingData.FirstName} {bookingData.LastName}");
                                    col.Item().Text($"Phone: {bookingData.CustomerPhone}");
                                });
                            row.RelativeItem()
                                .Column(col =>
                                {
                                    col.Item().Text($"Travel Date: {bookingData.TravelDate:dd MMM yyyy}").SemiBold();
                                    col.Item().Text($"Booked On: {bookingData.BookedAt:dd MMM yyyy HH:mm}");
                                    col.Item().Text($"Total Amount: ₹{bookingData.TotalAmount:F2}").SemiBold();
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
                            row.RelativeItem().Text($"From: {bookingData.OverallSource}").SemiBold();
                            row.RelativeItem().AlignCenter().Text("→").FontSize(16);
                            row.RelativeItem().AlignRight().Text($"To: {bookingData.OverallDestination}").SemiBold();
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
                                columns.ConstantColumn(40);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
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

                            foreach (var segment in bookingData.Segments.OrderBy(s => s.SegmentOrder))
                            {
                                table.Cell().Element(CellStyle).Text(segment.SegmentOrder.ToString());
                                table.Cell().Element(CellStyle).Text(segment.BusName);
                                table.Cell().Element(CellStyle).Text($"{segment.Source} → {segment.Destination}");
                                table.Cell().Element(CellStyle).Text($"{segment.DepartureTime:hh\\:mm}-{segment.ArrivalTime:hh\\:mm}");
                                table.Cell().Element(CellStyle).Text(string.Join(", ", segment.SeatNumbers));
                                table.Cell().Element(CellStyle).Text($"₹{segment.SegmentAmount:F2}");
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
                            row.RelativeItem().Text($"Payment Method: {GetPaymentMethodText(bookingData.PaymentMethod)}");
                            row.RelativeItem().Text($"Transaction ID: {bookingData.TransactionId}");
                            row.RelativeItem().Text($"Payment Date: {bookingData.PaymentDate:dd MMM yyyy HH:mm}");
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
                            notes.Item().Text("• Report to boarding point 15 minutes before departure");
                            notes.Item().Text("• This is a connecting route ticket - ensure you board the correct bus for each segment");
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
