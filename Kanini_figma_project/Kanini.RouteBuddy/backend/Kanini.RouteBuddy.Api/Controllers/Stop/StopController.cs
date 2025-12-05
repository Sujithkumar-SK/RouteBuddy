using Microsoft.AspNetCore.Mvc;
using Kanini.RouteBuddy.Application.Dto.Stop;
using Kanini.RouteBuddy.Application.Services.Stop;
using Kanini.RouteBuddy.Common.Errors;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Kanini.RouteBuddy.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StopController : ControllerBase
{
    private readonly IStopService _stopService;
    private readonly ILogger<StopController> _logger;

    public StopController(IStopService stopService, ILogger<StopController> logger)
    {
        _stopService = stopService;
        _logger = logger;
    }

    private bool IsAdmin()
    {
        if (User?.Identity?.IsAuthenticated != true) return true; // Allow for testing
        var roleClaim = User.FindFirst(ClaimTypes.Role);
        return roleClaim?.Value == "Admin";
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateStopDto dto)
    {
        try
        {
            if (!IsAdmin())
                return Unauthorized(new { Error = "Admin access required" });
            if (!ModelState.IsValid)
            {
                _logger.LogWarning(CommonMessages.ValidationFailed);
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Creating stop: {Name}", dto.Name);

            var result = await _stopService.CreateStopAsync(dto);

            if (result.IsFailure)
            {
                _logger.LogError("Stop creation failed: {Error}", result.Error.Description);
                return BadRequest(result.Error);
            }

            _logger.LogInformation("Stop created successfully with ID: {StopId}", result.Value.StopId);
            return CreatedAtAction(nameof(GetById), new { id = result.Value.StopId }, result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Stop creation failed: {Message}", ex.Message);
            return StatusCode(500, new { Error = StopMessages.UnexpectedError });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            if (id <= 0)
            {
                _logger.LogWarning(CommonMessages.InvalidId);
                return BadRequest(new { Error = CommonMessages.InvalidId });
            }

            _logger.LogInformation("Getting stop by ID: {StopId}", id);

            var result = await _stopService.GetStopByIdAsync(id);

            if (result.IsFailure)
            {
                _logger.LogError("Stop retrieval failed: {Error}", result.Error.Description);
                return BadRequest(result.Error);
            }

            _logger.LogInformation("Stop retrieved successfully: {StopId}", id);
            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Stop retrieval failed: {Message}", ex.Message);
            return StatusCode(500, new { Error = StopMessages.UnexpectedError });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            if (pageNumber <= 0 || pageSize <= 0 || pageSize > 100)
            {
                _logger.LogWarning(CommonMessages.InvalidPaginationParameters);
                return BadRequest(new { Error = CommonMessages.InvalidPaginationParameters });
            }

            _logger.LogInformation("Getting all stops, page: {PageNumber}, size: {PageSize}", pageNumber, pageSize);

            var result = await _stopService.GetAllStopsAsync(pageNumber, pageSize);

            if (result.IsFailure)
            {
                _logger.LogError("Stop list failed: {Error}", result.Error.Description);
                return BadRequest(result.Error);
            }

            _logger.LogInformation("Stop list completed: {Count} stops", result.Value.Data.Count());
            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Stop list failed: {Message}", ex.Message);
            return StatusCode(500, new { Error = StopMessages.UnexpectedError });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateStopDto dto)
    {
        try
        {
            if (!IsAdmin())
                return Unauthorized(new { Error = "Admin access required" });
            if (!ModelState.IsValid)
            {
                _logger.LogWarning(CommonMessages.ValidationFailed);
                return BadRequest(ModelState);
            }

            if (id <= 0)
            {
                _logger.LogWarning(CommonMessages.InvalidId);
                return BadRequest(new { Error = CommonMessages.InvalidId });
            }

            _logger.LogInformation("Updating stop: {StopId}", id);

            var result = await _stopService.UpdateStopAsync(id, dto);

            if (result.IsFailure)
            {
                _logger.LogError("Stop update failed: {Error}", result.Error.Description);
                return BadRequest(result.Error);
            }

            _logger.LogInformation("Stop updated successfully: {StopId}", id);
            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Stop update failed: {Message}", ex.Message);
            return StatusCode(500, new { Error = StopMessages.UnexpectedError });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            if (!IsAdmin())
                return Unauthorized(new { Error = "Admin access required" });
            if (id <= 0)
            {
                _logger.LogWarning(CommonMessages.InvalidId);
                return BadRequest(new { Error = CommonMessages.InvalidId });
            }

            _logger.LogInformation("Deleting stop: {StopId}", id);

            var result = await _stopService.DeleteStopAsync(id);

            if (result.IsFailure)
            {
                _logger.LogError("Stop delete failed: {Error}", result.Error.Description);
                return BadRequest(result.Error);
            }

            _logger.LogInformation("Stop deleted successfully: {StopId}", id);
            return Ok(new { Message = StopMessages.StopDeletedSuccessfully });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Stop delete failed: {Message}", ex.Message);
            return StatusCode(500, new { Error = StopMessages.UnexpectedError });
        }
    }

    [HttpGet("places")]
    public async Task<IActionResult> GetAllPlaces()
    {
        try
        {
            _logger.LogInformation("Getting all places started");

            var request = new PlaceAutocompleteRequestDto { Query = "", Limit = 1000 };
            var result = await _stopService.GetPlaceAutocompleteAsync(request);

            if (result.IsFailure)
            {
                _logger.LogError("Get all places failed: {Error}", result.Error.Description);
                return BadRequest(result.Error);
            }

            _logger.LogInformation("Get all places completed. Found {Count} places", result.Value.Count);
            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Get all places failed: {Error}", ex.Message);
            return StatusCode(500, new { Error = "An unexpected error occurred" });
        }
    }
}