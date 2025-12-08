using Kanini.RouteBuddy.Common.Utility;
using Entities = Kanini.RouteBuddy.Domain.Entities;

namespace Kanini.RouteBuddy.Data.Repositories.User;

public interface IUserRepository
{
    Task<bool> ExistsByEmailAsync(string email);
    Task<bool> ExistsByPhoneAsync(string phone);
    Task<Entities.User> AddAsync(Entities.User user);
    Task<Entities.User> CreateUserAsync(Entities.User user);
    Task<Entities.User?> GetByIdAsync(int userId);
    Task<Entities.User?> GetUserByIdAsync(int userId);
    Task<Entities.User?> GetUserByEmailAsync(string email);
    Task UpdateUserPasswordAsync(int userId, string newPasswordHash);
    Task UpdateLastLoginAsync(int userId);
    Task UpdateUserActiveStatusAsync(int userId, bool isActive);
    
    // Admin user management methods
    Task<IEnumerable<Entities.User>> GetAllUsersAsync(int pageNumber, int pageSize);
    Task<IEnumerable<Entities.User>> FilterUsersAsync(string? searchTerm, string? role, bool? isActive);
    Task<bool> UpdateUserStatusAsync(int userId, bool isActive);
}
