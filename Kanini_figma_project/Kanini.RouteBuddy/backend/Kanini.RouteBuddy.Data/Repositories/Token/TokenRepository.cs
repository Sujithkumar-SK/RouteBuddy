using Kanini.RouteBuddy.Data.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserEntity = Kanini.RouteBuddy.Domain.Entities.User;

namespace Kanini.RouteBuddy.Data.Repositories.Token;

public class TokenRepository : ITokenRepository
{
    private readonly RouteBuddyDatabaseContext _context;
    private readonly ILogger<TokenRepository> _logger;

    public TokenRepository(RouteBuddyDatabaseContext context, ILogger<TokenRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<UserEntity?> GetUserByIdAsync(int userId)
    {
        return await _context.Users
            .Where(u => u.UserId == userId && u.IsActive)
            .FirstOrDefaultAsync();
    }

    public async Task<UserEntity?> GetUserByEmailAsync(string email)
    {
        return await _context.Users
            .Where(u => u.Email == email && u.IsActive)
            .FirstOrDefaultAsync();
    }

    public async Task<UserEntity?> ValidateUserCredentialsAsync(string email, string password)
    {
        var user = await GetUserByEmailAsync(email);
        
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return null;

        return user;
    }
}