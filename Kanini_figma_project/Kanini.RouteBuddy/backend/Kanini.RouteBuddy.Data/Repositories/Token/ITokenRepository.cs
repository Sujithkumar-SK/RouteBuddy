using UserEntity = Kanini.RouteBuddy.Domain.Entities.User;

namespace Kanini.RouteBuddy.Data.Repositories.Token;

public interface ITokenRepository
{
    Task<UserEntity?> GetUserByIdAsync(int userId);
    Task<UserEntity?> GetUserByEmailAsync(string email);
    Task<UserEntity?> ValidateUserCredentialsAsync(string email, string password);
}