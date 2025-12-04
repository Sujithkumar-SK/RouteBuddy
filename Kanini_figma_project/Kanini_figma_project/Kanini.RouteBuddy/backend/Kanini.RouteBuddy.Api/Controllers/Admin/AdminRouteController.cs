using Microsoft.AspNetCore.Mvc;
using Kanini.RouteBuddy.Data.Repositories.Route;
using Microsoft.Extensions.Logging;

namespace Kanini.RouteBuddy.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/routes")]
public class AdminRouteController : ControllerBase
{
    private readonly IRouteRepository _routeRepository;
    private readonly ILogger<AdminRouteController> _logger;

    public AdminRouteController(IRouteRepository routeRepository, ILogger<AdminRouteController> logger)
    {
        _routeRepository = routeRepository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllRoutes([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 100)
    {
        try
        {
            var result = await _routeRepository.GetAllAsync(pageNumber, pageSize);
            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(new { data = result.Value });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get routes");
            return StatusCode(500, new { Error = "Failed to retrieve routes" });
        }
    }
}