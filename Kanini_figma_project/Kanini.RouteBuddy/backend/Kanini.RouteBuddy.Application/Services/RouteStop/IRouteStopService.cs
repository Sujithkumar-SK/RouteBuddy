using Kanini.RouteBuddy.Application.Dto.Route;
using Kanini.RouteBuddy.Common.Utility;

namespace Kanini.RouteBuddy.Application.Services.RouteStop;

public interface IRouteStopService
{
    Task<Result<RouteStopDto>> CreateRouteStopAsync(int routeId, CreateRouteStopDto dto);
    Task<Result<RouteStopDto>> GetRouteStopByIdAsync(int routeStopId);
    Task<IEnumerable<RouteStopDto>> GetRouteStopsByRouteIdAsync(int routeId);
    Task<Result<RouteStopDto>> UpdateRouteStopAsync(int routeStopId, UpdateRouteStopDto dto);
    Task<Result<bool>> DeleteRouteStopAsync(int routeStopId);
}