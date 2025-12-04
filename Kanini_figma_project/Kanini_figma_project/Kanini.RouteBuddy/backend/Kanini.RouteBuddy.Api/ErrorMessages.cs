namespace Kanini.RouteBuddy.Api.Constants
{
    public static class ErrorMessages
    {
        public const string MinAgeNegative = "MinAge cannot be negative";
        public const string MaxAgeNegative = "MaxAge cannot be negative";
        public const string MinAgeGreaterThanMaxAge = "MinAge cannot be greater than MaxAge";
        public const string AgeRangeInvalid = "Age range must be between 0 and 120";
        public const string SearchNameTooLong = "Search name cannot exceed 100 characters";
        public const string CustomerIdInvalid = "Customer ID must be greater than 0";
        public const string CustomerNotFound = "Customer with ID {0} not found";
        public const string VendorIdInvalid = "Vendor ID must be greater than 0";
        public const string VendorNotFound = "Vendor with ID {0} not found";
        public const string VendorStatusInvalid = "Vendor status must be 0 (Pending), 1 (Active), or 2 (Rejected)";
        public const string InternalServerError = "Internal server error: {0}";
        public const string DatabaseConnectionError = "Database connection failed";
        public const string UnauthorizedAccess = "Unauthorized access to this resource";
        public const string BookingIdInvalid = "Booking ID must be greater than 0";
        public const string BookingNotFound = "Booking with ID {0} not found";
        public const string BookingNotCancellable = "Booking with ID {0} cannot be refunded - only cancelled bookings are eligible";
        public const string RefundAlreadyProcessed = "Refund has already been processed for booking ID {0}";
    }
}