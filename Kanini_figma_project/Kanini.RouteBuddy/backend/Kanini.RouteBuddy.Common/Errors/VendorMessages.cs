namespace Kanini.RouteBuddy.Common.Errors;

public static class VendorMessages
{
    // Success Messages
    public const string VendorRegisteredSuccessfully = "Vendor registered successfully";
    public const string VendorApprovedSuccessfully = "Vendor approved successfully";
    public const string VendorRejectedSuccessfully = "Vendor rejected successfully";
    public const string ProfileUpdatedSuccessfully = "Profile updated successfully";
    public const string DashboardRetrievedSuccessfully = "Dashboard summary retrieved successfully";

    // Error Messages
    public const string VendorNotFound = "Vendor not found";
    public const string EmailAlreadyExists = "Email already exists";
    public const string PhoneAlreadyExists = "Phone number already exists";
    public const string BusinessLicenseExists = "Business license number already exists";
    public const string UnauthorizedAccess = "Unauthorized access to vendor data";
    public const string InvalidVendorStatus = "Invalid vendor status for this operation";
    public const string OnlyActiveVendorsCanUpdate = "Only active vendors can update profile";

    // Notification Messages
    public const string NewBookingReceived = "New booking received for Route {0}";
    public const string MaintenanceDue = "Bus maintenance due in {0} days";
    public const string PerformanceAchievement = "Congratulations! {0}% on-time performance this month";

    // Exception Messages
    public const string DatabaseError = "Database operation failed";
    public const string ValidationError = "Validation failed";
    public const string UnexpectedError = "An unexpected error occurred";
    public const string ServiceUnavailable = "Service temporarily unavailable";
    public const string InvalidOperation = "Invalid operation attempted";

    // Log Messages
    public static class LogMessages
    {
        public const string GettingDashboard = "Getting dashboard summary for vendor ID: {0}";
        public const string UpdatingProfile = "Updating profile for vendor ID: {0}";
        public const string GettingProfile = "Getting current vendor profile";
        public const string GettingPerformanceMetrics = "Getting performance metrics for vendor ID: {0}";
        public const string GettingNotifications = "Getting notifications for vendor ID: {0}";
        public const string GettingRevenueAnalytics = "Getting revenue analytics for vendor ID: {0}";
        public const string GettingFleetStatus = "Getting fleet status for vendor ID: {0}";
        public const string GettingMaintenanceSchedule = "Getting maintenance schedule for vendor ID: {0}";
        public const string GettingRecentBookings = "Getting recent bookings for vendor ID: {0}";
        public const string GettingQuickStats = "Getting quick stats for vendor ID: {0}";
        public const string GettingAlerts = "Getting alerts for vendor ID: {0}";
        public const string GettingRoutePerformance = "Getting route performance for vendor ID: {0}";
        public const string InvalidVendorId = "Invalid vendor ID provided: {0}";
        public const string DashboardRetrieved = "Dashboard summary retrieved successfully for vendor ID: {0}";
        public const string ProfileUpdated = "Profile updated successfully for vendor ID: {0}";
        public const string ProfileRetrieved = "Profile retrieved successfully for vendor ID: {0}";
        public const string PerformanceMetricsRetrieved = "Performance metrics retrieved successfully for vendor ID: {0}";
        public const string NotificationsRetrieved = "Notifications retrieved successfully for vendor ID: {0}";
        public const string RevenueAnalyticsRetrieved = "Revenue analytics retrieved successfully for vendor ID: {0}";
        public const string FleetStatusRetrieved = "Fleet status retrieved successfully for vendor ID: {0}";
        public const string MaintenanceScheduleRetrieved = "Maintenance schedule retrieved successfully for vendor ID: {0}";
        public const string RecentBookingsRetrieved = "Recent bookings retrieved successfully for vendor ID: {0}";
        public const string QuickStatsRetrieved = "Quick stats retrieved successfully for vendor ID: {0}";
        public const string AlertsRetrieved = "Alerts retrieved successfully for vendor ID: {0}";
        public const string RoutePerformanceRetrieved = "Route performance retrieved successfully for vendor ID: {0}";
        public const string SqlErrorDashboard = "SQL error getting dashboard for vendor ID: {0}";
        public const string SqlErrorProfile = "SQL error updating profile for vendor ID: {0}";
        public const string SqlErrorGetProfile = "SQL error getting vendor profile";
        public const string SqlErrorPerformanceMetrics = "SQL error getting performance metrics for vendor ID: {0}";
        public const string SqlErrorNotifications = "SQL error getting notifications for vendor ID: {0}";
        public const string SqlErrorRevenueAnalytics = "SQL error getting revenue analytics for vendor ID: {0}";
        public const string SqlErrorFleetStatus = "SQL error getting fleet status for vendor ID: {0}";
        public const string SqlErrorMaintenanceSchedule = "SQL error getting maintenance schedule for vendor ID: {0}";
        public const string SqlErrorRecentBookings = "SQL error getting recent bookings for vendor ID: {0}";
        public const string SqlErrorQuickStats = "SQL error getting quick stats for vendor ID: {0}";
        public const string SqlErrorAlerts = "SQL error getting alerts for vendor ID: {0}";
        public const string SqlErrorRoutePerformance = "SQL error getting route performance for vendor ID: {0}";
        public const string UnexpectedErrorDashboard = "Unexpected error getting dashboard for vendor ID: {0}";
        public const string UnexpectedErrorProfile = "Unexpected error updating profile for vendor ID: {0}";
        public const string UnexpectedErrorGetProfile = "Unexpected error getting vendor profile";
        public const string UnexpectedErrorPerformanceMetrics = "Unexpected error getting performance metrics for vendor ID: {0}";
        public const string UnexpectedErrorNotifications = "Unexpected error getting notifications for vendor ID: {0}";
        public const string UnexpectedErrorRevenueAnalytics = "Unexpected error getting revenue analytics for vendor ID: {0}";
        public const string UnexpectedErrorFleetStatus = "Unexpected error getting fleet status for vendor ID: {0}";
        public const string UnexpectedErrorMaintenanceSchedule = "Unexpected error getting maintenance schedule for vendor ID: {0}";
        public const string UnexpectedErrorRecentBookings = "Unexpected error getting recent bookings for vendor ID: {0}";
        public const string UnexpectedErrorQuickStats = "Unexpected error getting quick stats for vendor ID: {0}";
        public const string UnexpectedErrorAlerts = "Unexpected error getting alerts for vendor ID: {0}";
        public const string UnexpectedErrorRoutePerformance = "Unexpected error getting route performance for vendor ID: {0}";
    }
}