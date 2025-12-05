using System.Security.Claims;
using Kanini.RouteBuddy.Application.Dto.Common;
using Kanini.RouteBuddy.Application.Dto.Schedule;
using Kanini.RouteBuddy.Application.Dto.Route;
using Kanini.RouteBuddy.Application.Dto.Stop;
using Kanini.RouteBuddy.Application.Services.Schedule;
using Kanini.RouteBuddy.Application.Services.Route;
using Kanini.RouteBuddy.Application.Services.Vendor;
using Kanini.RouteBuddy.Common.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Kanini.RouteBuddy.Api.Controllers.Vendor;

[ApiController]
[Route("api/vendor/schedules")]
[Authorize(Roles = "Vendor")]
public class VendorScheduleController : ControllerBase
{
    private readonly IScheduleService _scheduleService;
    private readonly IRouteService _routeService;
    private readonly IVendorService _vendorService;
    private readonly ILogger<VendorScheduleController> _logger;

    public VendorScheduleController(
        IScheduleService scheduleService,
        IRouteService routeService,
        IVendorService vendorService,
        ILogger<VendorScheduleController> logger)
    {
        _scheduleService = scheduleService;
        _routeService = routeService;
        _vendorService = vendorService;
        _logger = logger;
    }

    private async Task<int> GetVendorIdFromClaimsAsync()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out int userId) || userId <= 0)
            return 0;

        var vendorResult = await _vendorService.GetVendorByUserIdAsync(userId);
        return vendorResult.IsSuccess ? vendorResult.Value.VendorId : 0;
    }

    [HttpGet("routes")]
    public async Task<ActionResult<ApiResponseDto<List<RouteSearchDto>>>> GetAllRoutes()
    {
        try
        {
            _logger.LogInformation("Getting all active routes for schedule creation");

            var result = await _routeService.GetAllActiveRoutesAsync();
            
            if (result.IsSuccess)
            {
                return Ok(ApiResponseDto<List<RouteSearchDto>>.SuccessResult(result.Value));
            }

            return BadRequest(ApiResponseDto<List<RouteSearchDto>>.ErrorResult(result.Error.Description));
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "Database error getting all routes");
            return StatusCode(500, ApiResponseDto<List<RouteSearchDto>>.ErrorResult("Database error occurred"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all routes");
            return StatusCode(500, ApiResponseDto<List<RouteSearchDto>>.ErrorResult("An unexpected error occurred"));
        }
    }



    [HttpGet("available-stops")]
    public async Task<ActionResult<ApiResponseDto<List<StopResponseDto>>>> GetAvailableStops()
    {
        try
        {
            _logger.LogInformation("Getting all available stops for vendor");

            var result = await _routeService.GetAllActiveStopsAsync();
            
            if (result.IsSuccess)
            {
                return Ok(ApiResponseDto<List<StopResponseDto>>.SuccessResult(result.Value));
            }

            return BadRequest(ApiResponseDto<List<StopResponseDto>>.ErrorResult(result.Error.Description));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting available stops");
            return StatusCode(500, ApiResponseDto<List<StopResponseDto>>.ErrorResult("An unexpected error occurred"));
        }
    }

    [HttpPost("routes/{routeId}/stops")]
    public async Task<ActionResult<ApiResponseDto<object>>> CreateRouteStops(
        int routeId, 
        [FromBody] List<CreateRouteStopRequest> stops)
    {
        try
        {
            var vendorId = await GetVendorIdFromClaimsAsync();
            if (vendorId <= 0)
            {
                return Unauthorized(ApiResponseDto<object>.ErrorResult("Invalid vendor authentication"));
            }

            // Validation
            if (routeId <= 0)
            {
                return BadRequest(ApiResponseDto<object>.ErrorResult("Invalid route ID"));
            }

            if (stops == null || !stops.Any())
            {
                return BadRequest(ApiResponseDto<object>.ErrorResult("At least one stop is required"));
            }

            if (stops.Count > 20)
            {
                return BadRequest(ApiResponseDto<object>.ErrorResult("Maximum 20 stops allowed per route"));
            }

            // Validate order numbers are unique and sequential
            var orderNumbers = stops.Select(s => s.OrderNumber).ToList();
            if (orderNumbers.Distinct().Count() != orderNumbers.Count)
            {
                return BadRequest(ApiResponseDto<object>.ErrorResult("Order numbers must be unique"));
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(ApiResponseDto<object>.ErrorResult(string.Join("; ", errors)));
            }

            _logger.LogInformation("Creating route stops for route: {RouteId}", routeId);

            var result = await _routeService.CreateRouteStopsAsync(routeId, stops, vendorId);
            
            if (result.IsSuccess)
            {
                return Ok(ApiResponseDto<object>.SuccessResult(new { }, "Route stops created successfully"));
            }

            return BadRequest(ApiResponseDto<object>.ErrorResult(result.Error.Description));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating route stops");
            return StatusCode(500, ApiResponseDto<object>.ErrorResult("An unexpected error occurred"));
        }
    }

    [HttpGet("routes/{routeId}/stops")]
    public async Task<ActionResult<ApiResponseDto<List<RouteStopDetailDto>>>> GetRouteStops(int routeId)
    {
        try
        {
            _logger.LogInformation("Getting route stops for route: {RouteId}", routeId);

            if (routeId <= 0)
            {
                return BadRequest(ApiResponseDto<List<RouteStopDetailDto>>.ErrorResult("Invalid route ID"));
            }

            var result = await _routeService.GetRouteStopsWithDetailsAsync(routeId);
            
            if (result.IsSuccess)
            {
                return Ok(ApiResponseDto<List<RouteStopDetailDto>>.SuccessResult(result.Value));
            }

            return BadRequest(ApiResponseDto<List<RouteStopDetailDto>>.ErrorResult(result.Error.Description));
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "Database error getting route stops for route: {RouteId}", routeId);
            return StatusCode(500, ApiResponseDto<List<RouteStopDetailDto>>.ErrorResult("Database error occurred"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting route stops for route: {RouteId}", routeId);
            return StatusCode(500, ApiResponseDto<List<RouteStopDetailDto>>.ErrorResult("An unexpected error occurred"));
        }
    }

    [HttpPost("{scheduleId}/stops")]
    public async Task<ActionResult<ApiResponseDto<object>>> CreateScheduleStops(
        int scheduleId,
        [FromBody] List<CreateRouteStopRequest> stops)
    {
        try
        {
            var vendorId = await GetVendorIdFromClaimsAsync();
            if (vendorId <= 0)
            {
                return Unauthorized(ApiResponseDto<object>.ErrorResult("Invalid vendor authentication"));
            }

            if (scheduleId <= 0)
            {
                return BadRequest(ApiResponseDto<object>.ErrorResult("Invalid schedule ID"));
            }

            if (stops == null || !stops.Any())
            {
                return BadRequest(ApiResponseDto<object>.ErrorResult("At least one stop is required"));
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(ApiResponseDto<object>.ErrorResult(string.Join("; ", errors)));
            }

            _logger.LogInformation("Creating schedule-specific stops for schedule: {ScheduleId}", scheduleId);

            var result = await _routeService.CreateScheduleRouteStopsAsync(scheduleId, stops);
            
            if (result.IsSuccess)
            {
                return Ok(ApiResponseDto<object>.SuccessResult(new { }, "Schedule stops created successfully"));
            }

            return BadRequest(ApiResponseDto<object>.ErrorResult(result.Error.Description));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating schedule stops");
            return StatusCode(500, ApiResponseDto<object>.ErrorResult("An unexpected error occurred"));
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponseDto<ScheduleResponseDto>>> CreateSchedule(
        [FromBody] VendorCreateScheduleDto dto)
    {
        try
        {
            var vendorId = await GetVendorIdFromClaimsAsync();
            if (vendorId <= 0)
            {
                return Unauthorized(ApiResponseDto<ScheduleResponseDto>.ErrorResult("Invalid vendor authentication"));
            }

            _logger.LogInformation("Vendor {VendorId} creating schedule for bus {BusId} on route {RouteId}", 
                vendorId, dto.BusId, dto.RouteId);

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(ApiResponseDto<ScheduleResponseDto>.ErrorResult(string.Join("; ", errors)));
            }

            var createDto = new CreateScheduleDto
            {
                BusId = dto.BusId,
                RouteId = dto.RouteId,
                TravelDate = dto.TravelDate,
                DepartureTime = dto.DepartureTime,
                ArrivalTime = dto.ArrivalTime
            };

            var result = await _scheduleService.CreateScheduleAsync(createDto, vendorId);

            if (result.IsSuccess)
            {
                _logger.LogInformation("Schedule created successfully with ID: {ScheduleId}", result.Value.ScheduleId);
                return CreatedAtAction(nameof(GetSchedule), new { id = result.Value.ScheduleId }, 
                    ApiResponseDto<ScheduleResponseDto>.SuccessResult(result.Value));
            }

            return BadRequest(ApiResponseDto<ScheduleResponseDto>.ErrorResult(result.Error.Description));
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "Database error creating schedule");
            return StatusCode(500, ApiResponseDto<ScheduleResponseDto>.ErrorResult("Database error occurred"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating schedule");
            return StatusCode(500, ApiResponseDto<ScheduleResponseDto>.ErrorResult("An unexpected error occurred"));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponseDto<ScheduleResponseDto>>> GetSchedule(int id)
    {
        try
        {
            var vendorId = await GetVendorIdFromClaimsAsync();
            if (vendorId <= 0)
            {
                return Unauthorized(ApiResponseDto<ScheduleResponseDto>.ErrorResult("Invalid vendor authentication"));
            }

            _logger.LogInformation("Getting schedule {ScheduleId} for vendor {VendorId}", id, vendorId);

            var result = await _scheduleService.GetScheduleByIdAsync(id);

            if (result.IsSuccess)
            {
                return Ok(ApiResponseDto<ScheduleResponseDto>.SuccessResult(result.Value));
            }

            return NotFound(ApiResponseDto<ScheduleResponseDto>.ErrorResult(result.Error.Description));
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "Database error getting schedule: {ScheduleId}", id);
            return StatusCode(500, ApiResponseDto<ScheduleResponseDto>.ErrorResult("Database error occurred"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting schedule: {ScheduleId}", id);
            return StatusCode(500, ApiResponseDto<ScheduleResponseDto>.ErrorResult("An unexpected error occurred"));
        }
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponseDto<PagedResultDto<ScheduleResponseDto>>>> GetMySchedules(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            var vendorId = await GetVendorIdFromClaimsAsync();
            if (vendorId <= 0)
            {
                return Unauthorized(ApiResponseDto<PagedResultDto<ScheduleResponseDto>>.ErrorResult("Invalid vendor authentication"));
            }

            _logger.LogInformation("Getting schedules for vendor {VendorId}, page {PageNumber}, size {PageSize}", 
                vendorId, pageNumber, pageSize);

            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0 || pageSize > 50) pageSize = 10;

            var result = await _scheduleService.GetSchedulesByVendorAsync(vendorId, pageNumber, pageSize);

            if (result.IsSuccess)
            {
                return Ok(ApiResponseDto<PagedResultDto<ScheduleResponseDto>>.SuccessResult(result.Value));
            }

            return BadRequest(ApiResponseDto<PagedResultDto<ScheduleResponseDto>>.ErrorResult(result.Error.Description));
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "Database error getting vendor schedules");
            return StatusCode(500, ApiResponseDto<PagedResultDto<ScheduleResponseDto>>.ErrorResult("Database error occurred"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting vendor schedules");
            return StatusCode(500, ApiResponseDto<PagedResultDto<ScheduleResponseDto>>.ErrorResult("An unexpected error occurred"));
        }
    }

    [HttpPost("bulk")]
    public async Task<ActionResult<ApiResponseDto<List<ScheduleResponseDto>>>> CreateBulkSchedule(
        [FromBody] VendorCreateBulkScheduleDto dto)
    {
        try
        {
            var vendorId = await GetVendorIdFromClaimsAsync();
            if (vendorId <= 0)
            {
                return Unauthorized(ApiResponseDto<List<ScheduleResponseDto>>.ErrorResult("Invalid vendor authentication"));
            }

            _logger.LogInformation("Vendor {VendorId} creating bulk schedules for bus {BusId} on route {RouteId} from {StartDate} to {EndDate}", 
                vendorId, dto.BusId, dto.RouteId, dto.StartDate, dto.EndDate);

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(ApiResponseDto<List<ScheduleResponseDto>>.ErrorResult(string.Join("; ", errors)));
            }

            var createDto = new CreateBulkScheduleDto
            {
                BusId = dto.BusId,
                RouteId = dto.RouteId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                DepartureTime = dto.DepartureTime,
                ArrivalTime = dto.ArrivalTime,
                OperatingDays = dto.OperatingDays
            };

            var result = await _scheduleService.CreateBulkScheduleAsync(createDto, vendorId);

            if (result.IsSuccess)
            {
                _logger.LogInformation("Bulk schedules created successfully. Count: {Count}", result.Value.Count);
                return Ok(ApiResponseDto<List<ScheduleResponseDto>>.SuccessResult(result.Value, $"Successfully created {result.Value.Count} schedules"));
            }

            return BadRequest(ApiResponseDto<List<ScheduleResponseDto>>.ErrorResult(result.Error.Description));
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "Database error creating bulk schedules");
            return StatusCode(500, ApiResponseDto<List<ScheduleResponseDto>>.ErrorResult("Database error occurred"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating bulk schedules");
            return StatusCode(500, ApiResponseDto<List<ScheduleResponseDto>>.ErrorResult("An unexpected error occurred"));
        }
    }
}