using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Kanini.RouteBuddy.Application.Dto.Common;
using Kanini.RouteBuddy.Application.Dto.Vendor;
using Kanini.RouteBuddy.Application.Services.Vendor;
using Microsoft.Data.SqlClient;
using System.Security.Claims;

namespace Kanini.RouteBuddy.Api.Controllers.Vendor;

[ApiController]
[Route("api/vendor")]
[Authorize(Roles = "Vendor")]
public class VendorController : ControllerBase
{
    private readonly IVendorService _vendorService;
    private readonly IVendorAnalyticsService _analyticsService;
    private readonly ILogger<VendorController> _logger;

    public VendorController(IVendorService vendorService, IVendorAnalyticsService analyticsService, ILogger<VendorController> logger)
    {
        _vendorService = vendorService;
        _analyticsService = analyticsService;
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

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                return BadRequest(new { error = "Invalid user token" });

            // Get vendor by user ID
            var vendor = await _vendorService.GetVendorByUserIdAsync(userId);
            if (vendor.IsFailure)
                return NotFound(new { error = "Vendor not found" });

            // Get dashboard summary
            var dashboardSummary = await _vendorService.GetVendorDashboardSummaryAsync(vendor.Value.VendorId);

            return Ok(dashboardSummary);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting vendor dashboard for user {UserId}", User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return StatusCode(500, new { error = "An error occurred" });
        }
    }

    [HttpGet("analytics")]
    public async Task<ActionResult<ApiResponseDto<VendorAnalyticsDto>>> GetAnalytics()
    {
        try
        {
            var vendorId = await GetVendorIdFromClaimsAsync();
            if (vendorId <= 0)
            {
                return Unauthorized(ApiResponseDto<VendorAnalyticsDto>.ErrorResult("Invalid vendor authentication"));
            }

            _logger.LogInformation("Getting complete analytics for vendor {VendorId}", vendorId);

            var result = await _analyticsService.GetCompleteAnalyticsAsync(vendorId);

            if (result.IsSuccess)
            {
                return Ok(ApiResponseDto<VendorAnalyticsDto>.SuccessResult(result.Value));
            }

            return BadRequest(ApiResponseDto<VendorAnalyticsDto>.ErrorResult(result.Error.Description));
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "Database error getting vendor analytics");
            return StatusCode(500, ApiResponseDto<VendorAnalyticsDto>.ErrorResult("Database error occurred"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting vendor analytics");
            return StatusCode(500, ApiResponseDto<VendorAnalyticsDto>.ErrorResult("An unexpected error occurred"));
        }
    }
}