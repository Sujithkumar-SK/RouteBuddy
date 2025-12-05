using Kanini.RouteBuddy.Common.Utility;
using Kanini.RouteBuddy.Domain.Entities;

namespace Kanini.RouteBuddy.Data.Repositories.Vendor;

public interface IVendorSeatLayoutRepository
{
    // Vendor can only READ templates and APPLY them to their buses
    Task<Result<List<SeatLayoutTemplate>>> GetTemplatesByBusTypeAsync(int busType);
    Task<Result<SeatLayoutTemplate>> GetTemplateByIdAsync(int templateId);
    Task<Result<List<SeatLayoutDetail>>> GetBusSeatLayoutAsync(int busId);
    Task<Result<bool>> ApplyTemplateToLayoutAsync(int busId, int templateId);
}