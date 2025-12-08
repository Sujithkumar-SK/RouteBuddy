using Kanini.RouteBuddy.Domain.Entities;

namespace Kanini.RouteBuddy.Data.Repositories.RouteStop;

public interface IRouteStopRepository
{
    Task<Domain.Entities.RouteStop> CreateAsync(Domain.Entities.RouteStop routeStop);
    Task<Domain.Entities.RouteStop?> GetByIdAsync(int routeStopId);
    Task<IEnumerable<Domain.Entities.RouteStop>> GetByRouteIdAsync(int routeId);
    Task<Domain.Entities.RouteStop> UpdateAsync(Domain.Entities.RouteStop routeStop);
    Task<bool> DeleteAsync(int routeStopId);
    Task<bool> ExistsByRouteAndOrderAsync(int routeId, int orderNumber);
    Task<IEnumerable<Domain.Entities.RouteStop>> GetByScheduleIdAsync(int scheduleId);
}