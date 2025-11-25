using Kanini.RouteBuddy.Common.Utility;
using Kanini.RouteBuddy.Domain.Entities;

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
}