namespace Kanini.RouteBuddy.Common.Errors;

public static class RouteStopMessages
{
    // Success Messages
    public const string RouteStopCreatedSuccessfully = "Route stop created successfully";
    public const string RouteStopUpdatedSuccessfully = "Route stop updated successfully";
    public const string RouteStopDeletedSuccessfully = "Route stop deleted successfully";

    // Error Messages
    public const string RouteStopNotFound = "Route stop not found";
    public const string DuplicateOrderNumber = "Order number already exists for this route";
    public const string InvalidOrderNumber = "Order number must be greater than 0";
    public const string RouteNotFound = "Route not found";
    public const string StopNotFound = "Stop not found";
    
    // Common Error Messages
    public const string ValidationError = "Validation error occurred";
    public const string DatabaseError = "Database error occurred";
    public const string UnexpectedError = "An unexpected error occurred";
}