using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Kanini.RouteBuddy.Application.Dto.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Kanini.RouteBuddy.Application.Services.Auth;

public class JwtOtpService : IJwtOtpService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<JwtOtpService> _logger;

    public JwtOtpService(IConfiguration configuration, ILogger<JwtOtpService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public string GenerateOtp()
    {
        var otp = new Random().Next(100000, 999999).ToString();
        _logger.LogInformation("Generated OTP: {Otp}", otp);
        return otp;
    }

    public string CreateOtpToken(string email, string otp, string otpType)
    {
        var secretKey = _configuration["TokenKey"];
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim("email", email),
            new Claim("otp", otp),
            new Claim("otpType", otpType),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(5),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string CreateOtpTokenWithData(string email, string otp, string otpType, RegistrationDataDto data)
    {
        var secretKey = _configuration["TokenKey"];
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim("email", email),
            new Claim("otp", otp),
            new Claim("otpType", otpType),
            new Claim("registrationData", JsonSerializer.Serialize(data)),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(5),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public bool ValidateOtp(string jwtToken, string inputOtp, string email, string otpType)
    {
        try
        {
            var secretKey = _configuration["TokenKey"];
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(secretKey!);

            tokenHandler.ValidateToken(jwtToken, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jsonToken = tokenHandler.ReadJwtToken(jwtToken);
            var emailClaim = jsonToken.Claims.FirstOrDefault(x => x.Type == "email")?.Value;
            var otpClaim = jsonToken.Claims.FirstOrDefault(x => x.Type == "otp")?.Value;
            var otpTypeClaim = jsonToken.Claims.FirstOrDefault(x => x.Type == "otpType")?.Value;

            return emailClaim == email && otpClaim == inputOtp && otpTypeClaim == otpType;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "OTP validation failed");
            return false;
        }
    }

    public RegistrationDataDto? GetRegistrationDataFromToken(string jwtToken)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jsonToken = tokenHandler.ReadJwtToken(jwtToken);
            var dataClaim = jsonToken.Claims.FirstOrDefault(x => x.Type == "registrationData")?.Value;

            if (string.IsNullOrEmpty(dataClaim))
                return null;

            return JsonSerializer.Deserialize<RegistrationDataDto>(dataClaim);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to extract registration data from token");
            return null;
        }
    }
}
