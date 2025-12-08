using Kanini.RouteBuddy.Common.Utility;
using Kanini.RouteBuddy.Domain.Entities;
using Kanini.RouteBuddy.Data.Models;

namespace Kanini.RouteBuddy.Data.Repositories.Route;

public interface IRouteRepository
{
    Task<Result<Domain.Entities.Route>> CreateAsync(Domain.Entities.Route route);
    Task<Result<Domain.Entities.Route>> GetByIdAsync(int routeId);
    Task<Result<List<Domain.Entities.Route>>> GetAllAsync(int pageNumber, int pageSize);
    Task<Result<int>> GetTotalCountAsync();
    Task<Result<Domain.Entities.Route>> UpdateAsync(Domain.Entities.Route route);
    Task<Result<bool>> DeleteAsync(int routeId);
    Task<Result<bool>> ExistsBySourceDestinationAsync(string source, string destination);
    Task<Result<List<Domain.Entities.RouteStop>>> GetRouteStopsAsync(int routeId);
    Task<Result<List<RouteSearchResult>>> SearchRoutesAsync(string? source, string? destination, int limit);
    Task<Result<List<RouteSearchResult>>> GetAllActiveRoutesAsync();
    Task<Result<List<RouteStopDetailResult>>> GetRouteStopsWithDetailsAsync(int routeId);
    Task<Result<List<Domain.Entities.Stop>>> GetAllActiveStopsAsync();
    Task<Result<bool>> CreateRouteStopsAsync(int routeId, List<RouteStopCreationModel> stops, int vendorId);
    Task<Result<bool>> CreateScheduleRouteStopsAsync(int scheduleId, List<RouteStopCreationModel> stops);
}

public class RouteSearchResult
{
    public int RouteId { get; set; }
    public string Source { get; set; } = null!;
    public string Destination { get; set; } = null!;
    public decimal Distance { get; set; }
    public TimeSpan Duration { get; set; }
    public decimal BasePrice { get; set; }
    public int TotalStops { get; set; }
    public bool IsActive { get; set; }
}

public class RouteStopDetailResult
{
    public int RouteStopId { get; set; }
    public int StopId { get; set; }
    public string StopName { get; set; } = null!;
    public string? Landmark { get; set; }
    public int OrderNumber { get; set; }
    public TimeSpan? ArrivalTime { get; set; }
    public TimeSpan? DepartureTime { get; set; }
}