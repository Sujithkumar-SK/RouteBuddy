using Kanini.RouteBuddy.Application.Services.Admin;
using Kanini.RouteBuddy.Common.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kanini.RouteBuddy.Api.Controllers
{
    [ApiController]
    [Route("api/admin/buses")]
    [Authorize(Roles = "Admin")]
    public class AdminBusController : ControllerBase
    {
        private readonly IAdminBusService _adminBusService;
        private readonly ILogger<AdminBusController> _logger;

        public AdminBusController(IAdminBusService adminBusService, ILogger<AdminBusController> logger)
        {
            _adminBusService = adminBusService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult> GetAllBuses()
        {
            try
            {
                _logger.LogInformation(BusMessages.LogMessages.GettingAllBuses);
                var result = await _adminBusService.GetAllBusesAsync();
                
                if (result.IsSuccess)
                {
                    _logger.LogInformation(BusMessages.LogMessages.AllBusesRetrievedSuccessfully, result.Value.Count);
                    return Ok(result.Value);
                }
                
                _logger.LogWarning(BusMessages.LogMessages.AllBusesRetrievalFailed);
                return BadRequest(result.Error.Description);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, BusMessages.LogMessages.AllBusesRetrievalException);
                return StatusCode(500, BusMessages.ErrorMessages.UnexpectedError);
            }
        }

        [HttpGet("pending")]
        public async Task<ActionResult> GetPendingBuses()
        {
            try
            {
                _logger.LogInformation(BusMessages.LogMessages.GettingPendingBuses);
                var result = await _adminBusService.GetPendingBusesAsync();
                
                if (result.IsSuccess)
                {
                    _logger.LogInformation(BusMessages.LogMessages.PendingBusesRetrievedSuccessfully, result.Value.Count);
                    return Ok(result.Value);
                }
                
                _logger.LogWarning(BusMessages.LogMessages.PendingBusesRetrievalFailed);
                return BadRequest(result.Error.Description);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, BusMessages.LogMessages.PendingBusesRetrievalException);
                return StatusCode(500, BusMessages.ErrorMessages.UnexpectedError);
            }
        }

        [HttpGet("filter")]
        public async Task<ActionResult> FilterBuses(
            [FromQuery] string? searchName,
            [FromQuery] int? status,
            [FromQuery] bool? isActive)
        {
            try
            {
                _logger.LogInformation(BusMessages.LogMessages.FilteringBuses, searchName, status, isActive);
                var result = await _adminBusService.FilterBusesAsync(searchName, status, isActive);
                
                if (result.IsSuccess)
                {
                    _logger.LogInformation(BusMessages.LogMessages.BusesFilteredSuccessfully, result.Value.Count);
                    return Ok(result.Value);
                }
                
                _logger.LogWarning(BusMessages.LogMessages.BusesFilterFailed);
                return BadRequest(result.Error.Description);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, BusMessages.LogMessages.BusesFilterException);
                return StatusCode(500, BusMessages.ErrorMessages.UnexpectedError);
            }
        }

        [HttpGet("{busId}")]
        public async Task<ActionResult> GetBusDetails(int busId)
        {
            try
            {
                if (busId <= 0)
                {
                    _logger.LogWarning(BusMessages.LogMessages.InvalidBusId, busId);
                    return BadRequest(BusMessages.ErrorMessages.InvalidBusData);
                }

                _logger.LogInformation(BusMessages.LogMessages.GettingBusById, busId);
                var result = await _adminBusService.GetBusDetailsAsync(busId);
                
                if (result.IsSuccess)
                {
                    _logger.LogInformation(BusMessages.LogMessages.BusRetrievedSuccessfully, busId);
                    return Ok(result.Value);
                }
                
                _logger.LogWarning(BusMessages.LogMessages.BusNotFound, busId);
                return NotFound(result.Error.Description);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, BusMessages.LogMessages.BusRetrievalException, busId);
                return StatusCode(500, BusMessages.ErrorMessages.UnexpectedError);
            }
        }

        [HttpPost("approve/{busId}")]
        public async Task<ActionResult> ApproveBus(int busId)
        {
            try
            {
                if (busId <= 0)
                {
                    _logger.LogWarning(BusMessages.LogMessages.InvalidBusId, busId);
                    return BadRequest(BusMessages.ErrorMessages.InvalidBusData);
                }

                _logger.LogInformation(BusMessages.LogMessages.ApprovingBus, busId);
                var result = await _adminBusService.ApproveBusAsync(busId);
                
                if (result.IsSuccess)
                {
                    _logger.LogInformation(BusMessages.LogMessages.BusApprovedSuccessfully, busId);
                    return Ok(new { message = "Bus approved and notification email sent" });
                }
                
                _logger.LogWarning(BusMessages.LogMessages.BusApprovalFailed, busId);
                return NotFound(result.Error.Description);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, BusMessages.LogMessages.BusApprovalException, busId);
                return StatusCode(500, BusMessages.ErrorMessages.UnexpectedError);
            }
        }

        [HttpPost("reject/{busId}")]
        public async Task<ActionResult> RejectBus(int busId, [FromBody] string rejectionReason)
        {
            try
            {
                if (busId <= 0)
                {
                    _logger.LogWarning(BusMessages.LogMessages.InvalidBusId, busId);
                    return BadRequest(BusMessages.ErrorMessages.InvalidBusData);
                }

                if (string.IsNullOrWhiteSpace(rejectionReason) || rejectionReason.Length < 10)
                {
                    _logger.LogWarning(BusMessages.LogMessages.InvalidRejectionReason, busId);
                    return BadRequest("Rejection reason must be at least 10 characters");
                }

                _logger.LogInformation(BusMessages.LogMessages.RejectingBus, busId);
                var result = await _adminBusService.RejectBusAsync(busId, rejectionReason);
                
                if (result.IsSuccess)
                {
                    _logger.LogInformation(BusMessages.LogMessages.BusRejectedSuccessfully, busId);
                    return Ok(new { message = "Bus rejected and notification email sent" });
                }
                
                _logger.LogWarning(BusMessages.LogMessages.BusRejectionFailed, busId);
                return NotFound(result.Error.Description);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, BusMessages.LogMessages.BusRejectionException, busId);
                return StatusCode(500, BusMessages.ErrorMessages.UnexpectedError);
            }
        }

        [HttpPut("deactivate/{busId}")]
        public async Task<ActionResult> DeactivateBus(int busId, [FromBody] string reason)
        {
            try
            {
                if (busId <= 0)
                {
                    _logger.LogWarning(BusMessages.LogMessages.InvalidBusId, busId);
                    return BadRequest(BusMessages.ErrorMessages.InvalidBusData);
                }

                if (string.IsNullOrWhiteSpace(reason) || reason.Length < 10)
                {
                    _logger.LogWarning(BusMessages.LogMessages.InvalidDeactivationReason, busId);
                    return BadRequest("Reason must be at least 10 characters");
                }

                _logger.LogInformation(BusMessages.LogMessages.DeactivatingBus, busId);
                var result = await _adminBusService.DeactivateBusAsync(busId, reason);
                
                if (result.IsSuccess)
                {
                    _logger.LogInformation(BusMessages.LogMessages.BusDeactivatedSuccessfully, busId);
                    return Ok(new { message = "Bus deactivated and notification email sent" });
                }
                
                _logger.LogWarning(BusMessages.LogMessages.BusDeactivationFailed, busId);
                return NotFound(result.Error.Description);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, BusMessages.LogMessages.BusDeactivationException, busId);
                return StatusCode(500, BusMessages.ErrorMessages.UnexpectedError);
            }
        }

        [HttpPut("reactivate/{busId}")]
        public async Task<ActionResult> ReactivateBus(int busId, [FromBody] string reason)
        {
            try
            {
                if (busId <= 0)
                {
                    _logger.LogWarning(BusMessages.LogMessages.InvalidBusId, busId);
                    return BadRequest(BusMessages.ErrorMessages.InvalidBusData);
                }

                if (string.IsNullOrWhiteSpace(reason) || reason.Length < 10)
                {
                    _logger.LogWarning(BusMessages.LogMessages.InvalidReactivationReason, busId);
                    return BadRequest("Reason must be at least 10 characters");
                }

                _logger.LogInformation(BusMessages.LogMessages.ReactivatingBus, busId);
                var result = await _adminBusService.ReactivateBusAsync(busId, reason);
                
                if (result.IsSuccess)
                {
                    _logger.LogInformation(BusMessages.LogMessages.BusReactivatedSuccessfully, busId);
                    return Ok(new { message = "Bus reactivated and notification email sent" });
                }
                
                _logger.LogWarning(BusMessages.LogMessages.BusReactivationFailed, busId);
                return NotFound(result.Error.Description);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, BusMessages.LogMessages.BusReactivationException, busId);
                return StatusCode(500, BusMessages.ErrorMessages.UnexpectedError);
            }
        }
    }
}