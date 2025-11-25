using Kanini.RouteBuddy.Domain.Entities;
using Kanini.RouteBuddy.Common.Utility;
using BusSchedule = Kanini.RouteBuddy.Domain.Entities.BusSchedule;

namespace Kanini.RouteBuddy.Data.Repositories.Schedule;

public interface IScheduleRepository
{
    Task<Result<BusSchedule>> CreateAsync(BusSchedule schedule);
    Task<Result<BusSchedule>> GetByIdAsync(int scheduleId);
    Task<Result<List<BusSchedule>>> GetByVendorIdAsync(int vendorId, int pageNumber, int pageSize);
    Task<Result<int>> GetCountByVendorIdAsync(int vendorId);
    Task<Result<BusSchedule>> UpdateAsync(BusSchedule schedule);
    Task<Result<bool>> DeleteAsync(int scheduleId);
    Task<Result<bool>> ExistsByBusRouteAndDateAsync(int busId, int routeId, DateTime travelDate);
}