using AutoMapper;
using Kanini.RouteBuddy.Application.Dto;
using Kanini.RouteBuddy.Application.Dto.Stop;
using Kanini.RouteBuddy.Application.Services.Buses;
using Kanini.RouteBuddy.Application.Services.Email;
using Kanini.RouteBuddy.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Kanini.RouteBuddy.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class Bus_Search_Book_Controller : ControllerBase
{
    private readonly IBus_Search_Book_Service _busService;
    private readonly IMapper _mapper;
    private readonly ILogger<Bus_Search_Book_Controller> _logger;
    private readonly IEmailService _emailService;

    public Bus_Search_Book_Controller(
        IBus_Search_Book_Service busService,
        IMapper mapper,
        ILogger<Bus_Search_Book_Controller> logger,
        IEmailService emailService
    )
    {
        _busService = busService;
        _mapper = mapper;
        _logger = logger;
        _emailService = emailService;
    }

    [HttpPost("search")]
    public async Task<IActionResult> SearchBuses([FromBody] BusSearchRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning(MagicStrings.LogMessages.ValidationFailed);
                return BadRequest(ModelState);
            }

            _logger.LogInformation(
                MagicStrings.LogMessages.BusSearchStarted,
                request.Source,
                request.Destination,
                request.TravelDate
            );

            var result = await _busService.SearchBusesAsync(request);

            if (result.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.BusSearchFailed,
                    result.Error.Description
                );
                return BadRequest(result.Error);
            }

            _logger.LogInformation(MagicStrings.LogMessages.BusSearchCompleted, result.Value.Count);
            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.BusSearchFailed, ex.Message);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    [HttpGet("{scheduleId}/seats")]
    public async Task<IActionResult> GetSeatLayout(int scheduleId, [FromQuery] DateTime travelDate)
    {
        try
        {
            var request = _mapper.Map<SeatLayoutRequestDto>((scheduleId, travelDate));

            if (!TryValidateModel(request))
            {
                _logger.LogWarning(MagicStrings.LogMessages.ValidationFailed);
                return BadRequest(ModelState);
            }

            _logger.LogInformation(
                MagicStrings.LogMessages.SeatLayoutStarted,
                scheduleId,
                travelDate
            );

            var result = await _busService.GetSeatLayoutAsync(request);

            if (result.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.SeatLayoutFailed,
                    result.Error.Description
                );
                return BadRequest(result.Error);
            }

            _logger.LogInformation(
                MagicStrings.LogMessages.SeatLayoutCompleted,
                result.Value.Seats.Count
            );
            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.SeatLayoutFailed, ex.Message);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    [HttpGet("{scheduleId}/stops")]
    public async Task<IActionResult> GetRouteStops(int scheduleId)
    {
        try
        {
            if (scheduleId <= 0)
            {
                _logger.LogWarning(MagicStrings.LogMessages.ValidationFailed);
                return BadRequest(new { Error = MagicStrings.ErrorMessages.ScheduleIdRequired });
            }

            _logger.LogInformation(MagicStrings.LogMessages.RouteStopsStarted, scheduleId);

            var result = await _busService.GetRouteStopsAsync(scheduleId);

            if (result.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.RouteStopsFailed,
                    result.Error.Description
                );
                return BadRequest(result.Error);
            }

            _logger.LogInformation(
                MagicStrings.LogMessages.RouteStopsCompleted,
                result.Value.Count
            );
            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.RouteStopsFailed, ex.Message);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    [HttpPost("book")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> BookSeats([FromBody] BookingRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning(MagicStrings.LogMessages.ValidationFailed);
                return BadRequest(ModelState);
            }

            // Get CustomerId from JWT token instead of trusting frontend
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                return BadRequest(new { Error = "Invalid user token" });

            // Get customer profile to find the correct CustomerId
            var customerResult = await _busService.GetCustomerByUserIdAsync(userId);
            if (customerResult.IsFailure)
            {
                _logger.LogError("Customer not found for UserId: {UserId}", userId);
                return BadRequest(new { Error = "Customer profile not found" });
            }

            // Override the CustomerId from JWT token
            request.CustomerId = customerResult.Value.CustomerId;

            _logger.LogInformation(
                MagicStrings.LogMessages.BookingStarted,
                request.ScheduleId,
                request.SeatNumbers.Count,
                request.CustomerId
            );

            var result = await _busService.BookSeatsAsync(request);

            if (result.IsFailure)
            {
                _logger.LogError(MagicStrings.LogMessages.BookingFailed, result.Error.Description);
                return BadRequest(result.Error);
            }

            _logger.LogInformation(
                MagicStrings.LogMessages.BookingCompleted,
                result.Value.PNR,
                result.Value.BookingId
            );
            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.BookingFailed, ex.Message);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    [HttpPost("book/{bookingId}/confirm")]
    public async Task<IActionResult> ConfirmBooking(
        int bookingId,
        [FromBody] BookingConfirmationDto request
    )
    {
        try
        {
            if (bookingId != request.BookingId)
            {
                _logger.LogWarning(MagicStrings.LogMessages.ValidationFailed);
                return BadRequest(new { Error = "Booking ID mismatch" });
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning(MagicStrings.LogMessages.ValidationFailed);
                return BadRequest(ModelState);
            }

            _logger.LogInformation(MagicStrings.LogMessages.BookingConfirmationStarted, bookingId);

            var result = await _busService.ConfirmBookingAsync(request);

            if (result.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.BookingConfirmationFailed,
                    result.Error.Description
                );
                return BadRequest(result.Error);
            }

            _logger.LogInformation(
                MagicStrings.LogMessages.BookingConfirmationCompleted,
                bookingId
            );

            _ = Task.Run(async () =>
            {
                try
                {
                    await _emailService.SendBookingConfirmationAsync(bookingId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        MagicStrings.LogMessages.EmailSendingFailed,
                        bookingId,
                        ex.Message
                    );
                }
            });

            return Ok(new { Message = result.Value });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.BookingConfirmationFailed, ex.Message);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }



    [HttpPost("search/filtered")]
    public async Task<IActionResult> SearchBusesFiltered([FromBody] BusSearchFilterDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning(MagicStrings.LogMessages.ValidationFailed);
                return BadRequest(ModelState);
            }

            _logger.LogInformation(
                MagicStrings.LogMessages.FilteredBusSearchStarted,
                request.Source,
                request.Destination,
                request.TravelDate
            );

            var result = await _busService.SearchBusesFilteredAsync(request);

            if (result.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.FilteredBusSearchFailed,
                    result.Error.Description
                );
                return BadRequest(result.Error);
            }

            _logger.LogInformation(
                MagicStrings.LogMessages.FilteredBusSearchCompleted,
                result.Value.Count
            );
            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.FilteredBusSearchFailed, ex.Message);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }
}
