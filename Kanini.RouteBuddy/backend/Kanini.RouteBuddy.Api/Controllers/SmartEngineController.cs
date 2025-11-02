using Kanini.RouteBuddy.Application.Dto;
using Kanini.RouteBuddy.Application.Services.Email;
using Kanini.RouteBuddy.Application.Services.SmartEnigne;
using Kanini.RouteBuddy.Common;
using Microsoft.AspNetCore.Mvc;

namespace Kanini.RouteBuddy.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SmartEngineController : ControllerBase
{
    private readonly ISmartEngineService _smartEngineService;
    private readonly ILogger<SmartEngineController> _logger;
    private readonly ISmartEmailService _smartEmailService;

    public SmartEngineController(
        ISmartEngineService smartEngineService,
        ILogger<SmartEngineController> logger,
        ISmartEmailService smartEmailService
    )
    {
        _smartEngineService = smartEngineService;
        _logger = logger;
        _smartEmailService = smartEmailService;
    }

    [HttpPost("connecting-routes")]
    public async Task<IActionResult> FindConnectingRoutes(
        [FromBody] ConnectingSearchRequestDto request
    )
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning(MagicStrings.LogMessages.ValidationFailed);
                return BadRequest(ModelState);
            }

            _logger.LogInformation(
                MagicStrings.LogMessages.ConnectingRoutesSearchStarted,
                request.Source,
                request.Destination,
                request.TravelDate
            );

            var result = await _smartEngineService.FindConnectingRoutesAsync(request);

            if (result.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.ConnectingRoutesSearchFailed,
                    result.Error.Description
                );
                return BadRequest(result.Error);
            }

            _logger.LogInformation(
                MagicStrings.LogMessages.ConnectingRoutesSearchCompleted,
                result.Value.Count
            );
            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.ConnectingRoutesSearchFailed, ex.Message);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    [HttpPost("book-connecting-route")]
    public async Task<IActionResult> BookConnectingRoute(
        [FromBody] ConnectingBookingRequestDto request
    )
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning(MagicStrings.LogMessages.ValidationFailed);
                return BadRequest(ModelState);
            }

            _logger.LogInformation(
                MagicStrings.LogMessages.ConnectingBookingStarted,
                request.CustomerId,
                request.Segments.Count,
                request.TravelDate
            );

            var result = await _smartEngineService.BookConnectingRouteAsync(request);

            if (result.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.ConnectingBookingFailed,
                    result.Error.Description
                );
                return BadRequest(result.Error);
            }

            _logger.LogInformation(
                MagicStrings.LogMessages.ConnectingBookingCompleted,
                result.Value.PNR,
                result.Value.BookingId
            );
            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.ConnectingBookingFailed, ex.Message);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    [HttpPost("book-connecting-route/{bookingId}/confirm")]
    public async Task<IActionResult> ConfirmConnectingBooking(
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

            _logger.LogInformation(
                MagicStrings.LogMessages.ConnectingBookingConfirmationStarted,
                bookingId
            );

            var result = await _smartEngineService.ConfirmConnectingBookingAsync(request);

            if (result.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.ConnectingBookingConfirmationFailed,
                    result.Error.Description
                );
                return BadRequest(result.Error);
            }

            _logger.LogInformation(
                MagicStrings.LogMessages.ConnectingBookingConfirmationCompleted,
                bookingId
            );

            // Fire & Forget: Send smart connecting booking confirmation email
            _ = Task.Run(async () =>
            {
                try
                {
                    await _smartEmailService.SendConnectingBookingConfirmationAsync(bookingId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        MagicStrings.LogMessages.SmartEmailSendingFailed,
                        bookingId,
                        ex.Message
                    );
                }
            });

            return Ok(new { Message = result.Value });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                MagicStrings.LogMessages.ConnectingBookingConfirmationFailed,
                ex.Message
            );
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }
}
