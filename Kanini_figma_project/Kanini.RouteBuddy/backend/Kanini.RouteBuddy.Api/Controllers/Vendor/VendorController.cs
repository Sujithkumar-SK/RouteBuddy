using System.Security.Claims;
using Kanini.RouteBuddy.Application.Dto.Common;
using Kanini.RouteBuddy.Application.Dto.Vendor;
using Kanini.RouteBuddy.Application.Services.Vendor;
using Kanini.RouteBuddy.Application.Validators;
using Kanini.RouteBuddy.Common.Errors;
using Kanini.RouteBuddy.Common.Services;
using Kanini.RouteBuddy.Common.Validators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Kanini.RouteBuddy.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VendorController : ControllerBase
{
    private readonly IVendorService _vendorService;
    private readonly IVendorAnalyticsService _analyticsService;

    public VendorController(IVendorService vendorService, IVendorAnalyticsService analyticsService)
    {
        _vendorService = vendorService;
        _analyticsService = analyticsService;
    }

    private int GetVendorIdFromClaims()
    {
        if (User?.Identity?.IsAuthenticated != true)
            return 1;
        var vendorIdClaim = User.FindFirst("VendorId") ?? User.FindFirst(ClaimTypes.NameIdentifier);
        return vendorIdClaim != null && int.TryParse(vendorIdClaim.Value, out int vendorId)
            ? vendorId
            : 1;
    }

    [HttpGet("dashboard")]
    public async Task<ActionResult<ApiResponseDto<VendorDashboardSummaryDto>>> GetDashboardSummary()
    {
        try
        {
            var id = GetVendorIdFromClaims();
            if (id <= 0)
                return Unauthorized(
                    ApiResponseDto<VendorDashboardSummaryDto>.ErrorResult(
                        "Invalid vendor authentication"
                    )
                );

            VendorFileLogger.LogInfo(VendorMessages.LogMessages.GettingDashboard, id);

            var result = await _vendorService.GetVendorDashboardSummaryAsync(id);
            VendorFileLogger.LogInfo(VendorMessages.LogMessages.DashboardRetrieved, id);
            return Ok(
                ApiResponseDto<VendorDashboardSummaryDto>.SuccessResult(
                    result,
                    VendorMessages.DashboardRetrievedSuccessfully
                )
            );
        }
        catch (SqlException ex)
        {
            VendorFileLogger.LogError(
                VendorMessages.LogMessages.SqlErrorDashboard,
                ex,
                GetVendorIdFromClaims()
            );
            return StatusCode(
                500,
                ApiResponseDto<VendorDashboardSummaryDto>.ErrorResult(VendorMessages.DatabaseError)
            );
        }
        catch (DbUpdateException ex)
        {
            VendorFileLogger.LogError(
                VendorMessages.LogMessages.SqlErrorDashboard,
                ex,
                GetVendorIdFromClaims()
            );
            return StatusCode(
                500,
                ApiResponseDto<VendorDashboardSummaryDto>.ErrorResult(VendorMessages.DatabaseError)
            );
        }
        catch (Exception ex)
        {
            VendorFileLogger.LogError(
                VendorMessages.LogMessages.UnexpectedErrorDashboard,
                ex,
                GetVendorIdFromClaims()
            );
            return StatusCode(
                500,
                ApiResponseDto<VendorDashboardSummaryDto>.ErrorResult(
                    VendorMessages.UnexpectedError
                )
            );
        }
    }

    [HttpPut("profile")]
    public async Task<ActionResult<ApiResponseDto<VendorResponseDto>>> UpdateProfile(
        UpdateVendorProfileDto dto
    )
    {
        try
        {
            var id = GetVendorIdFromClaims();
            if (id <= 0)
                return Unauthorized(
                    ApiResponseDto<VendorResponseDto>.ErrorResult("Invalid vendor authentication")
                );

            VendorFileLogger.LogInfo(VendorMessages.LogMessages.UpdatingProfile, id);

            if (dto == null)
                return BadRequest(
                    ApiResponseDto<VendorResponseDto>.ErrorResult("Request data is required")
                );

            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);
                return BadRequest(
                    ApiResponseDto<VendorResponseDto>.ErrorResult(string.Join("; ", errors))
                );
            }

            var validationErrors = VendorValidator.ValidateProfileUpdate(dto);
            if (validationErrors.Any())
                return BadRequest(
                    ApiResponseDto<VendorResponseDto>.ErrorResult(
                        string.Join("; ", validationErrors)
                    )
                );

            var result = await _vendorService.UpdateVendorProfileAsync(id, dto);

            if (result.IsSuccess)
            {
                VendorFileLogger.LogInfo(VendorMessages.LogMessages.ProfileUpdated, id);
                return Ok(
                    ApiResponseDto<VendorResponseDto>.SuccessResult(
                        result.Value,
                        VendorMessages.ProfileUpdatedSuccessfully
                    )
                );
            }

            return BadRequest(
                ApiResponseDto<VendorResponseDto>.ErrorResult(result.Error.Description)
            );
        }
        catch (SqlException ex)
        {
            VendorFileLogger.LogError(
                VendorMessages.LogMessages.SqlErrorProfile,
                ex,
                GetVendorIdFromClaims()
            );
            return StatusCode(
                500,
                ApiResponseDto<VendorResponseDto>.ErrorResult(VendorMessages.DatabaseError)
            );
        }
        catch (DbUpdateException ex)
        {
            VendorFileLogger.LogError(
                VendorMessages.LogMessages.SqlErrorProfile,
                ex,
                GetVendorIdFromClaims()
            );
            return StatusCode(
                500,
                ApiResponseDto<VendorResponseDto>.ErrorResult(VendorMessages.DatabaseError)
            );
        }
        catch (Exception ex)
        {
            VendorFileLogger.LogError(
                VendorMessages.LogMessages.UnexpectedErrorProfile,
                ex,
                GetVendorIdFromClaims()
            );
            return StatusCode(
                500,
                ApiResponseDto<VendorResponseDto>.ErrorResult(VendorMessages.UnexpectedError)
            );
        }
    }

    [HttpGet("me")]
    public async Task<ActionResult<ApiResponseDto<VendorResponseDto>>> GetMyProfile()
    {
        try
        {
            VendorFileLogger.LogInfo(VendorMessages.LogMessages.GettingProfile);

            var vendorId = GetVendorIdFromClaims();
            if (vendorId <= 0)
                return Unauthorized(
                    ApiResponseDto<VendorResponseDto>.ErrorResult("Invalid vendor authentication")
                );

            var result = await _vendorService.GetVendorByIdAsync(vendorId);

            if (result.IsSuccess)
            {
                VendorFileLogger.LogInfo(VendorMessages.LogMessages.ProfileRetrieved, vendorId);
                return Ok(ApiResponseDto<VendorResponseDto>.SuccessResult(result.Value));
            }

            return NotFound(
                ApiResponseDto<VendorResponseDto>.ErrorResult(result.Error.Description)
            );
        }
        catch (SqlException ex)
        {
            VendorFileLogger.LogError(VendorMessages.LogMessages.SqlErrorGetProfile, ex);
            return StatusCode(
                500,
                ApiResponseDto<VendorResponseDto>.ErrorResult(VendorMessages.DatabaseError)
            );
        }
        catch (Exception ex)
        {
            VendorFileLogger.LogError(VendorMessages.LogMessages.UnexpectedErrorGetProfile, ex);
            return StatusCode(
                500,
                ApiResponseDto<VendorResponseDto>.ErrorResult(VendorMessages.UnexpectedError)
            );
        }
    }

    [HttpGet("performance")]
    public async Task<ActionResult<ApiResponseDto<object>>> GetPerformanceMetrics()
    {
        try
        {
            var id = GetVendorIdFromClaims();
            if (id <= 0)
                return Unauthorized(
                    ApiResponseDto<object>.ErrorResult("Invalid vendor authentication")
                );

            VendorFileLogger.LogInfo("Controller: Getting performance metrics for vendor {0}", id);

            var result = await _analyticsService.GetPerformanceMetricsAsync(id);
            VendorFileLogger.LogInfo(
                "Controller: Service returned IsSuccess={0}",
                result.IsSuccess
            );

            if (result.IsSuccess)
            {
                VendorFileLogger.LogInfo(
                    VendorMessages.LogMessages.PerformanceMetricsRetrieved,
                    id
                );
                return Ok(ApiResponseDto<object>.SuccessResult(result.Value));
            }

            VendorFileLogger.LogError(
                "Controller: Service returned error: {0}",
                null,
                result.Error?.Description ?? "Unknown error"
            );
            return BadRequest(
                ApiResponseDto<object>.ErrorResult(result.Error?.Description ?? "Unknown error")
            );
        }
        catch (SqlException ex)
        {
            VendorFileLogger.LogError(
                VendorMessages.LogMessages.SqlErrorPerformanceMetrics,
                ex,
                GetVendorIdFromClaims()
            );
            return StatusCode(
                500,
                ApiResponseDto<object>.ErrorResult(VendorMessages.DatabaseError)
            );
        }
        catch (Exception ex)
        {
            VendorFileLogger.LogError(
                VendorMessages.LogMessages.UnexpectedErrorPerformanceMetrics,
                ex,
                GetVendorIdFromClaims()
            );
            return StatusCode(
                500,
                ApiResponseDto<object>.ErrorResult(VendorMessages.UnexpectedError)
            );
        }
    }

    [HttpGet("notifications")]
    public async Task<ActionResult<ApiResponseDto<List<object>>>> GetNotifications()
    {
        try
        {
            var id = GetVendorIdFromClaims();
            if (id <= 0)
                return Unauthorized(
                    ApiResponseDto<List<object>>.ErrorResult("Invalid vendor authentication")
                );

            VendorFileLogger.LogInfo(VendorMessages.LogMessages.GettingNotifications, id);

            var result = await _analyticsService.GetNotificationsAsync(id);
            if (result.IsSuccess)
            {
                VendorFileLogger.LogInfo(VendorMessages.LogMessages.NotificationsRetrieved, id);
                return Ok(ApiResponseDto<List<object>>.SuccessResult(result.Value));
            }
            return BadRequest(ApiResponseDto<List<object>>.ErrorResult(result.Error.Description));
        }
        catch (SqlException ex)
        {
            VendorFileLogger.LogError(
                VendorMessages.LogMessages.SqlErrorNotifications,
                ex,
                GetVendorIdFromClaims()
            );
            return StatusCode(
                500,
                ApiResponseDto<List<object>>.ErrorResult(VendorMessages.DatabaseError)
            );
        }
        catch (Exception ex)
        {
            VendorFileLogger.LogError(
                VendorMessages.LogMessages.UnexpectedErrorNotifications,
                ex,
                GetVendorIdFromClaims()
            );
            return StatusCode(
                500,
                ApiResponseDto<List<object>>.ErrorResult(VendorMessages.UnexpectedError)
            );
        }
    }

    [HttpGet("revenue-analytics")]
    public async Task<ActionResult<ApiResponseDto<object>>> GetRevenueAnalytics()
    {
        try
        {
            var id = GetVendorIdFromClaims();
            if (id <= 0)
                return Unauthorized(
                    ApiResponseDto<object>.ErrorResult("Invalid vendor authentication")
                );

            VendorFileLogger.LogInfo(VendorMessages.LogMessages.GettingRevenueAnalytics, id);

            var result = await _analyticsService.GetRevenueAnalyticsAsync(id);
            if (result.IsSuccess)
            {
                VendorFileLogger.LogInfo(VendorMessages.LogMessages.RevenueAnalyticsRetrieved, id);
                return Ok(ApiResponseDto<object>.SuccessResult(result.Value));
            }
            return BadRequest(ApiResponseDto<object>.ErrorResult(result.Error.Description));
        }
        catch (SqlException ex)
        {
            VendorFileLogger.LogError(
                VendorMessages.LogMessages.SqlErrorRevenueAnalytics,
                ex,
                GetVendorIdFromClaims()
            );
            return StatusCode(
                500,
                ApiResponseDto<object>.ErrorResult(VendorMessages.DatabaseError)
            );
        }
        catch (Exception ex)
        {
            VendorFileLogger.LogError(
                VendorMessages.LogMessages.UnexpectedErrorRevenueAnalytics,
                ex,
                GetVendorIdFromClaims()
            );
            return StatusCode(
                500,
                ApiResponseDto<object>.ErrorResult(VendorMessages.UnexpectedError)
            );
        }
    }

    [HttpGet("fleet-status")]
    public async Task<ActionResult<ApiResponseDto<object>>> GetFleetStatus()
    {
        try
        {
            var id = GetVendorIdFromClaims();
            if (id <= 0)
                return Unauthorized(
                    ApiResponseDto<object>.ErrorResult("Invalid vendor authentication")
                );

            VendorFileLogger.LogInfo(VendorMessages.LogMessages.GettingFleetStatus, id);

            var result = await _analyticsService.GetFleetStatusAsync(id);
            if (result.IsSuccess)
            {
                VendorFileLogger.LogInfo(VendorMessages.LogMessages.FleetStatusRetrieved, id);
                return Ok(ApiResponseDto<object>.SuccessResult(result.Value));
            }
            return BadRequest(ApiResponseDto<object>.ErrorResult(result.Error.Description));
        }
        catch (SqlException ex)
        {
            VendorFileLogger.LogError(
                VendorMessages.LogMessages.SqlErrorFleetStatus,
                ex,
                GetVendorIdFromClaims()
            );
            return StatusCode(
                500,
                ApiResponseDto<object>.ErrorResult(VendorMessages.DatabaseError)
            );
        }
        catch (Exception ex)
        {
            VendorFileLogger.LogError(
                VendorMessages.LogMessages.UnexpectedErrorFleetStatus,
                ex,
                GetVendorIdFromClaims()
            );
            return StatusCode(
                500,
                ApiResponseDto<object>.ErrorResult(VendorMessages.UnexpectedError)
            );
        }
    }

    [HttpGet("maintenance-schedule")]
    public async Task<ActionResult<ApiResponseDto<object>>> GetMaintenanceSchedule()
    {
        try
        {
            var id = GetVendorIdFromClaims();
            if (id <= 0)
                return Unauthorized(
                    ApiResponseDto<object>.ErrorResult("Invalid vendor authentication")
                );

            VendorFileLogger.LogInfo(VendorMessages.LogMessages.GettingMaintenanceSchedule, id);

            var result = await _analyticsService.GetMaintenanceScheduleAsync(id);
            if (result.IsSuccess)
            {
                VendorFileLogger.LogInfo(
                    VendorMessages.LogMessages.MaintenanceScheduleRetrieved,
                    id
                );
                return Ok(ApiResponseDto<object>.SuccessResult(result.Value));
            }
            return BadRequest(ApiResponseDto<object>.ErrorResult(result.Error.Description));
        }
        catch (SqlException ex)
        {
            VendorFileLogger.LogError(
                VendorMessages.LogMessages.SqlErrorMaintenanceSchedule,
                ex,
                GetVendorIdFromClaims()
            );
            return StatusCode(
                500,
                ApiResponseDto<object>.ErrorResult(VendorMessages.DatabaseError)
            );
        }
        catch (Exception ex)
        {
            VendorFileLogger.LogError(
                VendorMessages.LogMessages.UnexpectedErrorMaintenanceSchedule,
                ex,
                GetVendorIdFromClaims()
            );
            return StatusCode(
                500,
                ApiResponseDto<object>.ErrorResult(VendorMessages.UnexpectedError)
            );
        }
    }

    [HttpGet("recent-bookings")]
    public async Task<ActionResult<ApiResponseDto<object>>> GetRecentBookings()
    {
        try
        {
            var id = GetVendorIdFromClaims();
            if (id <= 0)
                return Unauthorized(
                    ApiResponseDto<object>.ErrorResult("Invalid vendor authentication")
                );

            VendorFileLogger.LogInfo(VendorMessages.LogMessages.GettingRecentBookings, id);

            var result = await _analyticsService.GetRecentBookingsAsync(id);
            if (result.IsSuccess)
            {
                VendorFileLogger.LogInfo(VendorMessages.LogMessages.RecentBookingsRetrieved, id);
                return Ok(ApiResponseDto<object>.SuccessResult(result.Value));
            }
            return BadRequest(ApiResponseDto<object>.ErrorResult(result.Error.Description));
        }
        catch (SqlException ex)
        {
            VendorFileLogger.LogError(
                VendorMessages.LogMessages.SqlErrorRecentBookings,
                ex,
                GetVendorIdFromClaims()
            );
            return StatusCode(
                500,
                ApiResponseDto<object>.ErrorResult(VendorMessages.DatabaseError)
            );
        }
        catch (Exception ex)
        {
            VendorFileLogger.LogError(
                VendorMessages.LogMessages.UnexpectedErrorRecentBookings,
                ex,
                GetVendorIdFromClaims()
            );
            return StatusCode(
                500,
                ApiResponseDto<object>.ErrorResult(VendorMessages.UnexpectedError)
            );
        }
    }

    [HttpGet("quick-stats")]
    public async Task<ActionResult<ApiResponseDto<object>>> GetQuickStats()
    {
        try
        {
            var id = GetVendorIdFromClaims();
            if (id <= 0)
                return Unauthorized(
                    ApiResponseDto<object>.ErrorResult("Invalid vendor authentication")
                );

            VendorFileLogger.LogInfo(VendorMessages.LogMessages.GettingQuickStats, id);

            var result = await _analyticsService.GetQuickStatsAsync(id);
            if (result.IsSuccess)
            {
                VendorFileLogger.LogInfo(VendorMessages.LogMessages.QuickStatsRetrieved, id);
                return Ok(ApiResponseDto<object>.SuccessResult(result.Value));
            }
            return BadRequest(ApiResponseDto<object>.ErrorResult(result.Error.Description));
        }
        catch (SqlException ex)
        {
            VendorFileLogger.LogError(
                VendorMessages.LogMessages.SqlErrorQuickStats,
                ex,
                GetVendorIdFromClaims()
            );
            return StatusCode(
                500,
                ApiResponseDto<object>.ErrorResult(VendorMessages.DatabaseError)
            );
        }
        catch (Exception ex)
        {
            VendorFileLogger.LogError(
                VendorMessages.LogMessages.UnexpectedErrorQuickStats,
                ex,
                GetVendorIdFromClaims()
            );
            return StatusCode(
                500,
                ApiResponseDto<object>.ErrorResult(VendorMessages.UnexpectedError)
            );
        }
    }

    [HttpGet("alerts")]
    public async Task<ActionResult<ApiResponseDto<object>>> GetAlerts()
    {
        try
        {
            var id = GetVendorIdFromClaims();
            if (id <= 0)
                return Unauthorized(
                    ApiResponseDto<object>.ErrorResult("Invalid vendor authentication")
                );

            VendorFileLogger.LogInfo(VendorMessages.LogMessages.GettingAlerts, id);

            var result = await _analyticsService.GetAlertsAsync(id);
            if (result.IsSuccess)
            {
                VendorFileLogger.LogInfo(VendorMessages.LogMessages.AlertsRetrieved, id);
                return Ok(ApiResponseDto<object>.SuccessResult(result.Value));
            }
            return BadRequest(ApiResponseDto<object>.ErrorResult(result.Error.Description));
        }
        catch (SqlException ex)
        {
            VendorFileLogger.LogError(
                VendorMessages.LogMessages.SqlErrorAlerts,
                ex,
                GetVendorIdFromClaims()
            );
            return StatusCode(
                500,
                ApiResponseDto<object>.ErrorResult(VendorMessages.DatabaseError)
            );
        }
        catch (Exception ex)
        {
            VendorFileLogger.LogError(
                VendorMessages.LogMessages.UnexpectedErrorAlerts,
                ex,
                GetVendorIdFromClaims()
            );
            return StatusCode(
                500,
                ApiResponseDto<object>.ErrorResult(VendorMessages.UnexpectedError)
            );
        }
    }
}
