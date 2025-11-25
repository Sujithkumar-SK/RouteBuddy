using Kanini.RouteBuddy.Common.Utility;

namespace Kanini.RouteBuddy.Application.Services.User;

public interface IUserService
{
    Task<Result<string>> SendOtpAsync(string email);
    Task<Result<string>> VerifyOtpAsync(string email, string otp);
    Task<Result<string>> ResendOtpAsync(string email);
    Task<Result<string>> SendForgotPasswordOtpAsync(string email);
    Task<Result<string>> ResendForgotPasswordOtpAsync(string email);
    Task<Result<string>> ResetPasswordAsync(string email, string otp, string newPassword);
}