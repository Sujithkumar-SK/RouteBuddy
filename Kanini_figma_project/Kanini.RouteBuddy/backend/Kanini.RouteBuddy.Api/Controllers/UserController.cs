using Kanini.RouteBuddy.Application.Services.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kanini.RouteBuddy.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult> GetAllUsers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 50)
        {
            try
            {
                _logger.LogInformation("Getting all users - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);
                
                var users = await _userService.GetAllUsersAsync(pageNumber, pageSize);
                
                _logger.LogInformation("Retrieved {Count} users", users.Count());
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all users");
                return StatusCode(500, new { error = "An error occurred while retrieving users" });
            }
        }

        [HttpGet("filter")]
        public async Task<ActionResult> FilterUsers(
            [FromQuery] string? searchTerm,
            [FromQuery] string? role,
            [FromQuery] bool? isActive)
        {
            try
            {
                _logger.LogInformation("Filtering users - Search: {SearchTerm}, Role: {Role}, Active: {IsActive}", 
                    searchTerm, role, isActive);
                
                var users = await _userService.FilterUsersAsync(searchTerm, role, isActive);
                
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while filtering users");
                return StatusCode(500, new { error = "An error occurred while filtering users" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetUserById(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Invalid user ID");

                var user = await _userService.GetUserByIdAsync(id);
                
                if (user == null)
                    return NotFound($"User with ID {id} not found");

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting user by ID: {UserId}", id);
                return StatusCode(500, new { error = "An error occurred while retrieving user" });
            }
        }

        [HttpPut("{id}/activate")]
        public async Task<ActionResult> ActivateUser(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Invalid user ID");

                var result = await _userService.ActivateUserAsync(id);
                
                if (!result)
                    return NotFound($"User with ID {id} not found");

                return Ok(new { message = "User activated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while activating user: {UserId}", id);
                return StatusCode(500, new { error = "An error occurred while activating user" });
            }
        }

        [HttpPut("{id}/deactivate")]
        public async Task<ActionResult> DeactivateUser(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Invalid user ID");

                var result = await _userService.DeactivateUserAsync(id);
                
                if (!result)
                    return NotFound($"User with ID {id} not found");

                return Ok(new { message = "User deactivated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deactivating user: {UserId}", id);
                return StatusCode(500, new { error = "An error occurred while deactivating user" });
            }
        }
    }
}