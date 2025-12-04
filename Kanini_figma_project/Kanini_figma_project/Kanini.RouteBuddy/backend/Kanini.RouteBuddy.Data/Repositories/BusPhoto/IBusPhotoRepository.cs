using Kanini.RouteBuddy.Common.Utility;

namespace Kanini.RouteBuddy.Data.Repositories.BusPhoto;

public interface IBusPhotoRepository
{
    Task<Result<Domain.Entities.BusPhoto>> CreateAsync(Domain.Entities.BusPhoto busPhoto);
    Task<Result<Domain.Entities.BusPhoto>> GetByIdAsync(int busPhotoId);
    Task<Result<IEnumerable<Domain.Entities.BusPhoto>>> GetByBusIdAsync(int busId);
    Task<Result<Domain.Entities.BusPhoto>> UpdateAsync(Domain.Entities.BusPhoto busPhoto);
    Task<Result<bool>> DeleteAsync(int busPhotoId);
    Task<Result<bool>> ExistsAsync(int busPhotoId);
    Task<Result<int>> GetCountByBusIdAsync(int busId);
}