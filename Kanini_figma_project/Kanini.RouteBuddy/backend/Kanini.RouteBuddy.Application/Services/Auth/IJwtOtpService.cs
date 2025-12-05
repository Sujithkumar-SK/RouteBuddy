using Kanini.RouteBuddy.Application.Dto.Auth;

namespace Kanini.RouteBuddy.Application.Services.Auth;

public interface IJwtOtpService
{
    string GenerateOtp();
    string CreateOtpToken(string email, string otp, string otpType);
    string CreateOtpTokenWithData(string email, string otp, string otpType, RegistrationDataDto data);
    bool ValidateOtp(string jwtToken, string inputOtp, string email, string otpType);
    RegistrationDataDto? GetRegistrationDataFromToken(string jwtToken);
}
