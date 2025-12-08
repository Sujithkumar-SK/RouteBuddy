// ﻿using AutoMapper;
// using Kanini.RouteBuddy.Application.Dto;
// using Kanini.RouteBuddy.Data.Repositories.User;
// using Entities = Kanini.RouteBuddy.Domain.Entities;
// using Microsoft.Extensions.Logging;
// using Kanini.RouteBuddy.Common.Utility;

// namespace Kanini.RouteBuddy.Application.Services.User;

// /// <summary>
// /// Provides user-related business logic and operations.
// /// </summary>
// public class UserService : IUserService
// {
//     // private readonly ILogger<UserService> _logger;
//     // private readonly IUserRepository _userRepository;
//     // private readonly IMapper _mapper;

//     // /// <summary>
//     // /// Initializes a new instance of the <see cref="UserService"/> class.
//     // /// </summary>
//     // /// <param name="logger">Logger for logging information and errors.</param>
//     // /// <param name="userRepository">Repository for user data access.</param>
//     // /// <param name="mapper">Mapper for object transformations.</param>
//     // public UserService(ILogger<UserService> logger,
//     //     IUserRepository userRepository,
//     //     IMapper mapper)
//     // {
//     //     _logger = logger;
//     //     _userRepository = userRepository;
//     //     _mapper = mapper;
//     // }

//     // /// <summary>
//     // /// Creates a new user if the email is unique.
//     // /// </summary>
//     // /// <param name="createUserRequest">The user creation request data.</param>
//     // /// <returns>
//     // /// A <see cref="Result{Guid}"/> containing the new user's ID if successful, or an error if failed.
//     // /// </returns>
//     // public async Task<Result<Guid>> CreateUserAsync(CreateUserRequestDto createUserRequest)
//     // {
//     //     _logger.LogInformation("Starting CreateUserAsync for email: {Email}", createUserRequest.Email);

//     //     Result emailCheck = await _userRepository.IsEmailUniqueAsync(createUserRequest.Email);
//     //     if (emailCheck.IsFailure)
//     //     {
//     //         _logger.LogWarning("Email already exists: {Email}", createUserRequest.Email);
//     //         return Result.Failure<Guid>(emailCheck.Error);
//     //     }

//     //     Entities.User userEntity = _mapper.Map<Entities.User>(createUserRequest);
//     //     _logger.LogDebug("Mapped CreateUserRequestDto to User entity: {@UserEntity}", userEntity);
//     //     Result<Guid> userId = await _userRepository.CreateUserAsync(userEntity);

//     //     _logger.LogInformation("User creation initiated for email: {Email}", createUserRequest.Email);
//     //     return userId;
//     // }

//     // /// <summary>
//     // /// Retrieves a user by their unique identifier.
//     // /// </summary>
//     // /// <param name="userId">The unique identifier of the user.</param>
//     // /// <returns>
//     // /// A <see cref="Result{UserResponseDto}"/> containing user details if found, or an error if not found.
//     // /// </returns>
//     // public async Task<Result<UserResponseDto>> GetUserAsunbGetUserAsync(Guid userId)
//     // {
//     //     _logger.LogInformation("Starting GetUserAsync for userId: {UserId}", userId);
//     //     Result<Entities.User> userResult = await _userRepository.GetUserAsync(userId);

//     //     if (userResult.IsFailure)
//     //     {
//     //         _logger.LogWarning("User not found with ID: {UserId}", userId);
//     //         return Result.Failure<UserResponseDto>(userResult.Error);
//     //     }

//     //     UserResponseDto userResponse = _mapper.Map<UserResponseDto>(userResult.Value);
//     //     _logger.LogInformation("Successfully retrieved user with ID: {UserId}", userId);
//     //     return userResponse;
//     // }
// }
