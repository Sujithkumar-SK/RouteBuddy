namespace Kanini.RouteBuddy.Application.Dto.Vendor;

public class VendorDashboardSummaryDto
{
    public int TotalBuses { get; set; }
    public int ActiveBuses { get; set; }
    public int PendingBuses { get; set; }
    public int TotalRoutes { get; set; }
    public int TotalSchedules { get; set; }
    public int UpcomingSchedules { get; set; }
    public string VendorStatus { get; set; } = null!;
    public DateTime LastUpdated { get; set; }
}