using Kanini.RouteBuddy.Common.Utility;
using RefreshTokenEntity = Kanini.RouteBuddy.Domain.Entities.RefreshToken;
using UserEntity = Kanini.RouteBuddy.Domain.Entities.User;

namespace Kanini.RouteBuddy.Data.Repositories.Token;

public interface IRefreshTokenRepository
{
    Task<RefreshTokenEntity> CreateRefreshTokenAsync(RefreshTokenEntity refreshToken);
    Task<UserEntity?> GetUserByRefreshTokenAsync(string token);
    Task RevokeRefreshTokenAsync(string token);
    Task<Result<RefreshTokenEntity>> CreateAsync(RefreshTokenEntity refreshToken);
    Task<Result<RefreshTokenEntity?>> GetByTokenAsync(string token);
    Task<Result<bool>> RevokeAsync(string token);
    Task<Result<bool>> RevokeAllByUserIdAsync(int userId);
    Task<Result<bool>> DeleteExpiredTokensAsync();
}
