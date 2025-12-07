using Kanini.RouteBuddy.Api.Controllers;
using Kanini.RouteBuddy.Application.Dto.Auth;
using Kanini.RouteBuddy.Application.Services.Auth;
using Kanini.RouteBuddy.Common.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Security.Claims;

namespace Kanini.RouteBuddy.UnitTests.Api.Controllers.Auth;

[TestFixture]
public class AuthControllerTests
{
    private Mock<IAuthService> _mockAuthService;
    private Mock<ILogger<AuthController>> _mockLogger;
    private AuthController _controller;

    [SetUp]
    public void Setup()
    {
        _mockAuthService = new Mock<IAuthService>();
        _mockLogger = new Mock<ILogger<AuthController>>();
        _controller = new AuthController(_mockAuthService.Object, _mockLogger.Object);
        
        // Setup HttpContext for cookie operations
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    [Test]
    public async Task RegisterWithOtp_ValidRequest_ReturnsOk()
    {
        // Arrange
        var request = new RegisterWithOtpRequestDto { Email = "test@example.com" };
        var response = new OtpResponseDto { Email = "test@example.com", Message = "OTP sent" };
        _mockAuthService.Setup(x => x.RegisterWithOtpAsync(request)).ReturnsAsync(Result.Success(response));

        // Act
        var result = await _controller.RegisterWithOtp(request);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult.Value, Is.EqualTo(response));
    }

    [Test]
    public async Task RegisterWithOtp_ServiceFailure_ReturnsBadRequest()
    {
        // Arrange
        var request = new RegisterWithOtpRequestDto { Email = "test@example.com" };
        _mockAuthService.Setup(x => x.RegisterWithOtpAsync(request))
            .ReturnsAsync(Result.Failure<OtpResponseDto>(Error.Failure("Test.Error", "Test error")));

        // Act
        var result = await _controller.RegisterWithOtp(request);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task Login_ValidCredentials_ReturnsOkAndSetsCookies()
    {
        // Arrange
        var request = new LoginRequestDto { Email = "test@example.com", Password = "password" };
        var response = new LoginResponseDto 
        { 
            UserId = 1, 
            Email = "test@example.com", 
            Role = "Customer",
            AccessToken = "access_token",
            RefreshToken = "refresh_token",
            Message = "Login successful"
        };
        _mockAuthService.Setup(x => x.LoginAsync(request)).ReturnsAsync(Result.Success(response));

        // Act
        var result = await _controller.Login(request);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult.Value, Is.Not.Null);
    }

    [Test]
    public async Task Login_InvalidCredentials_ReturnsBadRequest()
    {
        // Arrange
        var request = new LoginRequestDto { Email = "test@example.com", Password = "wrong" };
        _mockAuthService.Setup(x => x.LoginAsync(request))
            .ReturnsAsync(Result.Failure<LoginResponseDto>(Error.Unauthorized("Login.Failed", "Invalid credentials")));

        // Act
        var result = await _controller.Login(request);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task RefreshToken_NoTokenInCookie_ReturnsBadRequest()
    {
        // Act
        var result = await _controller.RefreshToken();

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task RefreshToken_ValidToken_ReturnsOk()
    {
        // Arrange
        _controller.HttpContext.Request.Headers.Cookie = "refreshToken=valid_token";
        var response = new LoginResponseDto { AccessToken = "new_access", RefreshToken = "new_refresh" };
        _mockAuthService.Setup(x => x.RefreshTokenAsync(It.IsAny<RefreshTokenRequestDto>()))
            .ReturnsAsync(Result.Success(response));

        // Act
        var result = await _controller.RefreshToken();

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public async Task Logout_ValidUser_ReturnsOk()
    {
        // Arrange
        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "1") };
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext.HttpContext.User = principal;
        
        _mockAuthService.Setup(x => x.LogoutAsync(It.IsAny<string>(), 1))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _controller.Logout();

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public async Task ChangePassword_ValidRequest_ReturnsOk()
    {
        // Arrange
        var request = new ChangePasswordRequestDto { CurrentPassword = "old", NewPassword = "new" };
        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "1") };
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext.HttpContext.User = principal;
        
        _mockAuthService.Setup(x => x.ChangePasswordAsync(1, request)).ReturnsAsync(Result.Success());

        // Act
        var result = await _controller.ChangePassword(request);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public async Task ForgotPassword_ValidEmail_ReturnsOk()
    {
        // Arrange
        var request = new ForgotPasswordDto { Email = "test@example.com" };
        var response = new OtpResponseDto { Email = "test@example.com", Message = "OTP sent" };
        _mockAuthService.Setup(x => x.ForgotPasswordAsync(request)).ReturnsAsync(Result.Success(response));

        // Act
        var result = await _controller.ForgotPassword(request);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public async Task VerifyForgotPasswordOtp_ValidOtp_ReturnsOk()
    {
        // Arrange
        var request = new VerifyForgotPasswordOtpRequestDto { Email = "test@example.com", Otp = "123456" };
        _mockAuthService.Setup(x => x.VerifyForgotPasswordOtpAsync(request)).ReturnsAsync(Result.Success());

        // Act
        var result = await _controller.VerifyForgotPasswordOtp(request);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public async Task ResetPassword_ValidRequest_ReturnsOk()
    {
        // Arrange
        var request = new ResetPasswordDto { Email = "test@example.com", NewPassword = "newpass" };
        _mockAuthService.Setup(x => x.ResetPasswordAsync(request)).ReturnsAsync(Result.Success());

        // Act
        var result = await _controller.ResetPassword(request);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public async Task CompleteCustomerProfile_ValidRequest_ReturnsOk()
    {
        // Arrange
        var request = new CompleteCustomerProfileDto { FirstName = "John", LastName = "Doe" };
        _mockAuthService.Setup(x => x.CompleteCustomerProfileAsync(1, request)).ReturnsAsync(Result.Success());

        // Act
        var result = await _controller.CompleteCustomerProfile(1, request);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public async Task CompleteVendorProfile_ValidRequest_ReturnsOk()
    {
        // Arrange
        var request = new CompleteVendorProfileDto { AgencyName = "Test Agency", OwnerName = "John Doe" };
        _mockAuthService.Setup(x => x.CompleteVendorProfileAsync(1, request)).ReturnsAsync(Result.Success());

        // Act
        var result = await _controller.CompleteVendorProfile(1, request);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }
}