using Kanini.RouteBuddy.Application.Dto;
using Kanini.RouteBuddy.Common.Utility;

namespace Kanini.RouteBuddy.Application.Services.Vendor;

public interface IVendorSeatLayoutService
{
    // Vendors can only READ templates and APPLY them to their buses
    Task<Result<List<SeatLayoutTemplateListDto>>> GetTemplatesByBusTypeAsync(int busType);
    Task<Result<SeatLayoutTemplateResponseDto>> GetTemplateByIdAsync(int templateId);
    Task<Result<bool>> ApplyTemplateToLayoutAsync(int busId, int templateId, int vendorId);
}