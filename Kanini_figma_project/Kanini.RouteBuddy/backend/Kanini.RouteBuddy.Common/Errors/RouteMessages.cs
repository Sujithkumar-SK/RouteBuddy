namespace Kanini.RouteBuddy.Common.Errors;

public static class RouteMessages
{
    public const string UnexpectedError = "An unexpected error occurred";
    public const string RouteDeletedSuccessfully = "Route deleted successfully";
    public static class LogMessages
    {
        public const string RouteCreationStarted = "Creating route from {Source} to {Destination}";
        public const string RouteRetrievalStarted = "Getting route by ID: {RouteId}";
        public const string RouteListRetrievalStarted = "Getting all routes, page: {PageNumber}, size: {PageSize}";
        public const string RouteUpdateStarted = "Updating route: {RouteId}";
        public const string RouteDeleteStarted = "Deleting route: {RouteId}";
        public const string RouteStopsRetrievalStarted = "Getting route stops for route: {RouteId}";
        
        public const string RouteCreatedSuccessfully = "Route created successfully: {RouteId}";
        public const string RouteRetrievedSuccessfully = "Route retrieved successfully: {RouteId}";
        public const string RouteListRetrievedSuccessfully = "Route list retrieved successfully: {Count} routes";
        public const string RouteUpdatedSuccessfully = "Route updated successfully: {RouteId}";
        public const string RouteDeletedSuccessfully = "Route deleted successfully: {RouteId}";
        public const string RouteStopsRetrievedSuccessfully = "Route stops retrieved successfully: {Count} stops";
        
        public const string RouteCreationFailed = "Route creation failed: {Error}";
        public const string RouteRetrievalFailed = "Route retrieval failed: {Error}";
        public const string RouteListFailed = "Route list failed: {Error}";
        public const string RouteUpdateFailed = "Route update failed: {Error}";
        public const string RouteDeleteFailed = "Route delete failed: {Error}";
        public const string RouteStopsFailed = "Route stops failed: {Error}";
        public const string RouteAlreadyExistsWarning = "Validation failed - route already exists";
    }
    
    public static class ErrorMessages
    {
        public const string RouteNotFound = "Route not found";
        public const string RouteAlreadyExists = "Route already exists between these cities";
        public const string InvalidRouteData = "Invalid route data provided";
        public const string RouteInUse = "Route cannot be deleted as it has active schedules";
        public const string ValidationError = "Validation error occurred";
        public const string DatabaseError = "Database error occurred";
        public const string UnexpectedError = "An unexpected error occurred";
    }
    
    public static class ErrorCodes
    {
        public const string RouteNotFound = "Route.NotFound";
        public const string RouteExists = "Route.Exists";
        public const string RouteCreationFailed = "Route.CreationFailed";
        public const string RouteRetrievalFailed = "Route.RetrievalFailed";
        public const string RouteListFailed = "Route.ListFailed";
        public const string RouteUpdateFailed = "Route.UpdateFailed";
        public const string RouteDeleteFailed = "Route.DeleteFailed";
        public const string RouteStopsFailed = "RouteStops.Failed";
        public const string RouteUnexpectedError = "Route.UnexpectedError";
        public const string RouteStopsUnexpectedError = "RouteStops.UnexpectedError";
    }
}