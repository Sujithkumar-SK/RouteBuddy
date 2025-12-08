using Kanini.RouteBuddy.Api.Constants;
using Kanini.RouteBuddy.Application.Dto.Admin;
using Kanini.RouteBuddy.Application.Dto.Booking;
using Kanini.RouteBuddy.Application.Services.Customer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Kanini.RouteBuddy.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(ICustomerService customerService, ILogger<CustomerController> logger)
        {
            _customerService = customerService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AdminCustomerDTO>>> GetAllCustomers()
        {
            _logger.LogInformation("Getting all customers");
            try
            {
                var customers = await _customerService.GetAllCustomersAsync();
                _logger.LogInformation("Retrieved {Count} customers", customers.Count());
                return Ok(customers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all customers");
                return StatusCode(500, string.Format(ErrorMessages.InternalServerError, ex.Message));
            }
        }

        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<AdminCustomerDTO>>> FilterCustomers(
            [FromQuery] string? searchName,
            [FromQuery] bool? isActive,
            [FromQuery] int? minAge,
            [FromQuery] int? maxAge)
        {
            try
            {
                // Enhanced validations
                if (!string.IsNullOrEmpty(searchName) && searchName.Length > 100)
                    return BadRequest(ErrorMessages.SearchNameTooLong);
                
                if (minAge.HasValue && (minAge < 0 || minAge > 120))
                    return BadRequest(ErrorMessages.AgeRangeInvalid);
                
                if (maxAge.HasValue && (maxAge < 0 || maxAge > 120))
                    return BadRequest(ErrorMessages.AgeRangeInvalid);
                
                if (minAge.HasValue && maxAge.HasValue && minAge > maxAge)
                    return BadRequest(ErrorMessages.MinAgeGreaterThanMaxAge);

                var customers = await _customerService.FilterCustomersAsync(searchName, isActive, minAge, maxAge);
                return Ok(customers);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument in filter customers: {Message}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Database connection error in filter customers");
                return StatusCode(503, string.Format(ErrorMessages.DatabaseConnectionError));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access attempt in filter customers");
                return StatusCode(403, ErrorMessages.UnauthorizedAccess);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in filter customers");
                return StatusCode(500, string.Format(ErrorMessages.InternalServerError, ex.Message));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AdminCustomerDTO>> GetCustomerById(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(ErrorMessages.CustomerIdInvalid);

                var customer = await _customerService.GetCustomerByIdAsync(id);
                if (customer == null)
                    return NotFound(string.Format(ErrorMessages.CustomerNotFound, id));

                return Ok(customer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, string.Format(ErrorMessages.InternalServerError, ex.Message));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> SoftDeleteCustomer(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(ErrorMessages.CustomerIdInvalid);

                var result = await _customerService.SoftDeleteCustomerAsync(id);
                if (!result)
                    return NotFound(string.Format(ErrorMessages.CustomerNotFound, id));

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, string.Format(ErrorMessages.InternalServerError, ex.Message));
            }
        }

        [HttpPost("{userId}/bookings/cancel")]
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult<CancelBookingResponseDto>> CancelBooking(int userId, [FromBody] CancelBookingRequestDto request)
        {
            try
            {
                // Get user ID from JWT token
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out int tokenUserId))
                    return BadRequest("Invalid user token");

                // Ensure the userId in URL matches the token
                if (userId != tokenUserId)
                    return Forbid();

                // Get actual customer ID from user ID
                var customerProfile = await _customerService.GetCustomerProfileByUserIdAsync(userId);
                if (customerProfile == null)
                    return BadRequest("Customer profile not found");

                // Validate model
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Validate booking ID
                if (request.BookingId <= 0)
                    return BadRequest("Invalid booking ID");

                // Validate reason length
                if (string.IsNullOrWhiteSpace(request.Reason) || request.Reason.Length < 10 || request.Reason.Length > 250)
                    return BadRequest("Cancellation reason must be between 10 and 250 characters");

                var result = await _customerService.CancelBookingAsync(customerProfile.CustomerId, request);
                
                if (!result.IsSuccess)
                    return BadRequest(new { error = result.Message });

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument in cancel booking: {Message}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Database connection error in cancel booking");
                return StatusCode(503, "Database connection error");
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access attempt in cancel booking");
                return StatusCode(403, "Unauthorized access");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in cancel booking for user: {UserId}, booking: {BookingId}", userId, request.BookingId);
                return StatusCode(500, string.Format(ErrorMessages.InternalServerError, ex.Message));
            }
        }
    }
}