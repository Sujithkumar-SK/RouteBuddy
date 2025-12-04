using Kanini.RouteBuddy.Common.Utility;
using Kanini.RouteBuddy.Data.Repositories.Email;

namespace Kanini.RouteBuddy.Application.Services.Pdf;

public interface IPdfService
{
    Task<Result<byte[]>> GenerateBookingTicketAsync(BookingEmailData bookingData);
    Task<Result<byte[]>> GenerateConnectingBookingTicketAsync(ConnectingBookingEmailData bookingData);
}
