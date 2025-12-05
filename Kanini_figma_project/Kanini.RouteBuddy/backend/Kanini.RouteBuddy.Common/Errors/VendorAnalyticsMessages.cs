namespace Kanini.RouteBuddy.Common.Errors;

public static class VendorAnalyticsMessages
{
    public static class LogMessages
    {
        public const string RevenueAnalyticsRetrieved = "Revenue analytics retrieved successfully for vendor ID: {0}";
        public const string PerformanceMetricsRetrieved = "Performance metrics retrieved successfully for vendor ID: {0}";
        public const string FleetStatusRetrieved = "Fleet status retrieved successfully for vendor ID: {0}";
        public const string NotificationsRetrieved = "Notifications retrieved successfully for vendor ID: {0}";
        public const string MaintenanceScheduleRetrieved = "Maintenance schedule retrieved successfully for vendor ID: {0}";
        public const string RecentBookingsRetrieved = "Recent bookings retrieved successfully for vendor ID: {0}";
        public const string QuickStatsRetrieved = "Quick stats retrieved successfully for vendor ID: {0}";
        public const string AlertsRetrieved = "Alerts retrieved successfully for vendor ID: {0}";
        public const string RoutePerformanceRetrieved = "Route performance retrieved successfully for vendor ID: {0}";
        public const string AnalyticsOperationFailed = "Analytics operation failed for vendor ID: {0} - {1}";
    }

    public static class ErrorMessages
    {
        public const string DatabaseError = "Database error occurred during analytics operation";
        public const string UnexpectedError = "An unexpected error occurred during analytics operation";
        public const string VendorNotFound = "Vendor not found";
    }

    public static class ErrorCodes
    {
        public const string DatabaseError = "VendorAnalytics.Database.Error";
        public const string UnexpectedError = "VendorAnalytics.Unexpected.Error";
        public const string VendorNotFound = "VendorAnalytics.Vendor.NotFound";
    }
}