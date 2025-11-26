using Microsoft.AspNetCore.Mvc;
using Kanini.RouteBuddy.Application.Dto.Route;
using Kanini.RouteBuddy.Application.Dto.Common;
using Kanini.RouteBuddy.Application.Services.RouteStop;
using Kanini.RouteBuddy.Common.Errors;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Kanini.RouteBuddy.Api.Controllers;

[ApiController]
[Route("api/routes/{routeId}/stops")]
public class RouteStopController : ControllerBase
{
    private readonly IRouteStopService _routeStopService;

    public RouteStopController(IRouteStopService routeStopService)
    {
        _routeStopService = routeStopService;
    }

    private int GetVendorIdFromClaims()
    {
        if (User?.Identity?.IsAuthenticated != true) return 1;
        var vendorIdClaim = User.FindFirst("VendorId") ?? User.FindFirst(ClaimTypes.NameIdentifier);
        return vendorIdClaim != null && int.TryParse(vendorIdClaim.Value, out int vendorId) ? vendorId : 1;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponseDto<RouteStopDto>>> Create(int routeId, CreateRouteStopDto dto)
    {
        try
        {
            var vendorId = GetVendorIdFromClaims();
            if (vendorId <= 0)
                return Unauthorized(ApiResponseDto<RouteStopDto>.ErrorResult("Invalid vendor authentication"));
                
            if (dto == null)
                return BadRequest(ApiResponseDto<RouteStopDto>.ErrorResult(RouteStopMessages.ValidationError));
            
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(ApiResponseDto<RouteStopDto>.ErrorResult(string.Join("; ", errors)));
            }
            
            var result = await _routeStopService.CreateRouteStopAsync(routeId, dto);
            
            if (result.IsSuccess)
                return CreatedAtAction(nameof(GetById), new { routeId, id = result.Value.RouteStopId }, 
                    ApiResponseDto<RouteStopDto>.SuccessResult(result.Value, RouteStopMessages.RouteStopCreatedSuccessfully));
            
            return BadRequest(ApiResponseDto<RouteStopDto>.ErrorResult(result.Error.Description));
        }
        catch (SqlException)
        {
            return StatusCode(500, ApiResponseDto<RouteStopDto>.ErrorResult(RouteStopMessages.DatabaseError));
        }
        catch (Exception)
        {
            return StatusCode(500, ApiResponseDto<RouteStopDto>.ErrorResult(RouteStopMessages.UnexpectedError));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponseDto<RouteStopDto>>> GetById(int routeId, int id)
    {
        var vendorId = GetVendorIdFromClaims();
        if (vendorId <= 0)
            return Unauthorized(ApiResponseDto<RouteStopDto>.ErrorResult("Invalid vendor authentication"));
            
        var result = await _routeStopService.GetRouteStopByIdAsync(id);
        
        if (result.IsSuccess)
            return Ok(ApiResponseDto<RouteStopDto>.SuccessResult(result.Value));
        
        return NotFound(ApiResponseDto<RouteStopDto>.ErrorResult(result.Error.Description));
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponseDto<IEnumerable<RouteStopDto>>>> GetByRoute(int routeId)
    {
        var vendorId = GetVendorIdFromClaims();
        if (vendorId <= 0)
            return Unauthorized(ApiResponseDto<IEnumerable<RouteStopDto>>.ErrorResult("Invalid vendor authentication"));
            
        var routeStops = await _routeStopService.GetRouteStopsByRouteIdAsync(routeId);
        return Ok(ApiResponseDto<IEnumerable<RouteStopDto>>.SuccessResult(routeStops));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponseDto<RouteStopDto>>> Update(int routeId, int id, UpdateRouteStopDto dto)
    {
        var vendorId = GetVendorIdFromClaims();
        if (vendorId <= 0)
            return Unauthorized(ApiResponseDto<RouteStopDto>.ErrorResult("Invalid vendor authentication"));
            
        var result = await _routeStopService.UpdateRouteStopAsync(id, dto);
        
        if (result.IsSuccess)
            return Ok(ApiResponseDto<RouteStopDto>.SuccessResult(result.Value, RouteStopMessages.RouteStopUpdatedSuccessfully));
        
        return BadRequest(ApiResponseDto<RouteStopDto>.ErrorResult(result.Error.Description));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponseDto<object>>> Delete(int routeId, int id)
    {
        var vendorId = GetVendorIdFromClaims();
        if (vendorId <= 0)
            return Unauthorized(ApiResponseDto<object>.ErrorResult("Invalid vendor authentication"));
            
        var result = await _routeStopService.DeleteRouteStopAsync(id);
        
        if (result.IsSuccess)
            return Ok(ApiResponseDto<object>.SuccessResult(null!, RouteStopMessages.RouteStopDeletedSuccessfully));
        
        return BadRequest(ApiResponseDto<object>.ErrorResult(result.Error.Description));
    }
}