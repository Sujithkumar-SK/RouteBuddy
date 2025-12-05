using Kanini.RouteBuddy.Common.Utility;

namespace Kanini.RouteBuddy.Data.Repositories.VendorAnalytics;

public interface IVendorAnalyticsRepository
{
    Task<Result<object>> GetRevenueAnalyticsAsync(int vendorId);
    Task<Result<object>> GetPerformanceMetricsAsync(int vendorId);
    Task<Result<object>> GetFleetStatusAsync(int vendorId);
    Task<Result<List<object>>> GetNotificationsAsync(int vendorId);
    Task<Result<object>> GetMaintenanceScheduleAsync(int vendorId);
    Task<Result<object>> GetRecentBookingsAsync(int vendorId);
    Task<Result<object>> GetQuickStatsAsync(int vendorId);
    Task<Result<object>> GetAlertsAsync(int vendorId);
}