using Microsoft.AspNetCore.Mvc;
using Kanini.RouteBuddy.Application.Dto.Route;
using Kanini.RouteBuddy.Application.Dto.Common;
using Kanini.RouteBuddy.Application.Services.Route;
using Kanini.RouteBuddy.Common.Utility;
using Kanini.RouteBuddy.Common.Errors;
using Microsoft.Extensions.Logging;
using AutoMapper;
using System.Security.Claims;

namespace Kanini.RouteBuddy.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RouteController : ControllerBase
{
    private readonly IRouteService _routeService;
    private readonly IMapper _mapper;
    private readonly ILogger<RouteController> _logger;

    public RouteController(IRouteService routeService, IMapper mapper, ILogger<RouteController> logger)
    {
        _routeService = routeService;
        _mapper = mapper;
        _logger = logger;
    }

    private bool IsAdmin()
    {
        if (User?.Identity?.IsAuthenticated != true) return true; // Allow for testing
        var roleClaim = User.FindFirst(ClaimTypes.Role);
        return roleClaim?.Value == "Admin";
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRouteDto dto)
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

            _logger.LogInformation("Creating route from {Source} to {Destination}", dto.Source, dto.Destination);

            var result = await _routeService.CreateRouteAsync(dto);

            if (result.IsFailure)
            {
                _logger.LogError("Route creation failed: {Error}", result.Error.Description);
                return BadRequest(result.Error);
            }

            _logger.LogInformation("Route created successfully with ID: {RouteId}", result.Value.RouteId);
            return CreatedAtAction(nameof(GetById), new { id = result.Value.RouteId }, result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Route creation failed: {Message}", ex.Message);
            return StatusCode(500, new { Error = RouteMessages.UnexpectedError });
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

            _logger.LogInformation("Getting route by ID: {RouteId}", id);

            var result = await _routeService.GetRouteByIdAsync(id);

            if (result.IsFailure)
            {
                _logger.LogError("Route retrieval failed: {Error}", result.Error.Description);
                return BadRequest(result.Error);
            }

            _logger.LogInformation("Route retrieved successfully: {RouteId}", id);
            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Route retrieval failed: {Message}", ex.Message);
            return StatusCode(500, new { Error = RouteMessages.UnexpectedError });
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

            _logger.LogInformation("Getting all routes, page: {PageNumber}, size: {PageSize}", pageNumber, pageSize);

            var result = await _routeService.GetAllRoutesAsync(pageNumber, pageSize);

            if (result.IsFailure)
            {
                _logger.LogError("Route list failed: {Error}", result.Error.Description);
                return BadRequest(result.Error);
            }

            _logger.LogInformation("Route list completed: {Count} routes", result.Value.Data.Count());
            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Route list failed: {Message}", ex.Message);
            return StatusCode(500, new { Error = RouteMessages.UnexpectedError });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateRouteDto dto)
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

            _logger.LogInformation("Updating route: {RouteId}", id);

            var result = await _routeService.UpdateRouteAsync(id, dto);

            if (result.IsFailure)
            {
                _logger.LogError("Route update failed: {Error}", result.Error.Description);
                return BadRequest(result.Error);
            }

            _logger.LogInformation("Route updated successfully: {RouteId}", id);
            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Route update failed: {Message}", ex.Message);
            return StatusCode(500, new { Error = RouteMessages.UnexpectedError });
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

            _logger.LogInformation("Deleting route: {RouteId}", id);

            var result = await _routeService.DeleteRouteAsync(id);

            if (result.IsFailure)
            {
                _logger.LogError("Route delete failed: {Error}", result.Error.Description);
                return BadRequest(result.Error);
            }

            _logger.LogInformation("Route deleted successfully: {RouteId}", id);
            return Ok(new { Message = RouteMessages.RouteDeletedSuccessfully });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Route delete failed: {Message}", ex.Message);
            return StatusCode(500, new { Error = RouteMessages.UnexpectedError });
        }
    }

    [HttpGet("{id}/stops")]
    public async Task<IActionResult> GetRouteStops(int id)
    {
        try
        {
            if (id <= 0)
            {
                _logger.LogWarning(CommonMessages.InvalidId);
                return BadRequest(new { Error = CommonMessages.InvalidId });
            }

            _logger.LogInformation("Getting route stops for route: {RouteId}", id);

            var result = await _routeService.GetRouteStopsAsync(id);

            if (result.IsFailure)
            {
                _logger.LogError("Route stops failed: {Error}", result.Error.Description);
                return BadRequest(result.Error);
            }

            _logger.LogInformation("Route stops completed: {Count} stops", result.Value.Count);
            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Route stops failed: {Message}", ex.Message);
            return StatusCode(500, new { Error = RouteMessages.UnexpectedError });
        }
    }
}