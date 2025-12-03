using Kanini.RouteBuddy.Application.Dto.Route;
using Kanini.RouteBuddy.Application.Dto.Common;
using Kanini.RouteBuddy.Application.Dto.Stop;
using Kanini.RouteBuddy.Common.Utility;

namespace Kanini.RouteBuddy.Application.Services.Route;

public interface IRouteService
{
    Task<Result<RouteResponseDto>> CreateRouteAsync(CreateRouteDto dto);
    Task<Result<RouteResponseDto>> GetRouteByIdAsync(int routeId);
    Task<Result<PagedResultDto<RouteResponseDto>>> GetAllRoutesAsync(int pageNumber, int pageSize);
    Task<Result<RouteResponseDto>> UpdateRouteAsync(int routeId, UpdateRouteDto dto);
    Task<Result<bool>> DeleteRouteAsync(int routeId);
    Task<Result<List<RouteStopDto>>> GetRouteStopsAsync(int routeId);
    Task<Result<List<RouteSearchDto>>> SearchRoutesAsync(string? source, string? destination, int limit);
    Task<Result<List<RouteSearchDto>>> GetAllActiveRoutesAsync();
    Task<Result<List<RouteStopDetailDto>>> GetRouteStopsWithDetailsAsync(int routeId);
    Task<Result<List<StopResponseDto>>> GetAllActiveStopsAsync();
    Task<Result<bool>> CreateRouteStopsAsync(int routeId, List<CreateRouteStopRequest> stops, int vendorId);
    Task<Result<bool>> CreateScheduleRouteStopsAsync(int scheduleId, List<CreateRouteStopRequest> stops);
}