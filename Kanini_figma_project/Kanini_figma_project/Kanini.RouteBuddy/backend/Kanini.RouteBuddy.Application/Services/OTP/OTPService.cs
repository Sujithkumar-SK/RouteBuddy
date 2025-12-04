using Kanini.RouteBuddy.Common;
using Kanini.RouteBuddy.Data.Repositories.OTP;
using Microsoft.Extensions.Logging;

namespace Kanini.RouteBuddy.Application.Services.OTP;

public class OTPService : IOTPService
{
    private readonly IOTPRepository _otpRepository;
    private readonly ILogger<OTPService> _logger;

    public OTPService(IOTPRepository otpRepository, ILogger<OTPService> logger)
    {
        _otpRepository = otpRepository;
        _logger = logger;
    }

    public async Task<string> GenerateAndStoreOTPAsync(string email)
    {
        try
        {
            var otp = GenerateOTP();
            var expiryTime = DateTime.UtcNow.AddMinutes(10); // 10 minutes expiry

            await _otpRepository.StoreOTPAsync(email, otp, expiryTime);
            _logger.LogInformation("OTP generated and stored for email: {Email}", email);
            Console.WriteLine(
                $"\n========== OTP GENERATED ==========\nEmail: {email}\nOTP: {otp}\nExpiry: {expiryTime}\n==================================\n"
            );

            return otp;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating OTP for email: {Email}", email);
            throw new Exception(MagicStrings.ErrorMessages.InternalServerError);
        }
    }

    public async Task<bool> ValidateOTPAsync(string email, string otp)
    {
        try
        {
            return await _otpRepository.ValidateOTPAsync(email, otp);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating OTP for email: {Email}", email);
            throw new Exception(MagicStrings.ErrorMessages.InternalServerError);
        }
    }

    public async Task ClearOTPAsync(string email)
    {
        try
        {
            await _otpRepository.DeleteOTPAsync(email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing OTP for email: {Email}", email);
            throw new Exception(MagicStrings.ErrorMessages.InternalServerError);
        }
    }

    private static string GenerateOTP()
    {
        var random = new Random();
        return random.Next(100000, 999999).ToString();
    }
}
