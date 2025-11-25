using Kanini.RouteBuddy.Domain.Entities;

namespace Kanini.RouteBuddy.Data.Repositories.Vendor;

public interface IVendorRepository
{
    Task<Domain.Entities.Vendor> CreateAsync(Domain.Entities.Vendor vendor);
    Task<Domain.Entities.Vendor> CreateVendorAsync(Domain.Entities.Vendor vendor);
    Task<Domain.Entities.Vendor?> GetByIdAsync(int vendorId);
    Task<Domain.Entities.Vendor?> GetByUserIdAsync(int userId);
    Task<Domain.Entities.Vendor?> GetByEmailAsync(string email);
    Task<IEnumerable<Domain.Entities.Vendor>> GetAllAsync(int pageNumber, int pageSize);
    Task<int> GetTotalCountAsync();
    Task<Domain.Entities.Vendor> UpdateAsync(Domain.Entities.Vendor vendor);
    Task<bool> DeleteAsync(int vendorId);
    Task<bool> ExistsByEmailAsync(string email);
    Task<bool> ExistsByLicenseNumberAsync(string licenseNumber);
    Task<bool> ExistsByIdAsync(int vendorId);
    Task<IEnumerable<Domain.Entities.Vendor>> GetPendingVendorsAsync(int pageNumber, int pageSize);
    Task<int> GetPendingVendorsCountAsync();
    Task<(int TotalBuses, int ActiveBuses, int PendingBuses, int TotalRoutes, int TotalSchedules, int UpcomingSchedules, string VendorStatus)> GetDashboardSummaryAsync(int vendorId);
    Task<IEnumerable<Domain.Entities.Vendor>> FilterVendorsAsync(string? searchName, bool? isActive, int? status);
}