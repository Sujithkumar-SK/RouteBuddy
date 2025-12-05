using AutoMapper;
using BCrypt.Net;
using Kanini.RouteBuddy.Application.Dto;
using Kanini.RouteBuddy.Application.Services.Email;
using Kanini.RouteBuddy.Common;
using Kanini.RouteBuddy.Common.Utility;
using Kanini.RouteBuddy.Data.Repositories.User;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Kanini.RouteBuddy.Application.Services.User;

public class UserService : IUserService
{
    private readonly IMemoryCache _cache;
    private readonly IEmailService _emailService;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UserService> _logger;

    public UserService(
        IMemoryCache cache,
        IEmailService emailService,
        IUserRepository userRepository,
        IMapper mapper,
        ILogger<UserService> logger)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<string>> SendOtpAsync(string email)
    {
        try
        {
            _logger.LogInformation("Sending OTP to email: {Email}", email);

            if (string.IsNullOrWhiteSpace(email))
            {
                _logger.LogWarning("Email is required for OTP");
                return Result.Failure<string>(
                    Error.Failure("Email.Required", MagicStrings.ErrorMessages.EmailRequired)
                );
            }

            var existing = await _userRepository.ExistsByEmailAsync(email);
            if (existing)
            {
                _logger.LogWarning("Email already exists: {Email}", email);
                return Result.Failure<string>(
                    Error.Failure("Email.AlreadyExists", MagicStrings.ErrorMessages.EmailAlreadyExists)
                );
            }

            string otp = new Random().Next(100000, 999999).ToString();
            _cache.Set($"otp:{email}", otp, TimeSpan.FromMinutes(10));
            _cache.Set($"otp_sent:{email}", DateTime.UtcNow, TimeSpan.FromSeconds(120));

            string customerName = email.Split('@')[0];
            string emailBody = MagicStrings.EmailTemplates.GetVerificationCodeEmail(customerName, otp);
            
            var emailResult = await SendSimpleEmailAsync(email, "RouteBuddy Email Verification", emailBody);
            if (emailResult.IsFailure)
            {
                _logger.LogError("Failed to send OTP email to {Email}", email);
                return Result.Failure<string>(emailResult.Error);
            }

            _logger.LogInformation("OTP sent successfully to {Email}", email);
            return Result.Success(MagicStrings.SuccessMessages.OtpSent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending OTP to {Email}", email);
            return Result.Failure<string>(
                Error.Failure("OTP.SendFailed", MagicStrings.ErrorMessages.InternalServerError)
            );
        }
    }

    public Task<Result<string>> VerifyOtpAsync(string email, string otp)
    {
        try
        {
            _logger.LogInformation("Verifying OTP for email: {Email}", email);

            if (!_cache.TryGetValue($"otp:{email}", out string? cachedOtp) || cachedOtp != otp)
            {
                _logger.LogWarning("Invalid or expired OTP for {Email}", email);
                return Task.FromResult(Result.Failure<string>(
                    Error.Failure("OTP.Invalid", MagicStrings.ErrorMessages.InvalidOrExpiredOtp)
                ));
            }

            _cache.Set(email, email, TimeSpan.FromMinutes(15));
            _cache.Remove($"otp:{email}");

            _logger.LogInformation("OTP verified successfully for {Email}", email);
            return Task.FromResult(Result.Success(MagicStrings.SuccessMessages.EmailVerified));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying OTP for {Email}", email);
            return Task.FromResult(Result.Failure<string>(
                Error.Failure("OTP.VerifyFailed", MagicStrings.ErrorMessages.InternalServerError)
            ));
        }
    }

    public async Task<Result<string>> ResendOtpAsync(string email)
    {
        try
        {
            _logger.LogInformation("Resending OTP to email: {Email}", email);

            if (_cache.TryGetValue($"otp_sent:{email}", out _))
            {
                _logger.LogWarning("Resend not allowed for {Email} - too soon", email);
                return Result.Failure<string>(
                    Error.Failure("OTP.ResendNotAllowed", MagicStrings.ErrorMessages.ResendNotAllowed)
                );
            }

            string otp = new Random().Next(100000, 999999).ToString();
            _cache.Set($"otp:{email}", otp, TimeSpan.FromMinutes(10));
            _cache.Set($"otp_sent:{email}", DateTime.UtcNow, TimeSpan.FromSeconds(120));

            string customerName = email.Split('@')[0];
            string emailBody = MagicStrings.EmailTemplates.GetVerificationCodeEmail(customerName, otp);
            
            var emailResult = await SendSimpleEmailAsync(email, "RouteBuddy Email Verification (Resent)", emailBody);
            if (emailResult.IsFailure)
            {
                _logger.LogError("Failed to resend OTP email to {Email}", email);
                return Result.Failure<string>(emailResult.Error);
            }

            _logger.LogInformation("OTP resent successfully to {Email}", email);
            return Result.Success(MagicStrings.SuccessMessages.OtpResent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resending OTP to {Email}", email);
            return Result.Failure<string>(
                Error.Failure("OTP.ResendFailed", MagicStrings.ErrorMessages.InternalServerError)
            );
        }
    }

    public async Task<Result<string>> SendForgotPasswordOtpAsync(string email)
    {
        try
        {
            _logger.LogInformation("Sending forgot password OTP to email: {Email}", email);

            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null)
            {
                _logger.LogWarning("User not found for email: {Email}", email);
                return Result.Failure<string>(
                    Error.NotFound("User.NotFound", MagicStrings.ErrorMessages.EmailNotFound)
                );
            }

            string otp = new Random().Next(100000, 999999).ToString();
            _cache.Set($"reset:{email}", otp, TimeSpan.FromMinutes(10));
            _cache.Set($"reset_sent:{email}", DateTime.UtcNow, TimeSpan.FromSeconds(120));

            string customerName = email.Split('@')[0];
            string emailBody = MagicStrings.EmailTemplates.GetPasswordResetEmail(customerName, otp);
            
            var emailResult = await SendSimpleEmailAsync(email, "RouteBuddy Password Reset Code", emailBody);
            if (emailResult.IsFailure)
            {
                _logger.LogError("Failed to send password reset OTP to {Email}", email);
                return Result.Failure<string>(emailResult.Error);
            }

            _logger.LogInformation("Password reset OTP sent successfully to {Email}", email);
            return Result.Success(MagicStrings.SuccessMessages.PasswordResetMailSent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending forgot password OTP to {Email}", email);
            return Result.Failure<string>(
                Error.Failure("PasswordReset.SendFailed", MagicStrings.ErrorMessages.InternalServerError)
            );
        }
    }

    public async Task<Result<string>> ResendForgotPasswordOtpAsync(string email)
    {
        try
        {
            _logger.LogInformation("Resending forgot password OTP to email: {Email}", email);

            if (_cache.TryGetValue($"reset_sent:{email}", out _))
            {
                _logger.LogWarning("Resend not allowed for password reset {Email} - too soon", email);
                return Result.Failure<string>(
                    Error.Failure("PasswordReset.ResendNotAllowed", MagicStrings.ErrorMessages.ResendNotAllowed)
                );
            }

            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null)
            {
                _logger.LogWarning("User not found for email: {Email}", email);
                return Result.Failure<string>(
                    Error.NotFound("User.NotFound", MagicStrings.ErrorMessages.EmailNotFound)
                );
            }

            string otp = new Random().Next(100000, 999999).ToString();
            _cache.Set($"reset:{email}", otp, TimeSpan.FromMinutes(10));
            _cache.Set($"reset_sent:{email}", DateTime.UtcNow, TimeSpan.FromSeconds(120));

            string customerName = email.Split('@')[0];
            string emailBody = MagicStrings.EmailTemplates.GetPasswordResetEmail(customerName, otp);
            
            var emailResult = await SendSimpleEmailAsync(email, "RouteBuddy Password Reset Code (Resent)", emailBody);
            if (emailResult.IsFailure)
            {
                _logger.LogError("Failed to resend password reset OTP to {Email}", email);
                return Result.Failure<string>(emailResult.Error);
            }

            _logger.LogInformation("Password reset OTP resent successfully to {Email}", email);
            return Result.Success(MagicStrings.SuccessMessages.OtpResent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resending forgot password OTP to {Email}", email);
            return Result.Failure<string>(
                Error.Failure("PasswordReset.ResendFailed", MagicStrings.ErrorMessages.InternalServerError)
            );
        }
    }

    public async Task<Result<string>> ResetPasswordAsync(string email, string otp, string newPassword)
    {
        try
        {
            _logger.LogInformation("Resetting password for email: {Email}", email);

            if (!_cache.TryGetValue($"reset:{email}", out string? cachedOtp) || 
                string.IsNullOrEmpty(cachedOtp) || cachedOtp != otp)
            {
                _logger.LogWarning("Invalid or expired reset OTP for {Email}", email);
                return Result.Failure<string>(
                    Error.Failure("OTP.Invalid", MagicStrings.ErrorMessages.InvalidOrExpiredOtp)
                );
            }

            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null)
            {
                _logger.LogWarning("User not found for email: {Email}", email);
                return Result.Failure<string>(
                    Error.NotFound("User.NotFound", MagicStrings.ErrorMessages.EmailNotFound)
                );
            }

            var newHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _userRepository.UpdateUserPasswordAsync(user.UserId, newHash);

            _cache.Remove($"reset:{email}");

            _logger.LogInformation("Password reset successfully for {Email}", email);
            return Result.Success(MagicStrings.SuccessMessages.PasswordUpdated);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting password for {Email}", email);
            return Result.Failure<string>(
                Error.Failure("PasswordReset.Failed", MagicStrings.ErrorMessages.InternalServerError)
            );
        }
    }

    private async Task<Result<string>> SendSimpleEmailAsync(string toEmail, string subject, string body)
    {
        try
        {
            // Use the existing email service but with a simple method
            return await _emailService.SendVendorRejectionEmailAsync(toEmail, subject, body);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending simple email to {Email}", toEmail);
            return Result.Failure<string>(
                Error.Failure("Email.SendFailed", MagicStrings.ErrorMessages.InternalServerError)
            );
        }
    }

    // Admin user management methods
    public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync(int pageNumber, int pageSize)
    {
        try
        {
            _logger.LogInformation("Getting all users - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);
            
            var users = await _userRepository.GetAllUsersAsync(pageNumber, pageSize);
            var userDtos = _mapper.Map<IEnumerable<UserResponseDto>>(users);
            
            return userDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all users");
            return Enumerable.Empty<UserResponseDto>();
        }
    }

    public async Task<IEnumerable<UserResponseDto>> FilterUsersAsync(string? searchTerm, string? role, bool? isActive)
    {
        try
        {
            _logger.LogInformation("Filtering users - Search: {SearchTerm}, Role: {Role}, Active: {IsActive}", 
                searchTerm, role, isActive);
            
            var users = await _userRepository.FilterUsersAsync(searchTerm, role, isActive);
            var userDtos = _mapper.Map<IEnumerable<UserResponseDto>>(users);
            
            return userDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error filtering users");
            return Enumerable.Empty<UserResponseDto>();
        }
    }

    public async Task<UserResponseDto?> GetUserByIdAsync(int userId)
    {
        try
        {
            _logger.LogInformation("Getting user by ID: {UserId}", userId);
            
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                return null;
                
            return _mapper.Map<UserResponseDto>(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by ID: {UserId}", userId);
            return null;
        }
    }

    public async Task<bool> ActivateUserAsync(int userId)
    {
        try
        {
            _logger.LogInformation("Activating user: {UserId}", userId);
            
            return await _userRepository.UpdateUserStatusAsync(userId, true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error activating user: {UserId}", userId);
            return false;
        }
    }

    public async Task<bool> DeactivateUserAsync(int userId)
    {
        try
        {
            _logger.LogInformation("Deactivating user: {UserId}", userId);
            
            return await _userRepository.UpdateUserStatusAsync(userId, false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deactivating user: {UserId}", userId);
            return false;
        }
    }
}