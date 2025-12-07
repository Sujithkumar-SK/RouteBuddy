using Kanini.RouteBuddy.Application.Dto.Auth;
using Kanini.RouteBuddy.Application.Services;
using Kanini.RouteBuddy.Application.Services.Auth;
using Kanini.RouteBuddy.Application.Services.Email;
using Kanini.RouteBuddy.Common.Utility;
using Kanini.RouteBuddy.Data.Repositories.Customer;
using Kanini.RouteBuddy.Data.Repositories.Token;
using Kanini.RouteBuddy.Data.Repositories.User;
using Kanini.RouteBuddy.Data.Repositories.Vendor;
using Kanini.RouteBuddy.Data.Repositories.VendorDocuments;
using Kanini.RouteBuddy.Domain.Entities;
using Kanini.RouteBuddy.Domain.Enums;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Kanini.RouteBuddy.UnitTests.Api.Services.Auth;

[TestFixture]
public class AuthServiceTests
{
    private Mock<IUserRepository> _mockUserRepository;
    private Mock<ICustomerRepository> _mockCustomerRepository;
    private Mock<IVendorRepository> _mockVendorRepository;
    private Mock<IVendorDocumentRepository> _mockVendorDocumentRepository;
    private Mock<IRefreshTokenRepository> _mockRefreshTokenRepository;
    private Mock<IJwtTokenService> _mockJwtTokenService;
    private Mock<IJwtOtpService> _mockJwtOtpService;
    private Mock<IEmailService> _mockEmailService;
    private Mock<ICaptchaService> _mockCaptchaService;
    private Mock<ILogger<AuthService>> _mockLogger;
    private AuthService _service;

    [SetUp]
    public void Setup()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockCustomerRepository = new Mock<ICustomerRepository>();
        _mockVendorRepository = new Mock<IVendorRepository>();
        _mockVendorDocumentRepository = new Mock<IVendorDocumentRepository>();
        _mockRefreshTokenRepository = new Mock<IRefreshTokenRepository>();
        _mockJwtTokenService = new Mock<IJwtTokenService>();
        _mockJwtOtpService = new Mock<IJwtOtpService>();
        _mockEmailService = new Mock<IEmailService>();
        _mockCaptchaService = new Mock<ICaptchaService>();
        _mockLogger = new Mock<ILogger<AuthService>>();

        _service = new AuthService(_mockUserRepository.Object, _mockCustomerRepository.Object,
            _mockVendorRepository.Object, _mockVendorDocumentRepository.Object, _mockRefreshTokenRepository.Object,
            _mockJwtTokenService.Object, _mockJwtOtpService.Object, _mockEmailService.Object,
            _mockCaptchaService.Object, _mockLogger.Object);
    }

    [Test]
    public async Task RegisterWithOtpAsync_CaptchaFails_ReturnsFailure()
    {
        // Arrange
        var request = new RegisterWithOtpRequestDto { Email = "test@example.com", RecaptchaToken = "invalid" };
        _mockCaptchaService.Setup(x => x.VerifyRecaptchaAsync("invalid", "test@example.com"))
            .ReturnsAsync(Result.Failure<bool>(Error.Failure("Captcha.Failed", "Invalid captcha")));

        // Act
        var result = await _service.RegisterWithOtpAsync(request);

        // Assert
        Assert.That(result.IsFailure, Is.True);
    }

    [Test]
    public async Task RegisterWithOtpAsync_EmailExists_ReturnsFailure()
    {
        // Arrange
        var request = new RegisterWithOtpRequestDto { Email = "test@example.com", RecaptchaToken = "valid" };
        _mockCaptchaService.Setup(x => x.VerifyRecaptchaAsync("valid", "test@example.com"))
            .ReturnsAsync(Result.Success(true));
        _mockUserRepository.Setup(x => x.ExistsByEmailAsync("test@example.com")).ReturnsAsync(true);

        // Act
        var result = await _service.RegisterWithOtpAsync(request);

        // Assert
        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error.Code, Is.EqualTo("Email.Exists"));
    }

    [Test]
    public async Task RegisterWithOtpAsync_PhoneExists_ReturnsFailure()
    {
        // Arrange
        var request = new RegisterWithOtpRequestDto { Email = "test@example.com", Phone = "9876543210", RecaptchaToken = "valid" };
        _mockCaptchaService.Setup(x => x.VerifyRecaptchaAsync("valid", "test@example.com"))
            .ReturnsAsync(Result.Success(true));
        _mockUserRepository.Setup(x => x.ExistsByEmailAsync("test@example.com")).ReturnsAsync(false);
        _mockUserRepository.Setup(x => x.ExistsByPhoneAsync("9876543210")).ReturnsAsync(true);

        // Act
        var result = await _service.RegisterWithOtpAsync(request);

        // Assert
        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error.Code, Is.EqualTo("Phone.Exists"));
    }

    //[Test]
    //public async Task RegisterWithOtpAsync_ValidRequest_ReturnsSuccess()
    //{
    //    // Arrange
    //    var request = new RegisterWithOtpRequestDto { Email = "test@example.com", Phone = "9876543210", RecaptchaToken = "valid" };
    //    _mockCaptchaService.Setup(x => x.VerifyRecaptchaAsync("valid", "test@example.com"))
    //        .ReturnsAsync(Result.Success(true));
    //    _mockUserRepository.Setup(x => x.ExistsByEmailAsync("test@example.com")).ReturnsAsync(false);
    //    _mockUserRepository.Setup(x => x.ExistsByPhoneAsync("9876543210")).ReturnsAsync(false);
    //    _mockJwtOtpService.Setup(x => x.GenerateOtp()).Returns("123456");
    //    _mockJwtOtpService.Setup(x => x.CreateOtpTokenWithData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<RegistrationDataDto>()))
    //        .Returns("token");
    //    _mockEmailService.Setup(x => x.SendGenericEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
    //        .ReturnsAsync(Result.Success("Email sent"));

    //    // Act
    //    var result = await _service.RegisterWithOtpAsync(request);

    //    // Assert
    //    Assert.That(result.IsSuccess, Is.True);
    //    Assert.That(result.Value.Email, Is.EqualTo("test@example.com"));
    //}

    [Test]
    public async Task LoginAsync_UserNotFound_ReturnsFailure()
    {
        // Arrange
        var request = new LoginRequestDto { Email = "test@example.com", Password = "password" };
        _mockUserRepository.Setup(x => x.GetUserByEmailAsync("test@example.com")).ReturnsAsync((User)null);

        // Act
        var result = await _service.LoginAsync(request);

        // Assert
        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error.Code, Is.EqualTo("Login.Failed"));
    }

    [Test]
    public async Task LoginAsync_InvalidPassword_ReturnsFailure()
    {
        // Arrange
        var request = new LoginRequestDto { Email = "test@example.com", Password = "wrongpassword" };
        var user = new User { Email = "test@example.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("correctpassword") };
        _mockUserRepository.Setup(x => x.GetUserByEmailAsync("test@example.com")).ReturnsAsync(user);

        // Act
        var result = await _service.LoginAsync(request);

        // Assert
        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error.Code, Is.EqualTo("Login.Failed"));
    }

    [Test]
    public async Task LoginAsync_InactiveAccount_ReturnsFailure()
    {
        // Arrange
        var request = new LoginRequestDto { Email = "test@example.com", Password = "password" };
        var user = new User { Email = "test@example.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"), IsActive = false };
        _mockUserRepository.Setup(x => x.GetUserByEmailAsync("test@example.com")).ReturnsAsync(user);

        // Act
        var result = await _service.LoginAsync(request);

        // Assert
        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error.Code, Is.EqualTo("Account.Inactive"));
    }

    [Test]
    public async Task LoginAsync_ValidCredentials_ReturnsSuccess()
    {
        // Arrange
        var request = new LoginRequestDto { Email = "test@example.com", Password = "password" };
        var user = new User 
        { 
            UserId = 1, 
            Email = "test@example.com", 
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"), 
            IsActive = true,
            Role = UserRole.Customer
        };
        
        _mockUserRepository.Setup(x => x.GetUserByEmailAsync("test@example.com")).ReturnsAsync(user);
        _mockRefreshTokenRepository.Setup(x => x.RevokeAllByUserIdAsync(1)).ReturnsAsync(Result.Success(true));
        _mockJwtTokenService.Setup(x => x.GenerateAccessToken(user)).Returns("access_token");
        _mockJwtTokenService.Setup(x => x.GenerateRefreshToken()).Returns("refresh_token");
        _mockJwtTokenService.Setup(x => x.GetRefreshTokenExpiry()).Returns(DateTime.UtcNow.AddDays(7));
        _mockJwtTokenService.Setup(x => x.GetAccessTokenExpiry()).Returns(DateTime.UtcNow.AddMinutes(15));
        _mockRefreshTokenRepository.Setup(x => x.CreateRefreshTokenAsync(It.IsAny<RefreshToken>())).ReturnsAsync(new RefreshToken());
        _mockUserRepository.Setup(x => x.UpdateLastLoginAsync(1)).Returns(Task.CompletedTask);

        // Act
        var result = await _service.LoginAsync(request);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.UserId, Is.EqualTo(1));
        Assert.That(result.Value.Email, Is.EqualTo("test@example.com"));
    }

    [Test]
    public async Task ChangePasswordAsync_UserNotFound_ReturnsFailure()
    {
        // Arrange
        var request = new ChangePasswordRequestDto { CurrentPassword = "old", NewPassword = "new" };
        _mockUserRepository.Setup(x => x.GetUserByIdAsync(1)).ReturnsAsync((User)null);

        // Act
        var result = await _service.ChangePasswordAsync(1, request);

        // Assert
        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error.Code, Is.EqualTo("User.NotFound"));
    }

    [Test]
    public async Task ChangePasswordAsync_InvalidCurrentPassword_ReturnsFailure()
    {
        // Arrange
        var request = new ChangePasswordRequestDto { CurrentPassword = "wrong", NewPassword = "new" };
        var user = new User { PasswordHash = BCrypt.Net.BCrypt.HashPassword("correct") };
        _mockUserRepository.Setup(x => x.GetUserByIdAsync(1)).ReturnsAsync(user);

        // Act
        var result = await _service.ChangePasswordAsync(1, request);

        // Assert
        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error.Code, Is.EqualTo("Password.Invalid"));
    }

    [Test]
    public async Task ChangePasswordAsync_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var request = new ChangePasswordRequestDto { CurrentPassword = "old", NewPassword = "new" };
        var user = new User { PasswordHash = BCrypt.Net.BCrypt.HashPassword("old") };
        _mockUserRepository.Setup(x => x.GetUserByIdAsync(1)).ReturnsAsync(user);
        _mockUserRepository.Setup(x => x.UpdateUserPasswordAsync(1, It.IsAny<string>())).Returns(Task.CompletedTask);

        // Act
        var result = await _service.ChangePasswordAsync(1, request);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
    }

    [Test]
    public async Task ForgotPasswordAsync_UserNotFound_ReturnsFailure()
    {
        // Arrange
        var request = new ForgotPasswordDto { Email = "test@example.com" };
        _mockUserRepository.Setup(x => x.GetUserByEmailAsync("test@example.com")).ReturnsAsync((User)null);

        // Act
        var result = await _service.ForgotPasswordAsync(request);

        // Assert
        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error.Code, Is.EqualTo("User.NotFound"));
    }

    [Test]
    public async Task CompleteCustomerProfileAsync_UserNotFound_ReturnsFailure()
    {
        // Arrange
        var request = new CompleteCustomerProfileDto();
        _mockUserRepository.Setup(x => x.GetUserByIdAsync(1)).ReturnsAsync((User)null);

        // Act
        var result = await _service.CompleteCustomerProfileAsync(1, request);

        // Assert
        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error.Code, Is.EqualTo("User.NotFound"));
    }

    [Test]
    public async Task CompleteCustomerProfileAsync_InvalidRole_ReturnsFailure()
    {
        // Arrange
        var request = new CompleteCustomerProfileDto();
        var user = new User { Role = UserRole.Vendor };
        _mockUserRepository.Setup(x => x.GetUserByIdAsync(1)).ReturnsAsync(user);

        // Act
        var result = await _service.CompleteCustomerProfileAsync(1, request);

        // Assert
        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error.Code, Is.EqualTo("User.InvalidRole"));
    }

    [Test]
    public async Task LogoutAsync_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        _mockRefreshTokenRepository.Setup(x => x.RevokeRefreshTokenAsync("token")).Returns(Task.CompletedTask);

        // Act
        var result = await _service.LogoutAsync("token", 1);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
    }
}