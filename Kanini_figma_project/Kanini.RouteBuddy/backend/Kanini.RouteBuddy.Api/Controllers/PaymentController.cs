using System.Security.Claims;
using Kanini.RouteBuddy.Application.Dto;
using Kanini.RouteBuddy.Application.Services;
using Kanini.RouteBuddy.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kanini.RouteBuddy.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly ILogger<PaymentController> _logger;

    public PaymentController(
        IPaymentService paymentService,
        ILogger<PaymentController> logger)
    {
        _paymentService = paymentService;
        _logger = logger;
    }

    [HttpPost("initiate")]
    public async Task<IActionResult> InitiatePayment([FromBody] PaymentInitiateRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning(MagicStrings.LogMessages.ValidationFailed);
                return BadRequest(ModelState);
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int customerId))
            {
                return BadRequest(new { Error = MagicStrings.ErrorMessages.InvalidCustomerId });
            }

            var currentUser = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";

            _logger.LogInformation(MagicStrings.LogMessages.PaymentInitiationStarted, request.BookingId);

            var result = await _paymentService.InitiatePaymentAsync(request, customerId, currentUser);

            if (result.IsFailure)
            {
                _logger.LogError(MagicStrings.LogMessages.PaymentInitiationFailed, result.Error.Description);
                return BadRequest(result.Error);
            }

            _logger.LogInformation(MagicStrings.LogMessages.PaymentInitiationCompleted, "RazorpayOrder");
            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.PaymentInitiationFailed, ex.Message);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    [HttpPost("verify")]
    public async Task<IActionResult> VerifyPayment([FromBody] PaymentVerifyRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning(MagicStrings.LogMessages.ValidationFailed);
                return BadRequest(ModelState);
            }

            var currentUser = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";

            _logger.LogInformation(MagicStrings.LogMessages.PaymentVerificationStarted, request.RazorpayPaymentId);

            var result = await _paymentService.VerifyPaymentAsync(request, currentUser);

            if (result.IsFailure)
            {
                _logger.LogError(MagicStrings.LogMessages.PaymentVerificationFailed, result.Error.Description);
                return BadRequest(result.Error);
            }

            _logger.LogInformation(MagicStrings.LogMessages.PaymentVerificationCompleted, request.RazorpayPaymentId);
            return Ok(new
            {
                Message = "Payment verified and booking confirmed successfully",
                Payment = result.Value
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.PaymentVerificationFailed, ex.Message);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    [HttpGet("booking/{bookingId}")]
    public async Task<IActionResult> GetPaymentsByBookingId(int bookingId)
    {
        try
        {
            if (bookingId <= 0)
            {
                _logger.LogWarning(MagicStrings.LogMessages.InvalidBookingId, bookingId);
                return BadRequest(new { Error = MagicStrings.ErrorMessages.InvalidBookingId });
            }

            _logger.LogInformation(MagicStrings.LogMessages.GetPaymentsStarted, bookingId);

            var result = await _paymentService.GetPaymentsByBookingIdAsync(bookingId);

            if (result.IsFailure)
            {
                _logger.LogError(MagicStrings.LogMessages.GetPaymentsFailed, result.Error.Description);
                return BadRequest(result.Error);
            }

            _logger.LogInformation(MagicStrings.LogMessages.GetPaymentsCompleted, bookingId);
            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.GetPaymentsFailed, ex.Message);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }
}