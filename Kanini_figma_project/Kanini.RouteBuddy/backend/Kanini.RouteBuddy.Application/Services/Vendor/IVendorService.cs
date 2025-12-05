using Kanini.RouteBuddy.Application.Dto.Vendor;
using Kanini.RouteBuddy.Application.Dto.Common;
using Kanini.RouteBuddy.Application.Dto.Admin;
using Kanini.RouteBuddy.Common.Utility;

namespace Kanini.RouteBuddy.Application.Services.Vendor;

public interface IVendorService
{
    Task<Result<VendorResponseDto>> RegisterVendorAsync(VendorRegistrationDto dto);
    Task<Result<VendorResponseDto>> GetVendorByIdAsync(int vendorId);
    Task<PagedResultDto<VendorResponseDto>> GetAllVendorsAsync(int pageNumber, int pageSize);
    Task<Result<VendorResponseDto>> UpdateVendorAsync(int vendorId, VendorRegistrationDto dto);
    Task<Result<VendorResponseDto>> UpdateVendorProfileAsync(int vendorId, UpdateVendorProfileDto dto);
    Task<Result<bool>> DeleteVendorAsync(int vendorId);
    Task<Result<VendorResponseDto>> ApproveVendorAsync(int vendorId);
    Task<Result<bool>> RejectVendorAsync(int vendorId);
    Task<PagedResultDto<VendorResponseDto>> GetPendingVendorsAsync(int pageNumber, int pageSize);
    Task<PagedResultDto<AdminVendorDTO>> GetPendingVendorsForAdminAsync(int pageNumber, int pageSize);
    Task<VendorDashboardSummaryDto> GetVendorDashboardSummaryAsync(int vendorId);
    
    // Admin methods
    Task<Result<List<AdminVendorDTO>>> FilterVendorsAsync(string? searchName, bool? isActive, int? status);
    Task<Result<bool>> ApproveVendorWithEmailAsync(int vendorId);
    Task<Result<bool>> RejectVendorWithReasonAsync(int vendorId, string rejectionReason);
    Task<Result<bool>> DeactivateVendorWithReasonAsync(int vendorId, string reason);
    Task<Result<bool>> ReactivateVendorWithReasonAsync(int vendorId, string reason);
    Task<PagedResultDto<AdminVendorDTO>> GetAllVendorsForAdminAsync(int pageNumber, int pageSize);
    Task<Result<VendorApprovalDTO>> GetVendorForApprovalAsync(int vendorId);
    Task<Result<VendorResponseDto>> GetVendorByUserIdAsync(int userId);
}