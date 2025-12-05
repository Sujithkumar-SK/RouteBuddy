using Kanini.RouteBuddy.Application.Dto;
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
    
    // Admin user management methods
    Task<IEnumerable<UserResponseDto>> GetAllUsersAsync(int pageNumber, int pageSize);
    Task<IEnumerable<UserResponseDto>> FilterUsersAsync(string? searchTerm, string? role, bool? isActive);
    Task<UserResponseDto?> GetUserByIdAsync(int userId);
    Task<bool> ActivateUserAsync(int userId);
    Task<bool> DeactivateUserAsync(int userId);
}