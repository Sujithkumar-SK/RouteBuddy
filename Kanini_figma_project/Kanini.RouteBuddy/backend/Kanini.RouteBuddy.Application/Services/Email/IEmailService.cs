using Kanini.RouteBuddy.Common.Utility;

namespace Kanini.RouteBuddy.Application.Services.Email;

public interface IEmailService
{
    Task<Result<string>> SendBookingConfirmationAsync(int bookingId);
    Task<Result<string>> SendConnectingBookingConfirmationAsync(int bookingId);
}
