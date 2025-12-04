using Kanini.RouteBuddy.Common;
using Kanini.RouteBuddy.Common.Utility;
using Kanini.RouteBuddy.Data.DatabaseContext;
using Kanini.RouteBuddy.Data.Infrastructure;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RefreshTokenEntity = Kanini.RouteBuddy.Domain.Entities.RefreshToken;
using UserEntity = Kanini.RouteBuddy.Domain.Entities.User;

namespace Kanini.RouteBuddy.Data.Repositories.Token;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly RouteBuddyDatabaseContext _context;
    private readonly IDbReader _dbReader;
    private readonly string _connectionString;
    private readonly ILogger<RefreshTokenRepository> _logger;

    public RefreshTokenRepository(
        RouteBuddyDatabaseContext context,
        IDbReader dbReader,
        IConfiguration configuration,
        ILogger<RefreshTokenRepository> logger)
    {
        _context = context;
        _dbReader = dbReader;
        _connectionString = configuration.GetConnectionString("DatabaseConnectionString")!;
        _logger = logger;
    }

    // New simplified methods for AuthService
    public async Task<RefreshTokenEntity> CreateRefreshTokenAsync(RefreshTokenEntity refreshToken)
    {
        await _context.RefreshTokens.AddAsync(refreshToken);
        await _context.SaveChangesAsync();
        return refreshToken;
    }

    public async Task<UserEntity?> GetUserByRefreshTokenAsync(string token)
    {
        try
        {
            var parameters = new[] { new SqlParameter("@RefreshToken", token) };
            var result = await _dbReader.ExecuteStoredProcedureAsync(MagicStrings.StoredProcedures.GetUserByRefreshToken, parameters);
            
            if (result.Rows.Count == 0)
                return null;
                
            var row = result.Rows[0];
            return new UserEntity
            {
                UserId = Convert.ToInt32(row["UserId"]),
                Email = row["Email"].ToString()!,
                PasswordHash = row["PasswordHash"].ToString()!,
                Phone = row["Phone"].ToString()!,
                Role = (Domain.Enums.UserRole)Convert.ToInt32(row["Role"]),
                IsEmailVerified = Convert.ToBoolean(row["IsEmailVerified"]),
                IsActive = Convert.ToBoolean(row["IsActive"]),
                CreatedBy = row["CreatedBy"].ToString()!,
                CreatedOn = Convert.ToDateTime(row["CreatedOn"])
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by refresh token");
            throw;
        }
    }

    public async Task RevokeRefreshTokenAsync(string token)
    {
        var refreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token);
        if (refreshToken != null)
        {
            refreshToken.IsRevoked = true;
            refreshToken.RevokedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    // WRITE: Using EF Core (Rule #2)
    public async Task<Result<RefreshTokenEntity>> CreateAsync(RefreshTokenEntity refreshToken)
    {
        try
        {
            _logger.LogInformation("Creating refresh token for UserId: {UserId}", refreshToken.UserId);

            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Refresh token created successfully");
            return Result.Success(refreshToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating refresh token");
            return Result.Failure<RefreshTokenEntity>(
                Error.Failure("RefreshToken.CreateFailed", MagicStrings.ErrorMessages.DatabaseError));
        }
    }

    // READ: Using ADO.NET (Rule #2)
    public async Task<Result<RefreshTokenEntity?>> GetByTokenAsync(string token)
    {
        try
        {
            _logger.LogInformation("Getting refresh token");

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                "SELECT RefreshTokenId, Token, UserId, ExpiresAt, IsRevoked, RevokedAt, ReplacedByToken, CreatedBy, CreatedOn FROM RefreshTokens WHERE Token = @Token",
                connection);
            command.Parameters.AddWithValue("@Token", token);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                var refreshToken = new RefreshTokenEntity
                {
                    RefreshTokenId = reader.GetInt32(0),
                    Token = reader.GetString(1),
                    UserId = reader.GetInt32(2),
                    ExpiresAt = reader.GetDateTime(3),
                    IsRevoked = reader.GetBoolean(4),
                    RevokedAt = reader.IsDBNull(5) ? null : reader.GetDateTime(5),
                    ReplacedByToken = reader.IsDBNull(6) ? null : reader.GetString(6),
                    CreatedBy = reader.GetString(7),
                    CreatedOn = reader.GetDateTime(8)
                };

                return Result.Success<RefreshTokenEntity?>(refreshToken);
            }

            return Result.Success<RefreshTokenEntity?>(null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting refresh token");
            return Result.Failure<RefreshTokenEntity?>(
                Error.Failure("RefreshToken.GetFailed", MagicStrings.ErrorMessages.DatabaseError));
        }
    }

    // WRITE: Using EF Core (Rule #2)
    public async Task<Result<bool>> RevokeAsync(string token)
    {
        try
        {
            _logger.LogInformation("Revoking refresh token");

            var refreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token);
            if (refreshToken == null)
                return Result.Failure<bool>(Error.NotFound("RefreshToken.NotFound", "Refresh token not found"));

            refreshToken.IsRevoked = true;
            refreshToken.RevokedAt = DateTime.UtcNow;
            refreshToken.UpdatedOn = DateTime.UtcNow;
            refreshToken.UpdatedBy = "System";

            await _context.SaveChangesAsync();

            _logger.LogInformation("Refresh token revoked successfully");
            return Result.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking refresh token");
            return Result.Failure<bool>(
                Error.Failure("RefreshToken.RevokeFailed", MagicStrings.ErrorMessages.DatabaseError));
        }
    }

    // WRITE: Using EF Core (Rule #2)
    public async Task<Result<bool>> RevokeAllByUserIdAsync(int userId)
    {
        try
        {
            _logger.LogInformation("Revoking all refresh tokens for UserId: {UserId}", userId);

            var tokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == userId && !rt.IsRevoked)
                .ToListAsync();

            foreach (var token in tokens)
            {
                token.IsRevoked = true;
                token.RevokedAt = DateTime.UtcNow;
                token.UpdatedOn = DateTime.UtcNow;
                token.UpdatedBy = "System";
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("All refresh tokens revoked for UserId: {UserId}", userId);
            return Result.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking all refresh tokens for UserId: {UserId}", userId);
            return Result.Failure<bool>(
                Error.Failure("RefreshToken.RevokeAllFailed", MagicStrings.ErrorMessages.DatabaseError));
        }
    }

    // WRITE: Using EF Core (Rule #2)
    public async Task<Result<bool>> DeleteExpiredTokensAsync()
    {
        try
        {
            _logger.LogInformation("Deleting expired refresh tokens");

            var expiredTokens = await _context.RefreshTokens
                .Where(rt => rt.ExpiresAt < DateTime.UtcNow)
                .ToListAsync();

            _context.RefreshTokens.RemoveRange(expiredTokens);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Deleted {Count} expired refresh tokens", expiredTokens.Count);
            return Result.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting expired refresh tokens");
            return Result.Failure<bool>(
                Error.Failure("RefreshToken.DeleteExpiredFailed", MagicStrings.ErrorMessages.DatabaseError));
        }
    }
}
