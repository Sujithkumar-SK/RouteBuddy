using Kanini.RouteBuddy.Application.Dto.Auth;
using Kanini.RouteBuddy.Application.Services.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.IdentityModel.Tokens.Jwt;

namespace Kanini.RouteBuddy.UnitTests.Api.Services.Auth;

[TestFixture]
public class JwtOtpServiceTests
{
    private Mock<IConfiguration> _mockConfiguration;
    private Mock<ILogger<JwtOtpService>> _mockLogger;
    private JwtOtpService _service;

    [SetUp]
    public void Setup()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        _mockLogger = new Mock<ILogger<JwtOtpService>>();
        
        _mockConfiguration.Setup(x => x["TokenKey"]).Returns("ThisIsAVeryLongSecretKeyForJWTTokenGeneration123456789");
        
        _service = new JwtOtpService(_mockConfiguration.Object, _mockLogger.Object);
    }

    [Test]
    public void GenerateOtp_ReturnsValidOtp()
    {
        // Act
        var otp = _service.GenerateOtp();

        // Assert
        Assert.That(otp, Is.Not.Null);
        Assert.That(otp.Length, Is.EqualTo(6));
        Assert.That(otp, Does.Match(@"^\d{6}$"));
    }

    [Test]
    public void CreateOtpToken_ValidInput_ReturnsToken()
    {
        // Arrange
        var email = "test@example.com";
        var otp = "123456";
        var otpType = "Registration";

        // Act
        var token = _service.CreateOtpToken(email, otp, otpType);

        // Assert
        Assert.That(token, Is.Not.Null);
        Assert.That(token, Is.Not.Empty);
        
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadJwtToken(token);
        Assert.That(jsonToken.Claims.First(x => x.Type == "email").Value, Is.EqualTo(email));
        Assert.That(jsonToken.Claims.First(x => x.Type == "otp").Value, Is.EqualTo(otp));
        Assert.That(jsonToken.Claims.First(x => x.Type == "otpType").Value, Is.EqualTo(otpType));
    }

    [Test]
    public void CreateOtpTokenWithData_ValidInput_ReturnsToken()
    {
        // Arrange
        var email = "test@example.com";
        var otp = "123456";
        var otpType = "Registration";
        var data = new RegistrationDataDto { Email = email, Phone = "9876543210" };

        // Act
        var token = _service.CreateOtpTokenWithData(email, otp, otpType, data);

        // Assert
        Assert.That(token, Is.Not.Null);
        Assert.That(token, Is.Not.Empty);
        
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadJwtToken(token);
        Assert.That(jsonToken.Claims.First(x => x.Type == "email").Value, Is.EqualTo(email));
        Assert.That(jsonToken.Claims.First(x => x.Type == "otp").Value, Is.EqualTo(otp));
        Assert.That(jsonToken.Claims.Any(x => x.Type == "registrationData"), Is.True);
    }

    [Test]
    public void ValidateOtp_ValidToken_ReturnsTrue()
    {
        // Arrange
        var email = "test@example.com";
        var otp = "123456";
        var otpType = "Registration";
        var token = _service.CreateOtpToken(email, otp, otpType);

        // Act
        var result = _service.ValidateOtp(token, otp, email, otpType);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void ValidateOtp_InvalidOtp_ReturnsFalse()
    {
        // Arrange
        var email = "test@example.com";
        var otp = "123456";
        var otpType = "Registration";
        var token = _service.CreateOtpToken(email, otp, otpType);

        // Act
        var result = _service.ValidateOtp(token, "654321", email, otpType);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void ValidateOtp_InvalidEmail_ReturnsFalse()
    {
        // Arrange
        var email = "test@example.com";
        var otp = "123456";
        var otpType = "Registration";
        var token = _service.CreateOtpToken(email, otp, otpType);

        // Act
        var result = _service.ValidateOtp(token, otp, "wrong@example.com", otpType);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void ValidateOtp_InvalidToken_ReturnsFalse()
    {
        // Act
        var result = _service.ValidateOtp("invalid_token", "123456", "test@example.com", "Registration");

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void GetRegistrationDataFromToken_ValidToken_ReturnsData()
    {
        // Arrange
        var email = "test@example.com";
        var otp = "123456";
        var otpType = "Registration";
        var data = new RegistrationDataDto { Email = email, Phone = "9876543210", PasswordHash = "hashedpassword" };
        var token = _service.CreateOtpTokenWithData(email, otp, otpType, data);

        // Act
        var result = _service.GetRegistrationDataFromToken(token);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Email, Is.EqualTo(email));
        Assert.That(result.Phone, Is.EqualTo("9876543210"));
    }

    [Test]
    public void GetRegistrationDataFromToken_TokenWithoutData_ReturnsNull()
    {
        // Arrange
        var token = _service.CreateOtpToken("test@example.com", "123456", "Registration");

        // Act
        var result = _service.GetRegistrationDataFromToken(token);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public void GetRegistrationDataFromToken_InvalidToken_ReturnsNull()
    {
        // Act
        var result = _service.GetRegistrationDataFromToken("invalid_token");

        // Assert
        Assert.That(result, Is.Null);
    }
}