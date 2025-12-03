using Kanini.RouteBuddy.Domain.Entities;
using Kanini.RouteBuddy.Domain.Enums;

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
    Task<VendorApprovalData?> GetVendorForApprovalAsync(int vendorId);
    Task<Domain.Entities.Vendor> UpdateVendorOnlyAsync(Domain.Entities.Vendor vendor);
    Task UpdateVendorDocumentsStatusAsync(int vendorId, DocumentStatus status, string verifiedBy);
}

public class VendorApprovalData
{
    public Domain.Entities.Vendor Vendor { get; set; } = null!;
    public List<Domain.Entities.VendorDocument> Documents { get; set; } = new();
}