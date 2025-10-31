using Kanini.RouteBuddy.Common.Utility;
using Entities = Kanini.RouteBuddy.Domain.Entities;

namespace Kanini.RouteBuddy.Data.Repositories.User;

/// <summary>
/// Provides data access operations for user entities.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Checks if the provided email is unique among users.
    /// </summary>
    /// <param name="email">The email address to check.</param>
    /// <returns>
    /// A <see cref="Result"/> indicating success if the email is unique, or failure if it already exists.
    /// </returns>
    // Task<Result> IsEmailUniqueAsync(string email);

    // /// <summary>
    // /// Creates a new user in the database.
    // /// </summary>
    // /// <param name="user">The user entity to create.</param>
    // /// <returns>
    // /// A <see cref="Result{Guid}"/> containing the new user's ID if successful, or an error if failed.
    // /// </returns>
    // Task<Result<Guid>> CreateUserAsync(Entities.User user);

    // /// <summary>
    // /// Retrieves a user by their unique identifier.
    // /// </summary>
    // /// <param name="userId">The unique identifier of the user.</param>
    // /// <returns>
    // /// A <see cref="Result{User}"/> containing the user entity if found, or an error if not found.
    // /// </returns>
    // Task<Result<Entities.User>> GetUserAsync(Guid userId);
}
