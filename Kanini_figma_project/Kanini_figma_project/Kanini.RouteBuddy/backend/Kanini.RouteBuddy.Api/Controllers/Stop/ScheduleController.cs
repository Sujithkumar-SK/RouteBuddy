using System.Security.Claims;
using Kanini.RouteBuddy.Application.Dto.Common;
using Kanini.RouteBuddy.Application.Dto.Schedule;
using Kanini.RouteBuddy.Application.Services.Buses;
using Kanini.RouteBuddy.Application.Services.Schedule;
using Kanini.RouteBuddy.Common.Errors;
using Kanini.RouteBuddy.Common.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Kanini.RouteBuddy.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
// [Authorize] // Temporarily disabled for testing
public class ScheduleController : ControllerBase
{
    private readonly IScheduleService _scheduleService;
    private readonly IBusService _busService;
    private readonly ILogger<ScheduleController> _logger;
    private const int MaxPageSize = 50;

    public ScheduleController(
        IScheduleService scheduleService,
        IBusService busService,
        ILogger<ScheduleController> logger
    )
    {
        _scheduleService = scheduleService;
        _busService = busService;
        _logger = logger;
    }

    private int GetVendorIdFromClaims()
    {
        // For testing without JWT - return default vendor ID
        if (User?.Identity?.IsAuthenticated != true)
            return 1; // Default vendor ID for testing

        var vendorIdClaim = User.FindFirst("VendorId") ?? User.FindFirst(ClaimTypes.NameIdentifier);
        if (vendorIdClaim != null && int.TryParse(vendorIdClaim.Value, out int vendorId))
            return vendorId;
        return 1; // Default fallback
    }

    private async Task<bool> ValidateBusOwnership(int busId, int vendorId)
    {
        var busResult = await _busService.GetBusByIdAsync(busId, vendorId);
        return busResult.IsSuccess;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponseDto<ScheduleResponseDto>>> Create(
        CreateScheduleDto dto
    )
    {
        try
        {
            _logger.LogInformation(
                ScheduleMessages.LogMessages.CreatingSchedule,
                dto?.BusId ?? 0,
                dto?.RouteId ?? 0,
                dto?.TravelDate ?? DateTime.MinValue
            );

            if (dto == null)
            {
                _logger.LogWarning(ScheduleMessages.LogMessages.ScheduleCreationFailed, 0, 0);
                return BadRequest(
                    ApiResponseDto<ScheduleResponseDto>.ErrorResult(
                        ScheduleMessages.ErrorMessages.InvalidScheduleData
                    )
                );
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);
                _logger.LogWarning(
                    ScheduleMessages.LogMessages.ScheduleCreationFailed,
                    dto.BusId,
                    dto.RouteId
                );
                return BadRequest(
                    ApiResponseDto<ScheduleResponseDto>.ErrorResult(string.Join("; ", errors))
                );
            }

            var vendorId = GetVendorIdFromClaims();
            if (vendorId <= 0)
                return Unauthorized(
                    ApiResponseDto<ScheduleResponseDto>.ErrorResult("Invalid vendor authentication")
                );

            // Validate bus ownership
            if (!await ValidateBusOwnership(dto.BusId, vendorId))
            {
                _logger.LogWarning(
                    "Vendor {VendorId} attempted to create schedule for bus {BusId} they don't own",
                    vendorId,
                    dto.BusId
                );
                return Forbid("You can only create schedules for your own buses");
            }

            var result = await _scheduleService.CreateScheduleAsync(dto, vendorId);

            if (result.IsSuccess)
            {
                _logger.LogInformation(
                    ScheduleMessages.LogMessages.ScheduleCreatedSuccessfully,
                    result.Value.ScheduleId
                );
                return CreatedAtAction(
                    nameof(GetById),
                    new { id = result.Value.ScheduleId },
                    ApiResponseDto<ScheduleResponseDto>.SuccessResult(result.Value)
                );
            }

            _logger.LogWarning(
                ScheduleMessages.LogMessages.ScheduleCreationFailed,
                dto.BusId,
                dto.RouteId
            );
            return BadRequest(
                ApiResponseDto<ScheduleResponseDto>.ErrorResult(result.Error.Description)
            );
        }
        catch (SqlException ex)
        {
            _logger.LogError(
                ex,
                ScheduleMessages.LogMessages.ScheduleCreationException,
                dto?.BusId ?? 0,
                dto?.RouteId ?? 0
            );
            return StatusCode(
                500,
                ApiResponseDto<ScheduleResponseDto>.ErrorResult(
                    ScheduleMessages.ErrorMessages.DatabaseError
                )
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                ScheduleMessages.LogMessages.ScheduleCreationException,
                dto?.BusId ?? 0,
                dto?.RouteId ?? 0
            );
            return StatusCode(
                500,
                ApiResponseDto<ScheduleResponseDto>.ErrorResult(
                    ScheduleMessages.ErrorMessages.UnexpectedError
                )
            );
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponseDto<ScheduleResponseDto>>> GetById(int id)
    {
        try
        {
            var vendorId = GetVendorIdFromClaims();
            if (vendorId <= 0)
                return Unauthorized(
                    ApiResponseDto<ScheduleResponseDto>.ErrorResult("Invalid vendor authentication")
                );

            _logger.LogInformation(ScheduleMessages.LogMessages.GettingScheduleById, id);

            if (id <= 0)
            {
                _logger.LogWarning(ScheduleMessages.LogMessages.ScheduleNotFound, id);
                return BadRequest(
                    ApiResponseDto<ScheduleResponseDto>.ErrorResult(
                        ScheduleMessages.ErrorMessages.InvalidScheduleData
                    )
                );
            }

            var result = await _scheduleService.GetScheduleByIdAsync(id);

            if (result.IsSuccess)
            {
                // Validate bus ownership through schedule
                if (!await ValidateBusOwnership(result.Value.BusId, vendorId))
                {
                    _logger.LogWarning(
                        "Vendor {VendorId} attempted to access schedule {ScheduleId} for bus they don't own",
                        vendorId,
                        id
                    );
                    return Forbid("You can only access schedules for your own buses");
                }
            }

            result = await _scheduleService.GetScheduleByIdAsync(id);

            if (result.IsSuccess)
            {
                _logger.LogInformation(
                    ScheduleMessages.LogMessages.ScheduleRetrievedSuccessfully,
                    id
                );
                return Ok(ApiResponseDto<ScheduleResponseDto>.SuccessResult(result.Value));
            }

            _logger.LogWarning(ScheduleMessages.LogMessages.ScheduleNotFound, id);
            return NotFound(
                ApiResponseDto<ScheduleResponseDto>.ErrorResult(result.Error.Description)
            );
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, ScheduleMessages.LogMessages.ScheduleRetrievalException, id);
            return StatusCode(
                500,
                ApiResponseDto<ScheduleResponseDto>.ErrorResult(
                    ScheduleMessages.ErrorMessages.DatabaseError
                )
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ScheduleMessages.LogMessages.ScheduleRetrievalException, id);
            return StatusCode(
                500,
                ApiResponseDto<ScheduleResponseDto>.ErrorResult(
                    ScheduleMessages.ErrorMessages.UnexpectedError
                )
            );
        }
    }

    [HttpGet("my-schedules")]
    public async Task<
        ActionResult<ApiResponseDto<PagedResultDto<ScheduleResponseDto>>>
    > GetMySchedules([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var vendorId = GetVendorIdFromClaims();
            if (vendorId <= 0)
                return Unauthorized(
                    ApiResponseDto<PagedResultDto<ScheduleResponseDto>>.ErrorResult(
                        "Invalid vendor authentication"
                    )
                );

            _logger.LogInformation(
                ScheduleMessages.LogMessages.GettingSchedulesByVendor,
                vendorId,
                pageNumber,
                pageSize
            );

            if (pageNumber <= 0)
                pageNumber = 1;
            if (pageSize <= 0 || pageSize > MaxPageSize)
                pageSize = 10;

            var result = await _scheduleService.GetSchedulesByVendorAsync(
                vendorId,
                pageNumber,
                pageSize
            );

            if (result.IsSuccess)
            {
                _logger.LogInformation(
                    ScheduleMessages.LogMessages.SchedulesByVendorRetrievedSuccessfully,
                    result.Value.Data.Count(),
                    vendorId
                );
                return Ok(
                    ApiResponseDto<PagedResultDto<ScheduleResponseDto>>.SuccessResult(result.Value)
                );
            }

            _logger.LogWarning(
                ScheduleMessages.LogMessages.SchedulesByVendorRetrievalFailed,
                vendorId
            );
            return BadRequest(
                ApiResponseDto<PagedResultDto<ScheduleResponseDto>>.ErrorResult(
                    result.Error.Description
                )
            );
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, ScheduleMessages.LogMessages.SchedulesByVendorException, GetVendorIdFromClaims());
            return StatusCode(
                500,
                ApiResponseDto<PagedResultDto<ScheduleResponseDto>>.ErrorResult(
                    ScheduleMessages.ErrorMessages.DatabaseError
                )
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ScheduleMessages.LogMessages.SchedulesByVendorException, GetVendorIdFromClaims());
            return StatusCode(
                500,
                ApiResponseDto<PagedResultDto<ScheduleResponseDto>>.ErrorResult(
                    ScheduleMessages.ErrorMessages.UnexpectedError
                )
            );
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponseDto<ScheduleResponseDto>>> Update(
        int id,
        UpdateScheduleDto dto
    )
    {
        try
        {
            var vendorId = GetVendorIdFromClaims();
            if (vendorId <= 0)
                return Unauthorized(
                    ApiResponseDto<ScheduleResponseDto>.ErrorResult("Invalid vendor authentication")
                );

            _logger.LogInformation(ScheduleMessages.LogMessages.UpdatingSchedule, id);

            if (id <= 0 || dto == null)
            {
                _logger.LogWarning(ScheduleMessages.LogMessages.ScheduleUpdateFailed, id);
                return BadRequest(
                    ApiResponseDto<ScheduleResponseDto>.ErrorResult(
                        ScheduleMessages.ErrorMessages.InvalidScheduleData
                    )
                );
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);
                _logger.LogWarning(ScheduleMessages.LogMessages.ScheduleUpdateFailed, id);
                return BadRequest(
                    ApiResponseDto<ScheduleResponseDto>.ErrorResult(string.Join("; ", errors))
                );
            }

            // Get schedule to validate bus ownership
            var scheduleResult = await _scheduleService.GetScheduleByIdAsync(id);
            if (scheduleResult.IsFailure)
            {
                return scheduleResult.Error.Type == Common.Utility.ErrorType.NotFound
                    ? NotFound(
                        ApiResponseDto<ScheduleResponseDto>.ErrorResult(
                            scheduleResult.Error.Description
                        )
                    )
                    : StatusCode(
                        500,
                        ApiResponseDto<ScheduleResponseDto>.ErrorResult(
                            scheduleResult.Error.Description
                        )
                    );
            }

            // Validate bus ownership
            if (!await ValidateBusOwnership(scheduleResult.Value.BusId, vendorId))
            {
                _logger.LogWarning(
                    "Vendor {VendorId} attempted to update schedule {ScheduleId} for bus they don't own",
                    vendorId,
                    id
                );
                return Forbid("You can only update schedules for your own buses");
            }

            var result = await _scheduleService.UpdateScheduleAsync(id, dto);

            if (result.IsSuccess)
            {
                _logger.LogInformation(
                    ScheduleMessages.LogMessages.ScheduleUpdatedSuccessfully,
                    id
                );
                return Ok(ApiResponseDto<ScheduleResponseDto>.SuccessResult(result.Value));
            }

            _logger.LogWarning(ScheduleMessages.LogMessages.ScheduleUpdateFailed, id);
            return BadRequest(
                ApiResponseDto<ScheduleResponseDto>.ErrorResult(result.Error.Description)
            );
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, ScheduleMessages.LogMessages.ScheduleUpdateException, id);
            return StatusCode(
                500,
                ApiResponseDto<ScheduleResponseDto>.ErrorResult(
                    ScheduleMessages.ErrorMessages.DatabaseError
                )
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ScheduleMessages.LogMessages.ScheduleUpdateException, id);
            return StatusCode(
                500,
                ApiResponseDto<ScheduleResponseDto>.ErrorResult(
                    ScheduleMessages.ErrorMessages.UnexpectedError
                )
            );
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponseDto<object>>> Delete(int id)
    {
        try
        {
            var vendorId = GetVendorIdFromClaims();
            if (vendorId <= 0)
                return Unauthorized(
                    ApiResponseDto<object>.ErrorResult("Invalid vendor authentication")
                );

            _logger.LogInformation(ScheduleMessages.LogMessages.DeletingSchedule, id);

            if (id <= 0)
            {
                _logger.LogWarning(ScheduleMessages.LogMessages.ScheduleNotFound, id);
                return BadRequest(
                    ApiResponseDto<object>.ErrorResult(
                        ScheduleMessages.ErrorMessages.InvalidScheduleData
                    )
                );
            }

            // Get schedule to validate bus ownership
            var scheduleResult = await _scheduleService.GetScheduleByIdAsync(id);
            if (scheduleResult.IsFailure)
            {
                return scheduleResult.Error.Type == Common.Utility.ErrorType.NotFound
                    ? NotFound(ApiResponseDto<object>.ErrorResult(scheduleResult.Error.Description))
                    : StatusCode(
                        500,
                        ApiResponseDto<object>.ErrorResult(scheduleResult.Error.Description)
                    );
            }

            // Validate bus ownership
            if (!await ValidateBusOwnership(scheduleResult.Value.BusId, vendorId))
            {
                _logger.LogWarning(
                    "Vendor {VendorId} attempted to delete schedule {ScheduleId} for bus they don't own",
                    vendorId,
                    id
                );
                return Forbid("You can only delete schedules for your own buses");
            }

            var result = await _scheduleService.DeleteScheduleAsync(id);

            if (result.IsSuccess)
            {
                _logger.LogInformation(
                    ScheduleMessages.LogMessages.ScheduleDeletedSuccessfully,
                    id
                );
                return Ok(ApiResponseDto<object>.SuccessResult(null!));
            }

            _logger.LogWarning(ScheduleMessages.LogMessages.ScheduleNotFound, id);
            return BadRequest(ApiResponseDto<object>.ErrorResult(result.Error.Description));
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, ScheduleMessages.LogMessages.ScheduleDeletionException, id);
            return StatusCode(
                500,
                ApiResponseDto<object>.ErrorResult(ScheduleMessages.ErrorMessages.DatabaseError)
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ScheduleMessages.LogMessages.ScheduleDeletionException, id);
            return StatusCode(
                500,
                ApiResponseDto<object>.ErrorResult(ScheduleMessages.ErrorMessages.UnexpectedError)
            );
        }
    }

    [HttpPost("bulk")]
    public async Task<ActionResult<ApiResponseDto<List<ScheduleResponseDto>>>> CreateBulk(
        CreateBulkScheduleDto dto
    )
    {
        try
        {
            _logger.LogInformation(
                ScheduleMessages.LogMessages.CreatingBulkSchedule,
                dto?.BusId ?? 0,
                dto?.RouteId ?? 0,
                dto?.StartDate ?? DateTime.MinValue,
                dto?.EndDate ?? DateTime.MinValue
            );

            if (dto == null)
            {
                _logger.LogWarning(
                    ScheduleMessages.LogMessages.BulkScheduleCreationException,
                    0,
                    0
                );
                return BadRequest(
                    ApiResponseDto<List<ScheduleResponseDto>>.ErrorResult(
                        ScheduleMessages.ErrorMessages.InvalidScheduleData
                    )
                );
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);
                _logger.LogWarning(
                    ScheduleMessages.LogMessages.BulkScheduleCreationException,
                    dto.BusId,
                    dto.RouteId
                );
                return BadRequest(
                    ApiResponseDto<List<ScheduleResponseDto>>.ErrorResult(string.Join("; ", errors))
                );
            }

            var vendorId = GetVendorIdFromClaims();
            if (vendorId <= 0)
                return Unauthorized(
                    ApiResponseDto<List<ScheduleResponseDto>>.ErrorResult(
                        "Invalid vendor authentication"
                    )
                );

            // Validate bus ownership
            if (!await ValidateBusOwnership(dto.BusId, vendorId))
            {
                _logger.LogWarning(
                    "Vendor {VendorId} attempted to create bulk schedules for bus {BusId} they don't own",
                    vendorId,
                    dto.BusId
                );
                return Forbid("You can only create schedules for your own buses");
            }

            var result = await _scheduleService.CreateBulkScheduleAsync(dto, vendorId);

            if (result.IsSuccess)
            {
                _logger.LogInformation(
                    ScheduleMessages.LogMessages.BulkScheduleCreatedSuccessfully,
                    result.Value.Count
                );
                return Ok(ApiResponseDto<List<ScheduleResponseDto>>.SuccessResult(result.Value));
            }

            _logger.LogWarning(
                ScheduleMessages.LogMessages.BulkScheduleCreationException,
                dto.BusId,
                dto.RouteId
            );
            return BadRequest(
                ApiResponseDto<List<ScheduleResponseDto>>.ErrorResult(result.Error.Description)
            );
        }
        catch (SqlException ex)
        {
            _logger.LogError(
                ex,
                ScheduleMessages.LogMessages.BulkScheduleCreationException,
                dto?.BusId ?? 0,
                dto?.RouteId ?? 0
            );
            return StatusCode(
                500,
                ApiResponseDto<List<ScheduleResponseDto>>.ErrorResult(
                    ScheduleMessages.ErrorMessages.DatabaseError
                )
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                ScheduleMessages.LogMessages.BulkScheduleCreationException,
                dto?.BusId ?? 0,
                dto?.RouteId ?? 0
            );
            return StatusCode(
                500,
                ApiResponseDto<List<ScheduleResponseDto>>.ErrorResult(
                    ScheduleMessages.ErrorMessages.UnexpectedError
                )
            );
        }
    }
}
