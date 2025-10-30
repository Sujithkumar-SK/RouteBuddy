using Kanini.RouteBuddy.Application.Dto;
using Kanini.RouteBuddy.Common.Utility;

namespace Kanini.RouteBuddy.Application.Services.User;

/// <summary>
/// Defines user-related business operations.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="createUserRequest">The user creation request data.</param>
    /// <returns>
    /// A <see cref="Result{Guid}"/> containing the new user's ID if successful, or an error if failed.
    /// </returns>
    Task<Result<Guid>> CreateUserAsync(CreateUserRequestDto createUserRequest);

    /// <summary>
    /// Retrieves a user by their unique identifier.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>
    /// A <see cref="Result{UserResponseDto}"/> containing user details if found, or an error if not found.
    /// </returns>
    Task<Result<UserResponseDto>> GetUserAsunbGetUserAsync(Guid userId);
}
