using Kanini.RouteBuddy.Common.Errors;
using Kanini.RouteBuddy.Common.Utility;
using Kanini.RouteBuddy.Data.DatabaseContext;
using Microsoft.Extensions.Logging;
using Entities = Kanini.RouteBuddy.Domain.Entities;

namespace Kanini.RouteBuddy.Data.Repositories.User;

/// <summary>
/// Repository for user data access and persistence.
/// </summary>
public class UserRepository : IUserRepository
{
    // private readonly RouteBuddyDatabaseContext _context;
    // private readonly ILogger<UserRepository> _logger;

    // /// <summary>
    // /// Initializes a new instance of the <see cref="UserRepository"/> class.
    // /// </summary>
    // /// <param name="context">The database context for RouteBuddy.</param>
    // /// <param name="logger">Logger for logging information and errors.</param>
    // public UserRepository(RouteBuddyDatabaseContext context, ILogger<UserRepository> logger)
    // {
    //     _context = context;
    //     _logger = logger;
    // }

    // /// <summary>
    // /// Creates a new user in the database.
    // /// </summary>
    // /// <param name="user">The user entity to create.</param>
    // /// <returns>
    // /// A <see cref="Result{Guid}"/> containing the new user's ID if successful, or an error if failed.
    // /// </returns>
    // public async Task<Result<Guid>> CreateUserAsync(Entities.User user)
    // {
    //     _logger.LogInformation("Creating a new user with email: {Email}", user.Email);
    //     await _context.Users.AddAsync(user);
    //     await _context.SaveChangesAsync();
    //     _logger.LogInformation("User created with ID: {UserId}", user.Id);
    //     return user.Id;
    // }

    // /// <summary>
    // /// Retrieves a user by their unique identifier.
    // /// </summary>
    // /// <param name="userId">The unique identifier of the user.</param>
    // /// <returns>
    // /// A <see cref="Result{User}"/> containing the user entity if found, or an error if not found.
    // /// </returns>
    // public async Task<Result<Entities.User>> GetUserAsync(Guid userId)
    // {
    //     _logger.LogInformation("Attempting to retrieve user with ID: {UserId}", userId);
    //     var user = await _context.Users.FindAsync(userId);

    //     if (user is null)
    //     {
    //         _logger.LogWarning("User not found with ID: {UserId}", userId);
    //         return Result.Failure<Entities.User>(UserErrors.NotFound(userId));
    //     }

    //     _logger.LogInformation("User retrieved with ID: {UserId}", userId);
    //     return user;
    // }

    // /// <summary>
    // /// Checks if the provided email is unique among users.
    // /// </summary>
    // /// <param name="email">The email address to check.</param>
    // /// <returns>
    // /// A <see cref="Result"/> indicating success if the email is unique, or failure if it already exists.
    // /// </returns>
    // public Task<Result> IsEmailUniqueAsync(string email)
    // {
    //     _logger.LogInformation("Checking if email is unique: {Email}", email);
    //     bool exists = _context.Users.Any(u => u.Email == email);
    //     if (exists)
    //     {
    //         _logger.LogWarning("Email already exists: {Email}", email);
    //         return Task.FromResult(Result.Failure(UserErrors.EmailAlreadyExists(email)));
    //     }
    //     _logger.LogInformation("Email is unique: {Email}", email);
    //     return Task.FromResult(Result.Success());
    // }
}
