using Kanini.RouteBuddy.Common.Utility;
using Kanini.RouteBuddy.Domain.Entities;
using Kanini.RouteBuddy.Domain.Enums;
using BusEntity = Kanini.RouteBuddy.Domain.Entities.Bus;

namespace Kanini.RouteBuddy.Data.Repositories.Buses;

public interface IBusRepository
{
    Task<Result<BusEntity>> CreateAsync(BusEntity bus);
    Task<Result<BusEntity>> GetByIdAsync(int busId);
    Task<Result<List<BusEntity>>> GetByVendorIdAsync(
        int vendorId,
        int pageNumber,
        int pageSize,
        BusStatus? status = null,
        BusType? busType = null,
        string? search = null
    );
    Task<Result<int>> GetCountByVendorIdAsync(
        int vendorId,
        BusStatus? status = null,
        BusType? busType = null,
        string? search = null
    );
    Task<Result<BusEntity>> UpdateAsync(BusEntity bus);
    Task<Result<bool>> DeleteAsync(int busId);
    Task<Result<bool>> ExistsByRegistrationNoAsync(string registrationNo);
    Task<Result<bool>> ExistsByIdAndVendorAsync(int busId, int vendorId);
    Task<Result<bool>> ExistsByNameAndVendorAsync(string busName, int vendorId);
    Task<Result<bool>> ExistsAsync(int busId);
    Task<Result<List<BusEntity>>> GetAwaitingConfirmationByVendorAsync(int vendorId);
    
    // Admin methods
    Task<List<BusEntity>> GetAllBusesForAdminAsync();
    Task<List<BusEntity>> GetBusesByStatusAsync(BusStatus status);
    Task<List<BusEntity>> FilterBusesForAdminAsync(string? searchName, int? status, bool? isActive);
    Task<BusEntity?> GetBusDetailsForAdminAsync(int busId);
    
    // Seat layout template methods
    Task<Result<bool>> ApplyTemplateAsync(int busId, int templateId);
}
