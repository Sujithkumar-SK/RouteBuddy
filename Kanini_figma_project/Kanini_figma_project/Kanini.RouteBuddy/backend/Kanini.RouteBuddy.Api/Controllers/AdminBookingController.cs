using Kanini.RouteBuddy.Api.Constants;
using Kanini.RouteBuddy.Application.Dto.Admin;
using Kanini.RouteBuddy.Application.Dto.Booking;
using Kanini.RouteBuddy.Application.Services.Booking;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Kanini.RouteBuddy.Api.Controllers
{
    [ApiController]
    [Route("api/admin/bookings")]
    public class AdminBookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly ILogger<AdminBookingController> _logger;

        public AdminBookingController(IBookingService bookingService, ILogger<AdminBookingController> logger)
        {
            _bookingService = bookingService ?? throw new ArgumentNullException(nameof(bookingService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AdminBookingDTO>>> GetAllBookings()
        {
            try
            {
                _logger.LogInformation("Retrieving all bookings");
                var bookings = await _bookingService.GetAllBookingsAsync();
                _logger.LogInformation("Retrieved {Count} bookings", bookings.Count());
                return Ok(bookings);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Database operation failed while getting all bookings");
                return StatusCode(503, ErrorMessages.DatabaseConnectionError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while getting all bookings");
                return StatusCode(500, string.Format(ErrorMessages.InternalServerError, ex.Message));
            }
        }

        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<AdminBookingDTO>>> FilterBookings(
            [FromQuery] string? searchName,
            [FromQuery] int? status,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            try
            {
                _logger.LogInformation("Filtering bookings with parameters: SearchName={SearchName}, Status={Status}, FromDate={FromDate}, ToDate={ToDate}", 
                    searchName, status, fromDate, toDate);
                
                var bookings = await _bookingService.FilterBookingsAsync(searchName, status, fromDate, toDate);
                _logger.LogInformation("Retrieved {Count} filtered bookings", bookings.Count());
                return Ok(bookings);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Database operation failed while filtering bookings");
                return StatusCode(503, ErrorMessages.DatabaseConnectionError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while filtering bookings");
                return StatusCode(500, string.Format(ErrorMessages.InternalServerError, ex.Message));
            }
        }

        [HttpGet("{bookingId:int:min(1):max(2147483647)}")]
        public async Task<ActionResult<AdminBookingDTO>> GetBookingById([Required] int bookingId)
        {
            try
            {
                if (bookingId < BookingConstants.MinBookingIdValue || bookingId > BookingConstants.MaxBookingIdValue)
                {
                    _logger.LogWarning("Invalid booking ID provided: {BookingId}", bookingId);
                    return BadRequest(ErrorMessages.BookingIdInvalid);
                }

                _logger.LogInformation("Retrieving booking with ID: {BookingId}", bookingId);
                var booking = await _bookingService.GetBookingByIdAsync(bookingId);
                
                if (booking == null)
                {
                    _logger.LogWarning("Booking not found: {BookingId}", bookingId);
                    return NotFound(string.Format(ErrorMessages.BookingNotFound, bookingId));
                }

                return Ok(booking);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument for booking ID: {BookingId}", bookingId);
                return BadRequest(ErrorMessages.BookingIdInvalid);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Database operation failed while getting booking {BookingId}", bookingId);
                return StatusCode(503, ErrorMessages.DatabaseConnectionError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while getting booking {BookingId}", bookingId);
                return StatusCode(500, string.Format(ErrorMessages.InternalServerError, ex.Message));
            }
        }

        [HttpGet("status-summary")]
        public async Task<ActionResult<BookingStatusSummaryDTO>> GetBookingStatusSummary()
        {
            try
            {
                _logger.LogInformation("Retrieving booking status summary");
                var summary = await _bookingService.GetBookingStatusSummaryAsync();
                return Ok(summary);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Database operation failed while getting booking status summary");
                return StatusCode(503, ErrorMessages.DatabaseConnectionError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while getting booking status summary");
                return StatusCode(500, string.Format(ErrorMessages.InternalServerError, ex.Message));
            }
        }


    }
}