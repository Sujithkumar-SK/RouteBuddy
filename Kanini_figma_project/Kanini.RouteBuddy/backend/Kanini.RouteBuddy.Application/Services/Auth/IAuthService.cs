using Kanini.RouteBuddy.Application.Dto.Auth;
using Kanini.RouteBuddy.Common.Utility;

namespace Kanini.RouteBuddy.Application.Services.Auth;

public interface IAuthService
{
    Task<Result<OtpResponseDto>> RegisterWithOtpAsync(RegisterWithOtpRequestDto request);
    Task<Result<OtpResponseDto>> ResendRegistrationOtpAsync(ResendOtpRequestDto request);
    Task<Result<RegistrationResponseDto>> VerifyRegistrationOtpAsync(VerifyRegistrationOtpRequestDto request);
    Task<Result<LoginResponseDto>> LoginAsync(LoginRequestDto request);
    Task<Result<LoginResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request);
    Task<Result> LogoutAsync(string refreshToken, int userId);
    Task<Result> ChangePasswordAsync(int userId, ChangePasswordRequestDto request);
    Task<Result<OtpResponseDto>> ForgotPasswordAsync(ForgotPasswordDto request);
    Task<Result> VerifyForgotPasswordOtpAsync(VerifyForgotPasswordOtpRequestDto request);
    Task<Result> ResetPasswordAsync(ResetPasswordDto request);
    Task<Result> CompleteCustomerProfileAsync(int userId, CompleteCustomerProfileDto request);
    Task<Result> CompleteVendorProfileAsync(int userId, CompleteVendorProfileDto request);
}
