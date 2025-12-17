using Kanini.RouteBuddy.Application.Dto.Auth;
using Kanini.RouteBuddy.Application.Services.Email;
using Kanini.RouteBuddy.Common;
using Kanini.RouteBuddy.Common.Services;
using Kanini.RouteBuddy.Common.Utility;
using Kanini.RouteBuddy.Data.Repositories.Customer;
using Kanini.RouteBuddy.Data.Repositories.Token;
using Kanini.RouteBuddy.Data.Repositories.User;
using Kanini.RouteBuddy.Data.Repositories.Vendor;
using Kanini.RouteBuddy.Data.Repositories.VendorDocuments;
using Kanini.RouteBuddy.Domain.Enums;
using Microsoft.Extensions.Logging;
using UserEntity = Kanini.RouteBuddy.Domain.Entities.User;
using CustomerEntity = Kanini.RouteBuddy.Domain.Entities.Customer;
using RefreshTokenEntity = Kanini.RouteBuddy.Domain.Entities.RefreshToken;
using VendorDocumentEntity = Kanini.RouteBuddy.Domain.Entities.VendorDocument;

namespace Kanini.RouteBuddy.Application.Services.Auth;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IVendorRepository _vendorRepository;
    private readonly IVendorDocumentRepository _vendorDocumentRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IJwtOtpService _jwtOtpService;
    private readonly IEmailService _emailService;
    private readonly ICaptchaService _captchaService;
    private readonly ILogger<AuthService> _logger;
    private readonly BlobService _blobService;

    public AuthService(
        IUserRepository userRepository,
        ICustomerRepository customerRepository,
        IVendorRepository vendorRepository,
        IVendorDocumentRepository vendorDocumentRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IJwtTokenService jwtTokenService,
        IJwtOtpService jwtOtpService,
        IEmailService emailService,
        ICaptchaService captchaService,
        ILogger<AuthService> logger,
        BlobService blobService)
    {
        _userRepository = userRepository;
        _customerRepository = customerRepository;
        _vendorRepository = vendorRepository;
        _vendorDocumentRepository = vendorDocumentRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtTokenService = jwtTokenService;
        _jwtOtpService = jwtOtpService;
        _emailService = emailService;
        _captchaService = captchaService;
        _logger = logger;
        _blobService = blobService;
    }

    public async Task<Result<OtpResponseDto>> RegisterWithOtpAsync(RegisterWithOtpRequestDto request)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.RegistrationOtpStarted, request.Email);

            // Verify reCAPTCHA
            var captchaResult = await _captchaService.VerifyRecaptchaAsync(request.RecaptchaToken, request.Email);
            if (captchaResult.IsFailure)
            {
                return Result.Failure<OtpResponseDto>(captchaResult.Error);
            }

            if (await _userRepository.ExistsByEmailAsync(request.Email))
            {
                _logger.LogWarning("Registration failed: Email already exists - {Email}", request.Email);
                return Result.Failure<OtpResponseDto>(Error.Conflict("Email.Exists", MagicStrings.ErrorMessages.EmailAlreadyExists));
            }

            if (await _userRepository.ExistsByPhoneAsync(request.Phone))
            {
                _logger.LogWarning("Registration failed: Phone already exists - {Phone}", request.Phone);
                return Result.Failure<OtpResponseDto>(Error.Conflict("Phone.Exists", MagicStrings.ErrorMessages.PhoneAlreadyExists));
            }

            var otp = _jwtOtpService.GenerateOtp();

            var registrationData = new RegistrationDataDto
            {
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Phone = request.Phone
            };

            var otpToken = _jwtOtpService.CreateOtpTokenWithData(
                request.Email,
                otp,
                $"Registration_{request.Role}",
                registrationData
            );

            string emailBody = MagicStrings.EmailTemplates.GetVerificationCodeEmail(request.Email.Split('@')[0], otp);
            await _emailService.SendGenericEmailAsync(request.Email, "RouteBuddy Registration OTP", emailBody);

            _logger.LogInformation(MagicStrings.LogMessages.RegistrationOtpCompleted, request.Email);

            return Result.Success(new OtpResponseDto
            {
                Email = request.Email,
                OtpToken = otpToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5),
                Message = MagicStrings.SuccessMessages.OtpSent
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.RegistrationOtpFailed, request.Email, ex.Message);
            return Result.Failure<OtpResponseDto>(Error.Failure("Registration.Failed", MagicStrings.ErrorMessages.RegistrationFailed));
        }
    }

    public async Task<Result<OtpResponseDto>> ResendRegistrationOtpAsync(ResendOtpRequestDto request)
    {
        try
        {
            _logger.LogInformation("Resending OTP for Email: {Email}", request.Email);

            if (await _userRepository.ExistsByEmailAsync(request.Email))
            {
                _logger.LogWarning("Resend OTP failed: Email already registered - {Email}", request.Email);
                return Result.Failure<OtpResponseDto>(Error.Conflict("Email.Exists", MagicStrings.ErrorMessages.EmailAlreadyExists));
            }

            var otp = _jwtOtpService.GenerateOtp();

            var registrationData = new RegistrationDataDto
            {
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Phone = request.Phone
            };

            var otpToken = _jwtOtpService.CreateOtpTokenWithData(
                request.Email,
                otp,
                $"Registration_{request.Role}",
                registrationData
            );

            string emailBody = MagicStrings.EmailTemplates.GetVerificationCodeEmail(request.Email.Split('@')[0], otp);
            await _emailService.SendGenericEmailAsync(request.Email, "RouteBuddy Registration OTP", emailBody);

            _logger.LogInformation("OTP resent successfully for Email: {Email}", request.Email);

            return Result.Success(new OtpResponseDto
            {
                Email = request.Email,
                OtpToken = otpToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5),
                Message = MagicStrings.SuccessMessages.OtpSent
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Resend OTP failed for Email: {Email}, Error: {Error}", request.Email, ex.Message);
            return Result.Failure<OtpResponseDto>(Error.Failure("ResendOtp.Failed", "Failed to resend OTP"));
        }
    }

    public async Task<Result<RegistrationResponseDto>> VerifyRegistrationOtpAsync(VerifyRegistrationOtpRequestDto request)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.RegistrationVerificationStarted, request.Email);

            if (!_jwtOtpService.ValidateOtp(request.OtpToken, request.Otp, request.Email, $"Registration_{request.Role}"))
            {
                _logger.LogWarning("OTP validation failed for Email: {Email}", request.Email);
                return Result.Failure<RegistrationResponseDto>(Error.Unauthorized("OTP.Invalid", MagicStrings.ErrorMessages.InvalidOrExpiredOtp));
            }

            if (await _userRepository.ExistsByEmailAsync(request.Email))
            {
                _logger.LogWarning("Registration failed: Email already exists - {Email}", request.Email);
                return Result.Failure<RegistrationResponseDto>(Error.Conflict("Email.Exists", MagicStrings.ErrorMessages.EmailAlreadyExists));
            }

            var registrationData = _jwtOtpService.GetRegistrationDataFromToken(request.OtpToken);
            if (registrationData == null)
            {
                _logger.LogWarning("Failed to extract registration data from token for Email: {Email}", request.Email);
                return Result.Failure<RegistrationResponseDto>(Error.Unauthorized("Token.Invalid", MagicStrings.ErrorMessages.InvalidOrExpiredOtp));
            }

            var user = new UserEntity
            {
                Email = request.Email,
                PasswordHash = registrationData.PasswordHash,
                Phone = registrationData.Phone,
                Role = (UserRole)request.Role,
                IsEmailVerified = true,
                IsActive = request.Role == 1, // Only activate Customer accounts immediately, Vendors need approval
                CreatedBy = request.Email,
                CreatedOn = DateTime.UtcNow
            };

            await _userRepository.CreateUserAsync(user);

            _logger.LogInformation(MagicStrings.LogMessages.RegistrationVerificationCompleted, request.Email, user.UserId);

            var response = new RegistrationResponseDto
            {
                UserId = user.UserId,
                Email = user.Email,
                Role = user.Role.ToString(),
                Message = MagicStrings.SuccessMessages.RegistrationCompleted
            };

            if (request.Role == 1) // Customer
            {
                response.RequiresVendorProfile = false;
                response.Message = "User account created. Please complete your customer profile.";
            }
            else // Vendor
            {
                response.RequiresVendorProfile = true;
                response.Message = "User account created. Please complete your vendor profile.";
            }

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.RegistrationVerificationFailed, request.Email, ex.Message);
            return Result.Failure<RegistrationResponseDto>(Error.Failure("Verification.Failed", MagicStrings.ErrorMessages.OtpVerificationFailed));
        }
    }

    public async Task<Result<LoginResponseDto>> LoginAsync(LoginRequestDto request)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.LoginAttemptStarted, request.Email);

            var user = await _userRepository.GetUserByEmailAsync(request.Email);
            if (user == null)
            {
                _logger.LogWarning("Login failed: User not found - {Email}", request.Email);
                return Result.Failure<LoginResponseDto>(Error.Unauthorized("Login.Failed", MagicStrings.ErrorMessages.InvalidCredentials));
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                _logger.LogWarning("Login failed: Invalid password - {Email}", request.Email);
                return Result.Failure<LoginResponseDto>(Error.Unauthorized("Login.Failed", MagicStrings.ErrorMessages.InvalidCredentials));
            }

            if (!user.IsActive)
            {
                _logger.LogWarning("Login failed: Account inactive - {Email}", request.Email);
                return Result.Failure<LoginResponseDto>(Error.Unauthorized("Account.Inactive", MagicStrings.ErrorMessages.AccountInactive));
            }

            // Additional check for vendor approval status
            if (user.Role == UserRole.Vendor)
            {
                var vendor = await _vendorRepository.GetByUserIdAsync(user.UserId);
                if (vendor == null || vendor.Status != VendorStatus.Active)
                {
                    _logger.LogWarning("Login failed: Vendor not approved - {Email}, Status: {Status}", request.Email, vendor?.Status.ToString() ?? "NotFound");
                    return Result.Failure<LoginResponseDto>(Error.Unauthorized("Vendor.NotApproved", "Your vendor account is pending approval. Please contact admin."));
                }
            }

            // Revoke all existing tokens for single active session
            await _refreshTokenRepository.RevokeAllByUserIdAsync(user.UserId);

            var accessToken = _jwtTokenService.GenerateAccessToken(user);
            var refreshTokenValue = _jwtTokenService.GenerateRefreshToken();

            var refreshToken = new RefreshTokenEntity
            {
                Token = refreshTokenValue,
                ExpiresAt = _jwtTokenService.GetRefreshTokenExpiry(),
                UserId = user.UserId,
                CreatedBy = user.Email,
                CreatedOn = DateTime.UtcNow
            };

            await _refreshTokenRepository.CreateRefreshTokenAsync(refreshToken);
            await _userRepository.UpdateLastLoginAsync(user.UserId);

            _logger.LogInformation(MagicStrings.LogMessages.LoginAttemptCompleted, request.Email, user.UserId);

            return Result.Success(new LoginResponseDto
            {
                UserId = user.UserId,
                Email = user.Email,
                Role = user.Role.ToString(),
                AccessToken = accessToken,
                RefreshToken = refreshTokenValue,
                AccessTokenExpiry = _jwtTokenService.GetAccessTokenExpiry(),
                RefreshTokenExpiry = _jwtTokenService.GetRefreshTokenExpiry(),
                Message = MagicStrings.SuccessMessages.LoginSuccessful
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.LoginAttemptFailed, request.Email, ex.Message);
            return Result.Failure<LoginResponseDto>(Error.Failure("Login.Failed", MagicStrings.ErrorMessages.LoginFailed));
        }
    }

    public async Task<Result<LoginResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request)
    {
        try
        {
            var user = await _refreshTokenRepository.GetUserByRefreshTokenAsync(request.RefreshToken);
            if (user == null)
                return Result.Failure<LoginResponseDto>(Error.Unauthorized("Token.Invalid", "Invalid refresh token"));

            if (!user.IsActive)
                return Result.Failure<LoginResponseDto>(Error.Unauthorized("Account.Inactive", "Account is inactive"));

            // Additional check for vendor approval status
            if (user.Role == UserRole.Vendor)
            {
                var vendor = await _vendorRepository.GetByUserIdAsync(user.UserId);
                if (vendor == null || vendor.Status != VendorStatus.Active)
                {
                    return Result.Failure<LoginResponseDto>(Error.Unauthorized("Vendor.NotApproved", "Your vendor account is no longer approved. Please contact admin."));
                }
            }

            await _refreshTokenRepository.RevokeRefreshTokenAsync(request.RefreshToken);

            var accessToken = _jwtTokenService.GenerateAccessToken(user);
            var newRefreshTokenValue = _jwtTokenService.GenerateRefreshToken();

            var newRefreshToken = new RefreshTokenEntity
            {
                Token = newRefreshTokenValue,
                ExpiresAt = _jwtTokenService.GetRefreshTokenExpiry(),
                UserId = user.UserId,
                CreatedBy = user.Email,
                CreatedOn = DateTime.UtcNow
            };

            await _refreshTokenRepository.CreateRefreshTokenAsync(newRefreshToken);

            return Result.Success(new LoginResponseDto
            {
                UserId = user.UserId,
                Email = user.Email,
                Role = user.Role.ToString(),
                AccessToken = accessToken,
                RefreshToken = newRefreshTokenValue,
                AccessTokenExpiry = _jwtTokenService.GetAccessTokenExpiry(),
                RefreshTokenExpiry = _jwtTokenService.GetRefreshTokenExpiry(),
                Message = "Token refreshed successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in RefreshTokenAsync");
            return Result.Failure<LoginResponseDto>(Error.Failure("Refresh.Failed", "Token refresh failed"));
        }
    }

    public async Task<Result> LogoutAsync(string refreshToken, int userId)
    {
        try
        {
            await _refreshTokenRepository.RevokeRefreshTokenAsync(refreshToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in LogoutAsync");
            return Result.Failure(Error.Failure("Logout.Failed", "Logout failed"));
        }
    }

    public async Task<Result> ChangePasswordAsync(int userId, ChangePasswordRequestDto request)
    {
        try
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                return Result.Failure(Error.NotFound("User.NotFound", "User not found"));

            if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
                return Result.Failure(Error.Unauthorized("Password.Invalid", "Current password is incorrect"));

            var newPasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            await _userRepository.UpdateUserPasswordAsync(userId, newPasswordHash);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ChangePasswordAsync");
            return Result.Failure(Error.Failure("PasswordChange.Failed", "Password change failed"));
        }
    }

    public async Task<Result<OtpResponseDto>> ForgotPasswordAsync(ForgotPasswordDto request)
    {
        try
        {
            var user = await _userRepository.GetUserByEmailAsync(request.Email);
            if (user == null)
                return Result.Failure<OtpResponseDto>(Error.NotFound("User.NotFound", "Email not found"));

            var otp = _jwtOtpService.GenerateOtp();
            var otpToken = _jwtOtpService.CreateOtpToken(request.Email, otp, "ForgotPassword");

            string emailBody = $"<h2>Password Reset</h2><p>Your OTP is: <strong>{otp}</strong></p><p>Valid for 5 minutes.</p>";
            await _emailService.SendGenericEmailAsync(request.Email, "Password Reset OTP", emailBody);

            return Result.Success(new OtpResponseDto
            {
                Email = request.Email,
                OtpToken = otpToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5),
                Message = "OTP sent successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ForgotPasswordAsync");
            return Result.Failure<OtpResponseDto>(Error.Failure("ForgotPassword.Failed", "Failed to send OTP"));
        }
    }

    public Task<Result> VerifyForgotPasswordOtpAsync(VerifyForgotPasswordOtpRequestDto request)
    {
        try
        {
            if (!_jwtOtpService.ValidateOtp(request.OtpToken, request.Otp, request.Email, "ForgotPassword"))
                return Task.FromResult(Result.Failure(Error.Unauthorized("OTP.Invalid", "Invalid or expired OTP")));

            return Task.FromResult(Result.Success());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in VerifyForgotPasswordOtpAsync");
            return Task.FromResult(Result.Failure(Error.Failure("Verification.Failed", "OTP verification failed")));
        }
    }

    public async Task<Result> ResetPasswordAsync(ResetPasswordDto request)
    {
        try
        {
            var user = await _userRepository.GetUserByEmailAsync(request.Email);
            if (user == null)
                return Result.Failure(Error.NotFound("User.NotFound", "User not found"));

            var newPasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            await _userRepository.UpdateUserPasswordAsync(user.UserId, newPasswordHash);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ResetPasswordAsync");
            return Result.Failure(Error.Failure("ResetPassword.Failed", "Password reset failed"));
        }
    }

    public async Task<Result> CompleteCustomerProfileAsync(int userId, CompleteCustomerProfileDto request)
    {
        try
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                return Result.Failure(Error.NotFound("User.NotFound", "User not found"));

            if (user.Role != UserRole.Customer)
                return Result.Failure(Error.Failure("User.InvalidRole", "User is not a customer"));

            var customer = new CustomerEntity
            {
                FirstName = request.FirstName,
                MiddleName = request.MiddleName,
                LastName = request.LastName,
                DateOfBirth = request.DateOfBirth,
                Gender = (Gender)request.Gender,
                IsActive = true,
                UserId = userId,
                CreatedBy = user.Email,
                CreatedOn = DateTime.UtcNow
            };

            await _customerRepository.CreateCustomerAsync(customer);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing customer profile");
            return Result.Failure(Error.Failure("Profile.Failed", "Failed to complete profile"));
        }
    }

    public async Task<Result> CompleteVendorProfileAsync(int userId, CompleteVendorProfileDto request)
    {
        try
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                return Result.Failure(Error.NotFound("User.NotFound", "User not found"));

            if (user.Role != UserRole.Vendor)
                return Result.Failure(Error.Failure("User.InvalidRole", "User is not a vendor"));

            var vendor = new Domain.Entities.Vendor
            {
                AgencyName = request.AgencyName,
                OwnerName = request.OwnerName,
                BusinessLicenseNumber = request.BusinessLicenseNumber,
                OfficeAddress = request.OfficeAddress,
                FleetSize = request.FleetSize,
                TaxRegistrationNumber = request.TaxRegistrationNumber,
                Status = VendorStatus.PendingApproval,
                IsActive = false,
                UserId = userId,
                CreatedBy = user.Email,
                CreatedOn = DateTime.UtcNow
            };

            await _vendorRepository.CreateVendorAsync(vendor);

            // Save documents
            if (request.BusinessLicenseDocument != null)
            {
                var docPath = await SaveDocumentAsync(request.BusinessLicenseDocument, vendor.VendorId, "BusinessLicense");
                await _vendorDocumentRepository.AddVendorDocumentAsync(new VendorDocumentEntity
                {
                    VendorId = vendor.VendorId,
                    DocumentFile = DocumentCategory.BusinessLicense,
                    DocumentPath = docPath,
                    UploadedAt = DateTime.UtcNow,
                    Status = DocumentStatus.Pending,
                    CreatedBy = user.Email,
                    CreatedOn = DateTime.UtcNow
                });
            }

            if (request.TaxRegistrationDocument != null)
            {
                var docPath = await SaveDocumentAsync(request.TaxRegistrationDocument, vendor.VendorId, "TaxRegistration");
                await _vendorDocumentRepository.AddVendorDocumentAsync(new VendorDocumentEntity
                {
                    VendorId = vendor.VendorId,
                    DocumentFile = DocumentCategory.TaxRegistration,
                    DocumentPath = docPath,
                    UploadedAt = DateTime.UtcNow,
                    Status = DocumentStatus.Pending,
                    CreatedBy = user.Email,
                    CreatedOn = DateTime.UtcNow
                });
            }

            if (request.OwnerIdentityDocument != null)
            {
                var docPath = await SaveDocumentAsync(request.OwnerIdentityDocument, vendor.VendorId, "OwnerIdentity");
                await _vendorDocumentRepository.AddVendorDocumentAsync(new VendorDocumentEntity
                {
                    VendorId = vendor.VendorId,
                    DocumentFile = DocumentCategory.OwnerIdentity,
                    DocumentPath = docPath,
                    UploadedAt = DateTime.UtcNow,
                    Status = DocumentStatus.Pending,
                    CreatedBy = user.Email,
                    CreatedOn = DateTime.UtcNow
                });
            }

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing vendor profile");
            return Result.Failure(Error.Failure("Profile.Failed", "Failed to complete profile"));
        }
    }

    private async Task<string> SaveDocumentAsync(Microsoft.AspNetCore.Http.IFormFile file, int vendorId, string category)
    {
        var uploadResult = await _blobService.UploadFileAsync(file);
        return uploadResult.BlobUrl;
    }
}
