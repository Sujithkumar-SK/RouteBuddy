using Kanini.RouteBuddy.Common.Utility;
using Kanini.RouteBuddy.Domain.Entities;
using StopEntity = Kanini.RouteBuddy.Domain.Entities.Stop;

namespace Kanini.RouteBuddy.Data.Repositories.Stop;

public interface IStopRepository
{
    Task<Result<StopEntity>> CreateAsync(StopEntity stop);
    Task<Result<StopEntity>> GetByIdAsync(int stopId);
    Task<Result<List<StopEntity>>> GetAllAsync(int pageNumber, int pageSize);
    Task<Result<int>> GetCountAsync();
    Task<Result<StopEntity>> UpdateAsync(StopEntity stop);
    Task<Result<bool>> DeleteAsync(int stopId);
    Task<Result<bool>> ExistsByNameAsync(string name);
}