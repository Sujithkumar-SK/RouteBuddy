using Kanini.RouteBuddy.Application.Dto.Bus;
using Kanini.RouteBuddy.Common.Utility;

namespace Kanini.RouteBuddy.Application.Services.Admin;

public interface IAdminBusService
{
    Task<Result<List<BusResponseDto>>> GetAllBusesAsync();
    Task<Result<List<BusResponseDto>>> GetPendingBusesAsync();
    Task<Result<List<BusResponseDto>>> FilterBusesAsync(string? searchName, int? status, bool? isActive);
    Task<Result<BusResponseDto>> GetBusDetailsAsync(int busId);
    Task<Result<bool>> ApproveBusAsync(int busId);
    Task<Result<bool>> RejectBusAsync(int busId, string rejectionReason);
    Task<Result<bool>> DeactivateBusAsync(int busId, string reason);
    Task<Result<bool>> ReactivateBusAsync(int busId, string reason);
}