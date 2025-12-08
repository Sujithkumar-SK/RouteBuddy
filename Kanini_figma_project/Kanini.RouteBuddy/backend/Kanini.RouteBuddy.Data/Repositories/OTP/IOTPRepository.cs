namespace Kanini.RouteBuddy.Data.Repositories.OTP;

public interface IOTPRepository
{
    Task StoreOTPAsync(string email, string otp, DateTime expiryTime);
    Task<bool> ValidateOTPAsync(string email, string otp);
    Task DeleteOTPAsync(string email);
}