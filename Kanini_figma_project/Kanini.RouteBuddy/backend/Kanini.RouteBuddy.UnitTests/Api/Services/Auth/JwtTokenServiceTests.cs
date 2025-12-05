using Kanini.RouteBuddy.Application.Services.Auth;
using Kanini.RouteBuddy.Domain.Entities;
using Kanini.RouteBuddy.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Kanini.RouteBuddy.UnitTests.Api.Services.Auth;

[TestFixture]
public class JwtTokenServiceTests
{
    private Mock<IConfiguration> _mockConfiguration;
    private Mock<ILogger<JwtTokenService>> _mockLogger;
    private JwtTokenService _service;
    private User _testUser;

    [SetUp]
    public void Setup()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        _mockLogger = new Mock<ILogger<JwtTokenService>>();
        
        _mockConfiguration.Setup(x => x["TokenKey"]).Returns("ThisIsAVeryLongSecretKeyForJWTTokenGeneration123456789");
        
        _service = new JwtTokenService(_mockConfiguration.Object, _mockLogger.Object);
        
        _testUser = new User
        {
            UserId = 1,
            Email = "test@example.com",
            Role = UserRole.Customer
        };
    }

    [Test]
    public void GenerateAccessToken_ValidUser_ReturnsValidJWT()
    {
        // Act
        var token = _service.GenerateAccessToken(_testUser);

        // Assert
        Assert.That(token, Is.Not.Null);
        Assert.That(token, Is.Not.Empty);

        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadJwtToken(token);

        Assert.That(jsonToken.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value, Is.EqualTo("1"));
        Assert.That(jsonToken.Claims.First(x => x.Type == ClaimTypes.Email).Value, Is.EqualTo("test@example.com"));
        Assert.That(jsonToken.Claims.First(x => x.Type == ClaimTypes.Role).Value, Is.EqualTo("Customer"));
    }

    [Test]
    public void GenerateRefreshToken_ReturnsValidToken()
    {
        // Act
        var token = _service.GenerateRefreshToken();

        // Assert
        Assert.That(token, Is.Not.Null);
        Assert.That(token, Is.Not.Empty);
        Assert.That(token.Length, Is.GreaterThan(0));
    }

    [Test]
    public void GenerateRefreshToken_GeneratesUniqueTokens()
    {
        // Act
        var token1 = _service.GenerateRefreshToken();
        var token2 = _service.GenerateRefreshToken();

        // Assert
        Assert.That(token1, Is.Not.EqualTo(token2));
    }

    [Test]
    public void GetAccessTokenExpiry_ReturnsCorrectExpiry()
    {
        // Act
        var expiry = _service.GetAccessTokenExpiry();

        // Assert
        var expectedExpiry = DateTime.UtcNow.AddMinutes(30);
        Assert.That(expiry, Is.EqualTo(expectedExpiry).Within(TimeSpan.FromSeconds(1)));
    }

    [Test]
    public void GetRefreshTokenExpiry_ReturnsCorrectExpiry()
    {
        // Act
        var expiry = _service.GetRefreshTokenExpiry();

        // Assert
        var expectedExpiry = DateTime.UtcNow.AddDays(7);
        Assert.That(expiry, Is.EqualTo(expectedExpiry).Within(TimeSpan.FromSeconds(1)));
    }

    [Test]
    public void GenerateAccessToken_TokenHasCorrectExpiry()
    {
        // Act
        var token = _service.GenerateAccessToken(_testUser);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadJwtToken(token);
        
        var expectedExpiry = DateTime.UtcNow.AddMinutes(30);
        Assert.That(jsonToken.ValidTo, Is.EqualTo(expectedExpiry).Within(TimeSpan.FromSeconds(1)));
    }

    [Test]
    public void GenerateAccessToken_TokenHasJti()
    {
        // Act
        var token = _service.GenerateAccessToken(_testUser);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadJwtToken(token);
        
        var jtiClaim = jsonToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti);
        Assert.That(jtiClaim, Is.Not.Null);
        Assert.That(jtiClaim.Value, Is.Not.Empty);
    }
}