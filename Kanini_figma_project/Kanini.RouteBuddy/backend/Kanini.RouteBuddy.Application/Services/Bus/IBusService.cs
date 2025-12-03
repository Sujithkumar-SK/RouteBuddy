
using Kanini.RouteBuddy.Application.Dto.Bus;
using Kanini.RouteBuddy.Application.Dto.Common;
using Kanini.RouteBuddy.Common.Utility;
using Kanini.RouteBuddy.Domain.Enums;

namespace Kanini.RouteBuddy.Application.Services.Buses;

public interface IBusService
{
    Task<Result<BusResponseDto>> CreateBusAsync(CreateBusDto dto, int vendorId);
    Task<Result<BusResponseDto>> GetBusByIdAsync(int busId, int vendorId);
    Task<Result<PagedResultDto<BusResponseDto>>> GetBusesByVendorAsync(int vendorId, int pageNumber, int pageSize, BusStatus? status = null, BusType? busType = null, string? search = null);
    Task<Result<BusResponseDto>> UpdateBusAsync(int busId, UpdateBusDto dto);
    Task<Result<bool>> DeleteBusAsync(int busId);
    Task<Result<BusResponseDto>> ActivateBusAsync(int busId, int vendorId);
    Task<Result<BusResponseDto>> DeactivateBusAsync(int busId, int vendorId);
    Task<Result<BusResponseDto>> SetMaintenanceAsync(int busId, int vendorId);
    Task<Result<List<BusResponseDto>>> GetAwaitingConfirmationAsync(int vendorId);
    Task<Result<bool>> ApplyTemplateAsync(int busId, int templateId, int vendorId);
    Task<Domain.Entities.Vendor?> GetVendorByUserIdAsync(int userId);
}