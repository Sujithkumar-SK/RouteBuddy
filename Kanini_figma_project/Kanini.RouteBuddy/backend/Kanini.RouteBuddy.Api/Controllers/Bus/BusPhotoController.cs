using Microsoft.AspNetCore.Mvc;
using Kanini.RouteBuddy.Application.Services.BusPhoto;
using Kanini.RouteBuddy.Application.Dto.BusPhoto;
using Kanini.RouteBuddy.Common.Utility;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Kanini.RouteBuddy.Application.Services.Buses;

namespace Kanini.RouteBuddy.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Vendor")]
public class BusPhotoController : ControllerBase
{
    private readonly IBusPhotoService _service;
    private readonly IBusService _busService;
    private readonly ILogger<BusPhotoController> _logger;

    public BusPhotoController(IBusPhotoService service, IBusService busService, ILogger<BusPhotoController> logger)
    {
        _service = service;
        _busService = busService;
        _logger = logger;
    }

    private async Task<int> GetVendorIdFromClaimsAsync()
    {
        try
        {
            if (User?.Identity?.IsAuthenticated != true)
                return 0;

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                return 0;

            var vendor = await _busService.GetVendorByUserIdAsync(userId);
            return vendor?.VendorId ?? 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get vendor ID from claims");
            return 0;
        }
    }

    private async Task<bool> ValidateBusOwnership(int busId, int vendorId)
    {
        var busResult = await _busService.GetBusByIdAsync(busId, vendorId);
        return busResult.IsSuccess;
    }

    [HttpPost]
    public async Task<ActionResult<BusPhotoDto>> Create([FromForm] CreateBusPhotoDto createDto)
    {
        try
        {
            var vendorId = await GetVendorIdFromClaimsAsync();
            if (vendorId <= 0)
                return Unauthorized("Invalid vendor authentication");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for bus photo creation");
                return BadRequest(ModelState);
            }

            // Validate bus ownership
            if (!await ValidateBusOwnership(createDto.BusId, vendorId))
            {
                _logger.LogWarning("Vendor {VendorId} attempted to add photo to bus {BusId} they don't own", vendorId, createDto.BusId);
                return Forbid("You can only add photos to your own buses");
            }

            var result = await _service.CreateAsync(createDto);
            
            if (result.IsFailure)
            {
                _logger.LogWarning("Bus photo creation failed: {Error}", result.Error.Description);
                return result.Error.Type switch
                {
                    ErrorType.NotFound => NotFound(result.Error.Description),
                    ErrorType.Conflict => Conflict(result.Error.Description),
                    ErrorType.Validation => BadRequest(result.Error.Description),
                    _ => StatusCode(500, result.Error.Description)
                };
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Value.BusPhotoId }, result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in bus photo creation");
            return StatusCode(500, "An unexpected error occurred");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BusPhotoDto>> GetById(int id)
    {
        try
        {
            var vendorId = await GetVendorIdFromClaimsAsync();
            if (vendorId <= 0)
                return Unauthorized("Invalid vendor authentication");

            if (id <= 0)
            {
                _logger.LogWarning("Invalid bus photo ID: {Id}", id);
                return BadRequest("Bus photo ID must be a positive number");
            }

            var result = await _service.GetByIdAsync(id);
            
            if (result.IsSuccess)
            {
                // Validate bus ownership
                if (!await ValidateBusOwnership(result.Value.BusId, vendorId))
                {
                    _logger.LogWarning("Vendor {VendorId} attempted to access photo {PhotoId} for bus they don't own", vendorId, id);
                    return Forbid("You can only access photos of your own buses");
                }
            }

            result = await _service.GetByIdAsync(id);
            
            if (result.IsFailure)
            {
                return result.Error.Type == ErrorType.NotFound 
                    ? NotFound(result.Error.Description)
                    : StatusCode(500, result.Error.Description);
            }

            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting bus photo by ID: {Id}", id);
            return StatusCode(500, "An unexpected error occurred");
        }
    }

    [HttpGet("bus/{busId}")]
    public async Task<ActionResult<IEnumerable<BusPhotoDto>>> GetByBusId(int busId)
    {
        try
        {
            var vendorId = await GetVendorIdFromClaimsAsync();
            if (vendorId <= 0)
                return Unauthorized("Invalid vendor authentication");

            if (busId <= 0)
            {
                _logger.LogWarning("Invalid bus ID: {BusId}", busId);
                return BadRequest("Bus ID must be a positive number");
            }

            // Validate bus ownership
            if (!await ValidateBusOwnership(busId, vendorId))
            {
                _logger.LogWarning("Vendor {VendorId} attempted to access photos for bus {BusId} they don't own", vendorId, busId);
                return Forbid("You can only access photos of your own buses");
            }

            var result = await _service.GetByBusIdAsync(busId);
            
            if (result.IsFailure)
            {
                return StatusCode(500, result.Error.Description);
            }

            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting bus photos by bus ID: {BusId}", busId);
            return StatusCode(500, "An unexpected error occurred");
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<BusPhotoDto>> Update(int id, UpdateBusPhotoDto updateDto)
    {
        try
        {
            var vendorId = await GetVendorIdFromClaimsAsync();
            if (vendorId <= 0)
                return Unauthorized("Invalid vendor authentication");

            if (id <= 0)
            {
                _logger.LogWarning("Invalid bus photo ID: {Id}", id);
                return BadRequest("Bus photo ID must be a positive number");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for bus photo update");
                return BadRequest(ModelState);
            }

            // Get photo to validate bus ownership
            var photoResult = await _service.GetByIdAsync(id);
            if (photoResult.IsFailure)
            {
                return photoResult.Error.Type == ErrorType.NotFound 
                    ? NotFound(photoResult.Error.Description)
                    : StatusCode(500, photoResult.Error.Description);
            }

            // Validate bus ownership
            if (!await ValidateBusOwnership(photoResult.Value.BusId, vendorId))
            {
                _logger.LogWarning("Vendor {VendorId} attempted to update photo {PhotoId} for bus they don't own", vendorId, id);
                return Forbid("You can only update photos of your own buses");
            }

            var result = await _service.UpdateAsync(id, updateDto);
            
            if (result.IsFailure)
            {
                return result.Error.Type == ErrorType.NotFound 
                    ? NotFound(result.Error.Description)
                    : StatusCode(500, result.Error.Description);
            }

            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error updating bus photo: {Id}", id);
            return StatusCode(500, "An unexpected error occurred");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var vendorId = await GetVendorIdFromClaimsAsync();
            if (vendorId <= 0)
                return Unauthorized("Invalid vendor authentication");

            if (id <= 0)
            {
                _logger.LogWarning("Invalid bus photo ID: {Id}", id);
                return BadRequest("Bus photo ID must be a positive number");
            }

            // Get photo to validate bus ownership
            var photoResult = await _service.GetByIdAsync(id);
            if (photoResult.IsFailure)
            {
                return photoResult.Error.Type == ErrorType.NotFound 
                    ? NotFound(photoResult.Error.Description)
                    : StatusCode(500, photoResult.Error.Description);
            }

            // Validate bus ownership
            if (!await ValidateBusOwnership(photoResult.Value.BusId, vendorId))
            {
                _logger.LogWarning("Vendor {VendorId} attempted to delete photo {PhotoId} for bus they don't own", vendorId, id);
                return Forbid("You can only delete photos of your own buses");
            }

            var result = await _service.DeleteAsync(id);
            
            if (result.IsFailure)
            {
                return result.Error.Type == ErrorType.NotFound 
                    ? NotFound(result.Error.Description)
                    : StatusCode(500, result.Error.Description);
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error deleting bus photo: {Id}", id);
            return StatusCode(500, "An unexpected error occurred");
        }
    }
}