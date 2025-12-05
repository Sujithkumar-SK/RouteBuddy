using Kanini.RouteBuddy.Common.Utility;

namespace Kanini.RouteBuddy.Application.Services.Email;

public interface ISmartEmailService
{
    Task<Result<string>> SendConnectingBookingConfirmationAsync(int bookingId);
}
