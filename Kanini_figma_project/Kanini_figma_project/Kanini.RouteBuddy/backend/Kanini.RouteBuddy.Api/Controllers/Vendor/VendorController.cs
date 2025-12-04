using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Kanini.RouteBuddy.Application.Services.Vendor;
using System.Security.Claims;

namespace Kanini.RouteBuddy.Api.Controllers.Vendor;

[ApiController]
[Route("api/vendor")]
[Authorize(Roles = "Vendor")]
public class VendorController : ControllerBase
{
    private readonly IVendorService _vendorService;
    private readonly ILogger<VendorController> _logger;

    public VendorController(IVendorService vendorService, ILogger<VendorController> logger)
    {
        _vendorService = vendorService;
        _logger = logger;
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
}