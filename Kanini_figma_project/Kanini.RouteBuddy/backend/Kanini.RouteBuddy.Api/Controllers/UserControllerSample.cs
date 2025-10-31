﻿using Kanini.RouteBuddy.Application.Dto;
using Kanini.RouteBuddy.Application.Services.User;
using Kanini.RouteBuddy.Common.Utility;
using Microsoft.AspNetCore.Mvc;

namespace Kanini.RouteBuddy.Api.Controllers;

/// <summary>
/// Controller for managing user-related operations.
/// </summary>
[Route("api/users")]
[ApiController]
public class UserControllerSample : ControllerBase
{
    // private readonly IUserService _userService;
    // private readonly ILogger<UserControllerSample> _logger;

    // /// <summary>
    // /// Initializes a new instance of the <see cref="UserController"/> class.
    // /// </summary>
    // /// <param name="userService">Service for user operations.</param>
    // /// <param name="logger">Logger instance.</param>
    // public UserControllerSample(IUserService userService, ILogger<UserControllerSample> logger)
    // {
    //     _userService = userService;
    //     _logger = logger;
    // }

    // /// <summary>
    // /// Creates a new user.
    // /// </summary>
    // /// <param name="createUserRequest">The user creation request data.</param>
    // /// <returns>
    // /// Returns <see cref="OkObjectResult"/> with the created user's ID if successful,
    // /// otherwise <see cref="BadRequestObjectResult"/> with error details.
    // /// </returns>
    // [HttpPost("create")]
    // public async Task<IActionResult> CreateUser([FromBody] CreateUserRequestDto createUserRequest)
    // {
    //     _logger.LogInformation("Attempting to create user with email: {Email}", createUserRequest.Email);
    //     Result<Guid> result = await _userService.CreateUserAsync(createUserRequest);

    //     if (result.IsSuccess)
    //     {
    //         _logger.LogInformation("User created successfully with UserId: {UserId}", result.Value);
    //         return Ok(new { UserId = result.Value });
    //     }

    //     _logger.LogError("Failed to create user: {ErrorMessage}", result.Error.Description);
    //     return BadRequest(result.Error);
    // }

    // /// <summary>
    // /// Retrieves a user by their unique identifier.
    // /// </summary>
    // /// <param name="userId">The unique identifier of the user.</param>
    // /// <returns>
    // /// Returns <see cref="OkObjectResult"/> with user details if found,
    // /// otherwise <see cref="NotFoundObjectResult"/> with error details.
    // /// </returns>
    // [HttpGet("{userId:guid}")]
    // public async Task<IActionResult> GetUser(Guid userId)
    // {
    //     _logger.LogInformation("Attempting to retrieve user with ID: {UserId}", userId);
    //     Result<UserResponseDto> result = await _userService.GetUserAsunbGetUserAsync(userId);
    //     if (result.IsSuccess)
    //     {
    //         _logger.LogInformation("User retrieved successfully with ID: {UserId}", userId);
    //         return Ok(result.Value);
    //     }
    //     _logger.LogWarning("Failed to retrieve user with ID: {UserId}, Error: {ErrorMessage}", userId, result.Error.Description);
    //     return NotFound(result.Error);
    // }
}
