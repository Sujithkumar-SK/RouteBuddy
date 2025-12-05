namespace Kanini.RouteBuddy.Common.Errors;

public static class ScheduleMessages
{
    public static class LogMessages
    {

        public const string CreatingSchedule = "Creating schedule for Bus ID: {BusId}, Route ID: {RouteId}, Travel Date: {TravelDate}";
        public const string ScheduleCreatedSuccessfully = "Schedule created successfully with ID: {ScheduleId}";
        public const string ScheduleCreationFailed = "Failed to create schedule for Bus ID: {BusId}, Route ID: {RouteId}";
        public const string ScheduleCreationException = "Exception occurred while creating schedule for Bus ID: {BusId}, Route ID: {RouteId}";
        public const string GettingScheduleById = "Getting schedule by ID: {ScheduleId}";
        public const string ScheduleRetrievedSuccessfully = "Schedule retrieved successfully with ID: {ScheduleId}";
        public const string ScheduleRetrievalException = "Exception occurred while retrieving schedule with ID: {ScheduleId}";
        public const string GettingSchedulesByVendor = "Getting schedules for vendor ID: {VendorId}, Page: {PageNumber}, Size: {PageSize}";
        public const string SchedulesByVendorRetrievedSuccessfully = "Retrieved {Count} schedules for vendor ID: {VendorId}";
        public const string SchedulesByVendorRetrievalFailed = "Failed to retrieve schedules for vendor ID: {VendorId}";
        public const string SchedulesByVendorException = "Exception occurred while retrieving schedules for vendor ID: {VendorId}";
        public const string ScheduleCountRetrievalFailed = "Failed to retrieve schedule count for vendor ID: {VendorId}";
        public const string UpdatingSchedule = "Updating schedule with ID: {ScheduleId}";
        public const string ScheduleUpdatedSuccessfully = "Schedule updated successfully with ID: {ScheduleId}";
        public const string ScheduleUpdateFailed = "Failed to update schedule with ID: {ScheduleId}";
        public const string ScheduleUpdateException = "Exception occurred while updating schedule with ID: {ScheduleId}";
        public const string DeletingSchedule = "Deleting schedule with ID: {ScheduleId}";
        public const string ScheduleDeletedSuccessfully = "Schedule deleted successfully with ID: {ScheduleId}";
        public const string ScheduleDeletionException = "Exception occurred while deleting schedule with ID: {ScheduleId}";
        public const string CreatingBulkSchedule = "Creating bulk schedule for Bus ID: {BusId}, Route ID: {RouteId}, Start Date: {StartDate}, End Date: {EndDate}";
        public const string BulkScheduleCreatedSuccessfully = "Bulk schedule created successfully with {Count} schedules";
        public const string BulkScheduleCreationException = "Exception occurred while creating bulk schedule for Bus ID: {BusId}, Route ID: {RouteId}";
        public const string BusNotFoundForVendor = "Bus with ID: {BusId} not found for vendor ID: {VendorId}";
        public const string BusNotActive = "Bus with ID: {BusId} is not active";
        public const string RouteNotFound = "Route with ID: {RouteId} not found";
        public const string ScheduleNotFound = "Schedule with ID: {ScheduleId} not found";
        public const string PastDateSchedule = "Cannot create schedule for past date: {TravelDate}";
        public const string InvalidScheduleTime = "Invalid schedule time - Departure: {DepartureTime}, Arrival: {ArrivalTime}";
        public const string ScheduleAlreadyExists = "Schedule already exists for Bus ID: {BusId}, Route ID: {RouteId}, Date: {TravelDate}";
        public const string ScheduleExistenceCheckFailed = "Failed to check schedule existence for Bus ID: {BusId}, Route ID: {RouteId}, Date: {TravelDate}";
        public const string CheckingScheduleExistence = "Checking schedule existence for Bus ID: {BusId}, Route ID: {RouteId}, Date: {TravelDate}";
        public const string ScheduleExistenceChecked = "Schedule existence checked - Exists: {Exists} for Bus ID: {BusId}, Route ID: {RouteId}, Date: {TravelDate}";
    }
    
    public static class ErrorMessages
    {
        public const string ScheduleNotFound = "Schedule not found";
        public const string ScheduleCreationFailed = "Failed to create schedule";
        public const string ScheduleUpdateFailed = "Failed to update schedule";
        public const string ScheduleDeletionFailed = "Failed to delete schedule";
        public const string InvalidScheduleData = "Invalid schedule data provided";
        public const string BusNotAvailable = "Bus is not available for scheduling";
        public const string BusNotActive = "Only active buses can be scheduled";
        public const string RouteNotFound = "Route not found";
        public const string ScheduleAlreadyExists = "Schedule already exists for the specified bus, route and date";
        public const string PastDateSchedule = "Cannot create schedule for past dates";
        public const string InvalidScheduleTime = "Arrival time must be after departure time";
        public const string DatabaseError = "Database operation failed";
        public const string UnexpectedError = "An unexpected error occurred";
    }
    
    public static class ErrorCodes
    {
        public const string ScheduleNotFound = "SCHEDULE_NOT_FOUND";
        public const string CreationFailed = "SCHEDULE_CREATION_FAILED";
        public const string UpdateFailed = "SCHEDULE_UPDATE_FAILED";
        public const string DeletionFailed = "SCHEDULE_DELETION_FAILED";
        public const string InvalidData = "INVALID_SCHEDULE_DATA";
        public const string BusNotFound = "BUS_NOT_FOUND";
        public const string BusNotActive = "BUS_NOT_ACTIVE";
        public const string RouteNotFound = "ROUTE_NOT_FOUND";
        public const string ScheduleExists = "SCHEDULE_ALREADY_EXISTS";
        public const string PastDate = "PAST_DATE_SCHEDULE";
        public const string InvalidTime = "INVALID_SCHEDULE_TIME";
        public const string DatabaseError = "DATABASE_ERROR";
        public const string UnexpectedError = "UNEXPECTED_ERROR";
    }
}