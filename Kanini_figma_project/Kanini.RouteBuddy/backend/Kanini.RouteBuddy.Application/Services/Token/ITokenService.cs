using Kanini.RouteBuddy.Application.Dto.Auth;
using Kanini.RouteBuddy.Common.Utility;
using UserEntity = Kanini.RouteBuddy.Domain.Entities.User;

namespace Kanini.RouteBuddy.Application.Services.Token;

public interface ITokenService
{
    string GenerateAccessToken(UserEntity user);
    Task<Result<string>> GenerateRefreshTokenAsync(int userId);
    Task<Result<AuthResponseDto>> RefreshTokenAsync(string refreshToken);
    Task<UserEntity?> ValidateUserAsync(string email, string password);
    Task<Result<bool>> RevokeRefreshTokenAsync(string refreshToken);
    Task<Result<bool>> RevokeAllUserTokensAsync(int userId);
}