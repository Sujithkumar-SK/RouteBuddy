using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Kanini.RouteBuddy.Application.Services.Customer;
using Kanini.RouteBuddy.Application.Dto;
using Kanini.RouteBuddy.Common;
using System.ComponentModel.DataAnnotations;
using Kanini.RouteBuddy.Domain.Enums;

namespace Kanini.RouteBuddy.Api.Controllers
{
    [ApiController]
    [Route("api/customer/profile")]
    [Authorize]
    public class CustomerProfileController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly ILogger<CustomerProfileController> _logger;

        public CustomerProfileController(ICustomerService customerService, ILogger<CustomerProfileController> logger)
        {
            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("my-profile")]
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult<CustomerProfileDto>> GetMyProfile()
        {
            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                    return BadRequest(new { Error = "Invalid user token" });

                _logger.LogInformation("Customer profile retrieval by UserId started: {UserId}", userId);

                var profile = await _customerService.GetCustomerProfileByUserIdAsync(userId);
                if (profile == null)
                {
                    _logger.LogWarning("Customer profile not found for UserId: {UserId}", userId);
                    return NotFound(new { Error = MagicStrings.ErrorMessages.CustomerProfileNotFound });
                }

                _logger.LogInformation("Customer profile retrieval by UserId completed: {UserId}", userId);
                return Ok(profile);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument for customer profile by UserId");
                return BadRequest(new { Error = MagicStrings.ErrorMessages.InvalidCustomerData });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Database operation failed while getting customer profile by UserId");
                return StatusCode(503, new { Error = "Service temporarily unavailable" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Customer profile retrieval by UserId failed: {Error}", ex.Message);
                return StatusCode(500, new { Error = MagicStrings.ErrorMessages.InternalServerError });
            }
        }

        [HttpPut("my-profile")]
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult> UpdateMyProfile([FromBody] UpdateCustomerProfileDto updateDto)
        {
            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                    return BadRequest(new { Error = "Invalid user token" });

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Validation failed for customer profile update by UserId: {UserId}", userId);
                    return BadRequest(ModelState);
                }

                // Get customer profile to find customerId
                var profile = await _customerService.GetCustomerProfileByUserIdAsync(userId);
                if (profile == null)
                {
                    _logger.LogWarning("Customer profile not found for UserId: {UserId}", userId);
                    return NotFound(new { Error = MagicStrings.ErrorMessages.CustomerProfileNotFound });
                }

                // Additional business validations
                var age = DateTime.Today.Year - updateDto.DateOfBirth.Year;
                if (updateDto.DateOfBirth.Date > DateTime.Today.AddYears(-age)) age--;
                
                if (age < 18)
                {
                    return BadRequest(new { Error = "Customer must be at least 18 years old" });
                }

                if (age > 100)
                {
                    return BadRequest(new { Error = "Invalid date of birth" });
                }

                if (updateDto.DateOfBirth > DateTime.Today)
                {
                    return BadRequest(new { Error = "Date of birth cannot be in the future" });
                }

                _logger.LogInformation("Customer profile update by UserId started: {UserId}", userId);

                var result = await _customerService.UpdateCustomerProfileAsync(profile.CustomerId, updateDto);
                if (!result)
                {
                    _logger.LogWarning("Customer profile update failed for UserId: {UserId}", userId);
                    return BadRequest(new { Error = MagicStrings.ErrorMessages.CustomerProfileUpdateFailed });
                }

                _logger.LogInformation("Customer profile update by UserId completed: {UserId}", userId);
                return Ok(new { Message = MagicStrings.SuccessMessages.CustomerProfileUpdatedSuccessfully });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument for customer profile update by UserId");
                return BadRequest(new { Error = MagicStrings.ErrorMessages.InvalidCustomerData });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Database operation failed while updating customer profile by UserId");
                return StatusCode(503, new { Error = "Service temporarily unavailable" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Customer profile update by UserId failed: {Error}", ex.Message);
                return StatusCode(500, new { Error = MagicStrings.ErrorMessages.InternalServerError });
            }
        }

        [HttpGet("{customerId:int:min(1):max(999999)}")]
        public async Task<ActionResult<CustomerProfileDto>> GetCustomerProfile([Required] int customerId)
        {
            try
            {
                _logger.LogInformation(MagicStrings.LogMessages.CustomerProfileRetrievalStarted, customerId);

                var profile = await _customerService.GetCustomerProfileAsync(customerId);
                if (profile == null)
                {
                    _logger.LogWarning("Customer profile not found: {CustomerId}", customerId);
                    return NotFound(new { Error = MagicStrings.ErrorMessages.CustomerProfileNotFound });
                }

                _logger.LogInformation(MagicStrings.LogMessages.CustomerProfileRetrievalCompleted, customerId);
                return Ok(profile);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument for customer profile: {CustomerId}", customerId);
                return BadRequest(new { Error = MagicStrings.ErrorMessages.InvalidCustomerData });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Database operation failed while getting customer profile {CustomerId}", customerId);
                return StatusCode(503, new { Error = "Service temporarily unavailable" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, MagicStrings.LogMessages.CustomerProfileRetrievalFailed, customerId, ex.Message);
                return StatusCode(500, new { Error = MagicStrings.ErrorMessages.InternalServerError });
            }
        }

        [HttpPut("{customerId:int:min(1):max(999999)}")]
        public async Task<ActionResult> UpdateCustomerProfile([Required] int customerId, [FromBody] UpdateCustomerProfileDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Validation failed for customer profile update: {CustomerId}", customerId);
                    return BadRequest(ModelState);
                }

                // Additional business validations
                var age = DateTime.Today.Year - updateDto.DateOfBirth.Year;
                if (updateDto.DateOfBirth.Date > DateTime.Today.AddYears(-age)) age--;
                
                if (age < 18)
                {
                    return BadRequest(new { Error = "Customer must be at least 18 years old" });
                }

                if (age > 100)
                {
                    return BadRequest(new { Error = "Invalid date of birth" });
                }

                if (updateDto.DateOfBirth > DateTime.Today)
                {
                    return BadRequest(new { Error = "Date of birth cannot be in the future" });
                }

                _logger.LogInformation(MagicStrings.LogMessages.CustomerProfileUpdateStarted, customerId);

                var result = await _customerService.UpdateCustomerProfileAsync(customerId, updateDto);
                if (!result)
                {
                    _logger.LogWarning("Customer profile update failed: {CustomerId}", customerId);
                    return BadRequest(new { Error = MagicStrings.ErrorMessages.CustomerProfileUpdateFailed });
                }

                _logger.LogInformation(MagicStrings.LogMessages.CustomerProfileUpdateCompleted, customerId);
                return Ok(new { Message = MagicStrings.SuccessMessages.CustomerProfileUpdatedSuccessfully });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument for customer profile update: {CustomerId}", customerId);
                return BadRequest(new { Error = MagicStrings.ErrorMessages.InvalidCustomerData });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Database operation failed while updating customer profile {CustomerId}", customerId);
                return StatusCode(503, new { Error = "Service temporarily unavailable" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, MagicStrings.LogMessages.CustomerProfileUpdateFailed, customerId, ex.Message);
                return StatusCode(500, new { Error = MagicStrings.ErrorMessages.InternalServerError });
            }
        }

        [HttpPut("my-profile/picture")]
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult> UpdateMyProfilePicture([FromForm] UpdateProfilePictureDto updateDto)
        {
            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                    return BadRequest(new { Error = "Invalid user token" });

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Validation failed for profile picture update by UserId: {UserId}", userId);
                    return BadRequest(ModelState);
                }

                // Get customer profile to find customerId
                var profile = await _customerService.GetCustomerProfileByUserIdAsync(userId);
                if (profile == null)
                {
                    _logger.LogWarning("Customer profile not found for UserId: {UserId}", userId);
                    return NotFound(new { Error = MagicStrings.ErrorMessages.CustomerProfileNotFound });
                }

                _logger.LogInformation("Customer profile picture update by UserId started: {UserId}", userId);

                var result = await _customerService.UpdateCustomerProfilePictureAsync(profile.CustomerId, updateDto.ProfilePicture);
                if (!result)
                {
                    _logger.LogWarning("Customer profile picture update failed for UserId: {UserId}", userId);
                    return BadRequest(new { Error = "Profile picture update failed" });
                }

                _logger.LogInformation("Customer profile picture update by UserId completed: {UserId}", userId);
                return Ok(new { Message = "Profile picture updated successfully" });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument for profile picture update by UserId");
                return BadRequest(new { Error = "Invalid file provided" });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Database operation failed while updating profile picture by UserId");
                return StatusCode(503, new { Error = "Service temporarily unavailable" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Profile picture update by UserId failed: {Error}", ex.Message);
                return StatusCode(500, new { Error = MagicStrings.ErrorMessages.InternalServerError });
            }
        }

        [HttpGet("{customerId:int:min(1):max(999999)}/bookings")]
        public async Task<ActionResult<IEnumerable<CustomerBookingDto>>> GetCustomerBookings(
            [Required] int customerId,
            [FromQuery] BookingStatus? status = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            try
            {
                _logger.LogInformation("Getting bookings for customer: {CustomerId}", customerId);

                var bookings = await _customerService.GetCustomerBookingsAsync(customerId, status, fromDate, toDate);
                
                _logger.LogInformation("Retrieved {Count} bookings for customer: {CustomerId}", bookings.Count(), customerId);
                return Ok(bookings);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument for customer bookings: {CustomerId}", customerId);
                return BadRequest(new { Error = "Invalid request parameters" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting bookings for customer: {CustomerId}", customerId);
                return StatusCode(500, new { Error = MagicStrings.ErrorMessages.InternalServerError });
            }
        }

        [HttpGet("my-bookings")]
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult<IEnumerable<CustomerBookingDto>>> GetMyBookings(
            [FromQuery] BookingStatus? status = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                    return BadRequest(new { Error = "Invalid user token" });

                // Get customer profile to find customerId
                var profile = await _customerService.GetCustomerProfileByUserIdAsync(userId);
                if (profile == null)
                {
                    _logger.LogWarning("Customer profile not found for UserId: {UserId}", userId);
                    return NotFound(new { Error = MagicStrings.ErrorMessages.CustomerProfileNotFound });
                }

                _logger.LogInformation("Getting bookings for customer: {CustomerId}", profile.CustomerId);

                var bookings = await _customerService.GetCustomerBookingsAsync(profile.CustomerId, status, fromDate, toDate);
                
                _logger.LogInformation("Retrieved {Count} bookings for customer: {CustomerId}", bookings.Count(), profile.CustomerId);
                return Ok(bookings);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument for my bookings");
                return BadRequest(new { Error = "Invalid request parameters" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting my bookings");
                return StatusCode(500, new { Error = MagicStrings.ErrorMessages.InternalServerError });
            }
        }

        [HttpGet("{customerId:int:min(1):max(999999)}/bookings/{bookingId:int:min(1):max(999999)}/ticket")]
        public async Task<ActionResult> DownloadTicket([Required] int customerId, [Required] int bookingId)
        {
            try
            {
                _logger.LogInformation("Downloading ticket for customer: {CustomerId}, booking: {BookingId}", customerId, bookingId);

                var ticketBytes = await _customerService.GenerateTicketAsync(customerId, bookingId);
                if (ticketBytes == null)
                {
                    return NotFound(new { Error = "Ticket not found or not accessible" });
                }

                return File(ticketBytes, "application/pdf", $"Ticket-{bookingId}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading ticket for customer: {CustomerId}, booking: {BookingId}", customerId, bookingId);
                return StatusCode(500, new { Error = MagicStrings.ErrorMessages.InternalServerError });
            }
        }
    }
}