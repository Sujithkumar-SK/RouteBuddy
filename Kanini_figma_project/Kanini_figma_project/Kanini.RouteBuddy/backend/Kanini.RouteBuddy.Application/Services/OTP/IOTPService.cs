namespace Kanini.RouteBuddy.Application.Services.OTP;

public interface IOTPService
{
    Task<string> GenerateAndStoreOTPAsync(string email);
    Task<bool> ValidateOTPAsync(string email, string otp);
    Task ClearOTPAsync(string email);
}