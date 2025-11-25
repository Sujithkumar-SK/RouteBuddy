namespace Kanini.RouteBuddy.Common.Errors;

public static class StopMessages
{
    public const string UnexpectedError = "An unexpected error occurred";
    public const string StopDeletedSuccessfully = "Stop deleted successfully";

    public static class LogMessages
    {
        public const string CreatingStop = "Creating stop: {StopName}";
        public const string StopCreatedSuccessfully = "Stop created successfully: {StopId}";
        public const string GettingStopById = "Getting stop by ID: {StopId}";
        public const string StopRetrievedSuccessfully = "Stop retrieved successfully: {StopId}";
        public const string GettingAllStops = "Getting all stops, page: {PageNumber}, size: {PageSize}";
        public const string StopsRetrievedSuccessfully = "Retrieved {Count} stops";
        public const string UpdatingStop = "Updating stop: {StopId}";
        public const string StopUpdatedSuccessfully = "Stop updated successfully: {StopId}";
        public const string DeletingStop = "Deleting stop: {StopId}";
        public const string StopDeletedSuccessfully = "Stop deleted successfully: {StopId}";
        public const string StopNotFound = "Stop not found: {StopId}";
    }
    
    public static class ErrorMessages
    {
        public const string StopNotFound = "Stop not found";
        public const string StopAlreadyExists = "Stop already exists";
        public const string InvalidStopData = "Invalid stop data provided";
        public const string StopInUse = "Stop cannot be deleted as it is used in routes";
        public const string DatabaseError = "Database error occurred";
        public const string UnexpectedError = "An unexpected error occurred";
    }
    
    public static class ErrorCodes
    {
        public const string StopNotFound = "Stop.NotFound";
        public const string StopExists = "Stop.Exists";
        public const string CreationFailed = "Stop.CreationFailed";
        public const string RetrievalFailed = "Stop.RetrievalFailed";
        public const string ListFailed = "Stop.ListFailed";
        public const string UpdateFailed = "Stop.UpdateFailed";
        public const string DeletionFailed = "Stop.DeletionFailed";
        public const string DatabaseError = "Stop.DatabaseError";
        public const string UnexpectedError = "Stop.UnexpectedError";
    }
}