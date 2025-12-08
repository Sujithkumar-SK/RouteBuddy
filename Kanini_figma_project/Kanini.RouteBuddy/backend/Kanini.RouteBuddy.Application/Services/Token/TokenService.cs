using Kanini.RouteBuddy.Application.Dto.Auth;
using Kanini.RouteBuddy.Application.Services.Token;
using Kanini.RouteBuddy.Common;
using Kanini.RouteBuddy.Common.Utility;
using Kanini.RouteBuddy.Data.Repositories.Token;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using RefreshTokenEntity = Kanini.RouteBuddy.Domain.Entities.RefreshToken;
using UserEntity = Kanini.RouteBuddy.Domain.Entities.User;

namespace Kanini.RouteBuddy.Application.Services.Token;

public class TokenService : ITokenService
{
    private readonly IConfiguration _config;
    private readonly SymmetricSecurityKey _key;
    private readonly ITokenRepository _tokenRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ILogger<TokenService> _logger;

    public TokenService(
        IConfiguration config,
        ITokenRepository tokenRepository,
        IRefreshTokenRepository refreshTokenRepository,
        ILogger<TokenService> logger)
    {
        _config = config;
        _tokenRepository = tokenRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _logger = logger;
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]!));
    }

    public string GenerateAccessToken(UserEntity user)
    {
        try
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var cred = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);
            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = cred,
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var myToken = tokenHandler.CreateToken(tokenDescription);
            return tokenHandler.WriteToken(myToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating access token");
            throw;
        }
    }

    public async Task<Result<string>> GenerateRefreshTokenAsync(int userId)
    {
        try
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            var token = Convert.ToBase64String(randomBytes);

            var refreshToken = new RefreshTokenEntity
            {
                Token = token,
                UserId = userId,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false,
                CreatedBy = "System",
                CreatedOn = DateTime.UtcNow
            };

            var result = await _refreshTokenRepository.CreateAsync(refreshToken);
            if (result.IsFailure)
                return Result.Failure<string>(result.Error);

            return Result.Success(token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating refresh token");
            return Result.Failure<string>(
                Error.Failure("RefreshToken.GenerateFailed", MagicStrings.ErrorMessages.UnexpectedError));
        }
    }

    public async Task<Result<AuthResponseDto>> RefreshTokenAsync(string refreshToken)
    {
        try
        {
            var tokenResult = await _refreshTokenRepository.GetByTokenAsync(refreshToken);
            if (tokenResult.IsFailure || tokenResult.Value == null)
                return Result.Failure<AuthResponseDto>(
                    Error.NotFound("RefreshToken.Invalid", "Invalid refresh token"));

            var token = tokenResult.Value;

            if (token.IsRevoked)
                return Result.Failure<AuthResponseDto>(
                    Error.Failure("RefreshToken.Revoked", "Refresh token has been revoked"));

            if (token.ExpiresAt < DateTime.UtcNow)
                return Result.Failure<AuthResponseDto>(
                    Error.Failure("RefreshToken.Expired", "Refresh token has expired"));

            var user = await _tokenRepository.GetUserByIdAsync(token.UserId);
            if (user == null)
                return Result.Failure<AuthResponseDto>(
                    Error.NotFound("User.NotFound", "User not found"));

            await _refreshTokenRepository.RevokeAsync(refreshToken);

            var newAccessToken = GenerateAccessToken(user);
            var newRefreshTokenResult = await GenerateRefreshTokenAsync(user.UserId);

            if (newRefreshTokenResult.IsFailure)
                return Result.Failure<AuthResponseDto>(newRefreshTokenResult.Error);

            var response = new AuthResponseDto
            {
                UserId = user.UserId,
                Email = user.Email,
                Role = user.Role,
                AccessToken = newAccessToken,
                RefreshToken = newRefreshTokenResult.Value,
                AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(15),
                RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7)
            };

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing token");
            return Result.Failure<AuthResponseDto>(
                Error.Failure("RefreshToken.RefreshFailed", MagicStrings.ErrorMessages.UnexpectedError));
        }
    }

    public async Task<UserEntity?> ValidateUserAsync(string email, string password)
    {
        try
        {
            return await _tokenRepository.ValidateUserCredentialsAsync(email, password);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating user");
            throw;
        }
    }

    public async Task<Result<bool>> RevokeRefreshTokenAsync(string refreshToken)
    {
        try
        {
            return await _refreshTokenRepository.RevokeAsync(refreshToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking refresh token");
            return Result.Failure<bool>(
                Error.Failure("RefreshToken.RevokeFailed", MagicStrings.ErrorMessages.UnexpectedError));
        }
    }

    public async Task<Result<bool>> RevokeAllUserTokensAsync(int userId)
    {
        try
        {
            return await _refreshTokenRepository.RevokeAllByUserIdAsync(userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking all user tokens");
            return Result.Failure<bool>(
                Error.Failure("RefreshToken.RevokeAllFailed", MagicStrings.ErrorMessages.UnexpectedError));
        }
    }
}