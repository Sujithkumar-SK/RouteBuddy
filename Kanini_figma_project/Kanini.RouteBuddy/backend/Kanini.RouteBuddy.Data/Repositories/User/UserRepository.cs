using Kanini.RouteBuddy.Common;
using Kanini.RouteBuddy.Data.DatabaseContext;
using Kanini.RouteBuddy.Data.Infrastructure;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Entities = Kanini.RouteBuddy.Domain.Entities;

namespace Kanini.RouteBuddy.Data.Repositories.User;

public class UserRepository : IUserRepository
{
    private readonly RouteBuddyDatabaseContext _context;
    private readonly IDbReader _dbReader;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(RouteBuddyDatabaseContext context, IDbReader dbReader, ILogger<UserRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbReader = dbReader ?? throw new ArgumentNullException(nameof(dbReader));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // READ: Using ADO.NET with stored procedure (Rule #2, #4)
    public async Task<bool> ExistsByEmailAsync(string email)
    {
        try
        {
            _logger.LogInformation("Checking if email exists: {Email}", email);
            
            var parameters = new[] { new SqlParameter("@Email", email) };
            var result = await _dbReader.ExecuteScalarAsync<int>(MagicStrings.StoredProcedures.CheckUserExistsByEmail, parameters);
            
            return result > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking email existence for {Email}", email);
            throw;
        }
    }

    // READ: Using ADO.NET with stored procedure (Rule #2, #4)
    public async Task<bool> ExistsByPhoneAsync(string phone)
    {
        try
        {
            _logger.LogInformation("Checking if phone exists: {Phone}", phone);
            
            var parameters = new[] { new SqlParameter("@Phone", phone) };
            var result = await _dbReader.ExecuteScalarAsync<int>(MagicStrings.StoredProcedures.CheckUserExistsByPhone, parameters);
            
            return result > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking phone existence for {Phone}", phone);
            throw;
        }
    }

    // WRITE: Using EF Core (Rule #2)
    public async Task<Entities.User> CreateUserAsync(Entities.User user)
    {
        return await AddAsync(user);
    }

    // WRITE: Using EF Core (Rule #2)
    public async Task<Entities.User> AddAsync(Entities.User user)
    {
        try
        {
            _logger.LogInformation("Adding new user with email: {Email}", user.Email);
            
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("User added successfully with ID: {UserId}", user.UserId);
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding user with email: {Email}", user.Email);
            throw;
        }
    }

    // READ: Using ADO.NET with stored procedure (Rule #2, #4)
    public async Task<Entities.User?> GetUserByIdAsync(int userId)
    {
        return await GetByIdAsync(userId);
    }

    // READ: Using ADO.NET with stored procedure (Rule #2, #4)
    public async Task<Entities.User?> GetByIdAsync(int userId)
    {
        try
        {
            _logger.LogInformation("Getting user by ID: {UserId}", userId);
            
            var parameters = new[] { new SqlParameter("@UserId", userId) };
            var result = await _dbReader.ExecuteStoredProcedureAsync(MagicStrings.StoredProcedures.GetUserById, parameters);
            
            if (result.Rows.Count == 0)
                return null;
                
            var row = result.Rows[0];
            return new Entities.User
            {
                UserId = Convert.ToInt32(row["UserId"]),
                Email = row["Email"].ToString()!,
                PasswordHash = row["PasswordHash"].ToString()!,
                Phone = row["Phone"].ToString()!,
                Role = (Kanini.RouteBuddy.Domain.Enums.UserRole)Convert.ToInt32(row["Role"]),
                IsEmailVerified = Convert.ToBoolean(row["IsEmailVerified"]),
                IsActive = Convert.ToBoolean(row["IsActive"]),
                CreatedBy = row["CreatedBy"].ToString()!,
                CreatedOn = Convert.ToDateTime(row["CreatedOn"])
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by ID: {UserId}", userId);
            throw;
        }
    }

    // READ: Using ADO.NET with stored procedure (Rule #2, #4)
    public async Task<Entities.User?> GetUserByEmailAsync(string email)
    {
        try
        {
            _logger.LogInformation("Getting user by email: {Email}", email);
            
            var parameters = new[] { new SqlParameter("@Email", email) };
            var result = await _dbReader.ExecuteStoredProcedureAsync(MagicStrings.StoredProcedures.GetUserByEmail, parameters);
            
            if (result.Rows.Count == 0)
                return null;
                
            var row = result.Rows[0];
            return new Entities.User
            {
                UserId = Convert.ToInt32(row["UserId"]),
                Email = row["Email"].ToString()!,
                PasswordHash = row["PasswordHash"].ToString()!,
                Phone = row["Phone"].ToString()!,
                Role = (Kanini.RouteBuddy.Domain.Enums.UserRole)Convert.ToInt32(row["Role"]),
                IsEmailVerified = Convert.ToBoolean(row["IsEmailVerified"]),
                IsActive = Convert.ToBoolean(row["IsActive"]),
                CreatedBy = row["CreatedBy"].ToString()!,
                CreatedOn = Convert.ToDateTime(row["CreatedOn"])
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by email: {Email}", email);
            throw;
        }
    }

    // WRITE: Using EF Core (Rule #2)
    public async Task UpdateUserPasswordAsync(int userId, string newPasswordHash)
    {
        try
        {
            _logger.LogInformation("Updating password for userId {UserId}", userId);

            var exists = await _context.Users.AnyAsync(u => u.UserId == userId);
            if (!exists)
            {
                _logger.LogWarning("Attempted to update password for non-existent userId {UserId}", userId);
                throw new Exception("User not found");
            }

            var utcNow = DateTime.UtcNow;
            var rows = await _context.Database.ExecuteSqlInterpolatedAsync(
                $"UPDATE Users SET PasswordHash = {newPasswordHash}, UpdatedOn = {utcNow} WHERE UserId = {userId}");

            if (rows == 0)
            {
                _logger.LogWarning("No rows affected while updating password (UserId: {UserId})", userId);
                throw new Exception("Password update failed");
            }

            _logger.LogInformation("Password updated successfully for userId {UserId}", userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating password for userId {UserId}", userId);
            throw;
        }
    }

    public async Task UpdateLastLoginAsync(int userId)
    {
        try
        {
            _logger.LogInformation("Updating last login for userId {UserId}", userId);

            var utcNow = DateTime.UtcNow;
            await _context.Database.ExecuteSqlInterpolatedAsync(
                $"UPDATE Users SET LastLogin = {utcNow}, UpdatedOn = {utcNow} WHERE UserId = {userId}");

            _logger.LogInformation("Last login updated successfully for userId {UserId}", userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating last login for userId {UserId}", userId);
            throw;
        }
    }

    public async Task UpdateUserActiveStatusAsync(int userId, bool isActive)
    {
        try
        {
            _logger.LogInformation("Updating active status for userId {UserId} to {IsActive}", userId, isActive);

            var utcNow = DateTime.UtcNow;
            await _context.Database.ExecuteSqlInterpolatedAsync(
                $"UPDATE Users SET IsActive = {isActive}, UpdatedBy = {"Admin"}, UpdatedOn = {utcNow} WHERE UserId = {userId}");

            _logger.LogInformation("Active status updated successfully for userId {UserId}", userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating active status for userId {UserId}", userId);
            throw;
        }
    }

    // Admin user management methods
    public async Task<IEnumerable<Entities.User>> GetAllUsersAsync(int pageNumber, int pageSize)
    {
        try
        {
            _logger.LogInformation("Getting all users - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);
            
            var users = await _context.Users
                .OrderByDescending(u => u.CreatedOn)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
                
            return users;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all users");
            throw;
        }
    }

    public async Task<IEnumerable<Entities.User>> FilterUsersAsync(string? searchTerm, string? role, bool? isActive)
    {
        try
        {
            _logger.LogInformation("Filtering users - Search: {SearchTerm}, Role: {Role}, Active: {IsActive}", 
                searchTerm, role, isActive);
            
            var query = _context.Users.AsQueryable();
            
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(u => u.Email.Contains(searchTerm) || u.Phone.Contains(searchTerm));
            }
            
            if (!string.IsNullOrEmpty(role) && Enum.TryParse<Kanini.RouteBuddy.Domain.Enums.UserRole>(role, out var userRole))
            {
                query = query.Where(u => u.Role == userRole);
            }
            
            if (isActive.HasValue)
            {
                query = query.Where(u => u.IsActive == isActive.Value);
            }
            
            var users = await query.OrderByDescending(u => u.CreatedOn).ToListAsync();
            return users;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error filtering users");
            throw;
        }
    }

    public async Task<bool> UpdateUserStatusAsync(int userId, bool isActive)
    {
        try
        {
            _logger.LogInformation("Updating user status for userId {UserId} to {IsActive}", userId, isActive);

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User not found for userId {UserId}", userId);
                return false;
            }

            user.IsActive = isActive;
            user.UpdatedBy = "Admin";
            user.UpdatedOn = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("User status updated successfully for userId {UserId}", userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user status for userId {UserId}", userId);
            return false;
        }
    }
}
