namespace Kanini.RouteBuddy.Common;

public static class MagicStrings
{
    public static class StoredProcedures
    {
        public const string SearchBuses = "sp_SearchBuses";
        public const string SearchBusesFiltered = "sp_SearchBusesFiltered";
        public const string FindConnectingRoutes = "sp_FindConnectingRoutes";
        public const string BookConnectingRoute = "sp_BookConnectingRoute";
        public const string ConfirmConnectingBooking = "sp_ConfirmConnectingBooking";
        public const string GetBusSeatLayout = "sp_GetBusSeatLayout";
        public const string ValidateSeatsAvailability = "sp_ValidateSeatsAvailability";
        public const string ValidateSeatsAndStops = "sp_ValidateSeatsAndStops";
        public const string GetRouteStops = "sp_GetRouteStops";
        public const string ConfirmBooking = "sp_ConfirmBooking";
        public const string ExpirePendingBookings = "sp_ExpirePendingBookings";
    }

    public static class LogMessages
    {
        public const string BusSearchStarted =
            "Bus search started for Source: {Source}, Destination: {Destination}, Date: {TravelDate}";
        public const string BusSearchCompleted = "Bus search completed. Found {Count} buses";
        public const string BusSearchFailed = "Bus search failed: {Error}";
        public const string ValidationFailed = "Validation failed for request";
        public const string SeatLayoutStarted =
            "Seat layout request started for ScheduleId: {ScheduleId}, Date: {TravelDate}";
        public const string SeatLayoutCompleted = "Seat layout completed. Found {Count} seats";
        public const string SeatLayoutFailed = "Seat layout failed: {Error}";
        public const string BookingStarted =
            "Booking started for ScheduleId: {ScheduleId}, Seats: {SeatCount}, Customer: {CustomerId}";
        public const string BookingCompleted =
            "Booking completed successfully. PNR: {PNR}, BookingId: {BookingId}";
        public const string BookingFailed = "Booking failed: {Error}";
        public const string SeatValidationStarted =
            "Seat availability validation started for {SeatCount} seats";
        public const string SeatValidationCompleted =
            "Seat validation completed. All seats available";
        public const string SeatValidationFailed = "Seat validation failed: {Error}";
        public const string RouteStopsStarted =
            "Route stops request started for ScheduleId: {ScheduleId}";
        public const string RouteStopsCompleted = "Route stops completed. Found {Count} stops";
        public const string RouteStopsFailed = "Route stops failed: {Error}";
        public const string BookingConfirmationStarted =
            "Booking confirmation started for BookingId: {BookingId}";
        public const string BookingConfirmationCompleted =
            "Booking confirmed successfully for BookingId: {BookingId}";
        public const string BookingConfirmationFailed = "Booking confirmation failed: {Error}";
        public const string BookingExpiryStarted = "Auto-expiry process started";
        public const string BookingExpiryCompleted =
            "Auto-expiry completed. Expired {Count} bookings";
        public const string BookingExpiryFailed = "Auto-expiry failed: {Error}";
        public const string FilteredBusSearchStarted =
            "Filtered bus search started for Source: {Source}, Destination: {Destination}, Date: {TravelDate}";
        public const string FilteredBusSearchCompleted =
            "Filtered bus search completed. Found {Count} buses";
        public const string FilteredBusSearchFailed = "Filtered bus search failed: {Error}";
        public const string ConnectingRoutesSearchStarted =
            "Connecting routes search started for Source: {Source}, Destination: {Destination}, Date: {TravelDate}";
        public const string ConnectingRoutesSearchCompleted =
            "Connecting routes search completed. Found {Count} routes";
        public const string ConnectingRoutesSearchFailed =
            "Connecting routes search failed: {Error}";
        public const string ConnectingBookingStarted =
            "Connecting booking started for Customer: {CustomerId}, Segments: {SegmentCount}, Date: {TravelDate}";
        public const string ConnectingBookingCompleted =
            "Connecting booking completed. PNR: {PNR}, BookingId: {BookingId}";
        public const string ConnectingBookingFailed = "Connecting booking failed: {Error}";
        public const string ConnectingBookingConfirmationStarted =
            "Connecting booking confirmation started for BookingId: {BookingId}";
        public const string ConnectingBookingConfirmationCompleted =
            "Connecting booking confirmed successfully for BookingId: {BookingId}";
        public const string ConnectingBookingConfirmationFailed =
            "Connecting booking confirmation failed: {Error}";
    }

    public static class ErrorMessages
    {
        public const string SourceRequired = "Source is required";
        public const string DestinationRequired = "Destination is required";
        public const string TravelDateRequired = "Travel date is required";
        public const string TravelDateInvalid = "Travel date cannot be in the past";
        public const string ScheduleIdRequired = "Schedule ID is required";
        public const string ScheduleNotFound = "Bus schedule not found";
        public const string DatabaseError = "Database operation failed";
        public const string UnexpectedError = "An unexpected error occurred";
        public const string SeatsRequired = "At least one seat must be selected";
        public const string PassengersRequired = "Passenger details are required";
        public const string SeatPassengerMismatch =
            "Number of seats must match number of passengers";
        public const string SeatsNotAvailable = "One or more selected seats are not available";
        public const string BookingFailed = "Booking could not be completed";
        public const string InvalidSeatNumbers = "Invalid seat numbers provided";
        public const string CustomerNotFound = "Customer not found";
        public const string BoardingStopRequired = "Boarding stop is required";
        public const string DroppingStopRequired = "Dropping stop is required";
        public const string InvalidBoardingStop = "Invalid boarding stop selected";
        public const string InvalidDroppingStop = "Invalid dropping stop selected";
        public const string SameStopError = "Boarding and dropping stops cannot be the same";
        public const string InvalidStopOrder = "Dropping stop must be after boarding stop in route";
        public const string BookingNotFound = "Booking not found";
        public const string BookingAlreadyConfirmed = "Booking is already confirmed";
        public const string BookingExpired = "Booking reservation has expired";
        public const string PaymentReferenceRequired = "Payment reference ID is required";
        public const string InvalidTimeRange = "Departure time 'from' cannot be greater than 'to'";
        public const string InvalidPriceRange =
            "Minimum price cannot be greater than maximum price";
        public const string InvalidSortOption = "Invalid sort option provided";
        public const string InvalidBusTypeFilter = "Invalid bus type filter provided";
        public const string InvalidAmenitiesFilter = "Invalid amenities filter provided";
        public const string NoConnectingRoutesFound =
            "No connecting routes available for this search";
        public const string InvalidToggleOption =
            "Invalid toggle option. Must be 'cheapest' or 'fastest'";
        public const string ConnectingRoutesNotFound = "No complete connecting routes found";
        public const string ConnectingBookingFailed =
            "Connecting route booking could not be completed";
        public const string SegmentValidationFailed = "One or more segments have validation errors";
        public const string PassengerMismatchAcrossSegments =
            "Passenger count must be same across all segments";
        public const string ConnectingBookingNotFound = "Connecting booking not found";
        public const string PartialBookingFailure = "Some segments could not be booked";
    }
}
