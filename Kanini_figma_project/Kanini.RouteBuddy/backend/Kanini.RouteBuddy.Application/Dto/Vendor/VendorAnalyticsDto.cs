namespace Kanini.RouteBuddy.Application.Dto.Vendor;

public class VendorAnalyticsDto
{
    public RevenueAnalyticsDto RevenueAnalytics { get; set; } = null!;
    public PerformanceMetricsDto PerformanceMetrics { get; set; } = null!;
    public FleetStatusDto FleetStatus { get; set; } = null!;
    public QuickStatsDto QuickStats { get; set; } = null!;
    public List<RecentBookingDto> RecentBookings { get; set; } = new();
    public List<VendorNotificationDto> Notifications { get; set; } = new();
    public List<VendorAlertDto> Alerts { get; set; } = new();
    public MaintenanceScheduleDto MaintenanceSchedule { get; set; } = null!;
}

public class RevenueAnalyticsDto
{
    public decimal TotalRevenue { get; set; }
    public decimal MonthlyRevenue { get; set; }
    public decimal WeeklyRevenue { get; set; }
}

public class PerformanceMetricsDto
{
    public int MonthlyBookings { get; set; }
    public double OnTimePerformance { get; set; }
}

public class FleetStatusDto
{
    public int TotalBuses { get; set; }
    public int ActiveBuses { get; set; }
    public int MaintenanceBuses { get; set; }
    public int IdleBuses { get; set; }
}

public class QuickStatsDto
{
    public int TotalBookings { get; set; }
    public int ActiveRoutes { get; set; }
    public decimal TotalRevenue { get; set; }
}

public class RecentBookingDto
{
    public int BookingId { get; set; }
    public string CustomerName { get; set; } = null!;
    public string Route { get; set; } = null!;
    public DateTime BookingDate { get; set; }
    public string Status { get; set; } = null!;
}

public class VendorNotificationDto
{
    public string Type { get; set; } = null!;
    public string Message { get; set; } = null!;
    public DateTime Time { get; set; }
}

public class VendorAlertDto
{
    public int AlertId { get; set; }
    public string Type { get; set; } = null!;
    public string Message { get; set; } = null!;
    public string Severity { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}

public class MaintenanceScheduleDto
{
    public List<UpcomingMaintenanceDto> UpcomingMaintenance { get; set; } = new();
    public List<object> MaintenanceHistory { get; set; } = new();
}

public class UpcomingMaintenanceDto
{
    public string BusNumber { get; set; } = null!;
    public string Type { get; set; } = null!;
    public DateTime DueDate { get; set; }
    public string Priority { get; set; } = null!;
}