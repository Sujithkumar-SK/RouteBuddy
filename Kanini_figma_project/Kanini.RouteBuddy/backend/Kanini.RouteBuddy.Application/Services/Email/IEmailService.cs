using Kanini.RouteBuddy.Common.Utility;

namespace Kanini.RouteBuddy.Application.Services.Email;

public interface IEmailService
{
    Task<Result<string>> SendBookingConfirmationAsync(int bookingId);
    Task<Result<string>> SendConnectingBookingConfirmationAsync(int bookingId);
    Task<Result<string>> SendVendorRejectionEmailAsync(string vendorEmail, string vendorName, string rejectionReason);
    Task<Result<string>> SendVendorApprovalEmailAsync(string vendorEmail, string vendorName);
    Task<Result<string>> SendBusNotificationEmailAsync(string vendorEmail, string vendorName, string message);
    Task<Result<string>> SendGenericEmailAsync(string toEmail, string subject, string htmlBody);
}
