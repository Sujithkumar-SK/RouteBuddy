using Kanini.RouteBuddy.Common.Utility;
using Kanini.RouteBuddy.Domain.Entities;
using Kanini.RouteBuddy.Domain.Enums;
using RouteStopEntity = Kanini.RouteBuddy.Domain.Entities.RouteStop;
using BookingEntity = Kanini.RouteBuddy.Domain.Entities.Booking;

namespace Kanini.RouteBuddy.Data.Repositories.Buses;

public interface IBus_Search_Book_Repository
{
    Task<Result<List<BusSchedule>>> SearchBusesAsync(
        string source,
        string destination,
        DateTime travelDate
    );
    Task<Result<List<BusSchedule>>> SearchBusesFilteredAsync(
        string source,
        string destination,
        DateTime travelDate,
        List<int>? busTypes,
        List<int>? amenities,
        TimeSpan? departureTimeFrom,
        TimeSpan? departureTimeTo,
        decimal? minPrice,
        decimal? maxPrice,
        string? sortBy
    );
    Task<Result<List<SeatLayoutDetail>>> GetSeatLayoutAsync(int scheduleId, DateTime travelDate);
    Task<Result<List<SeatLayoutDetail>>> ValidateSeatsAvailabilityAsync(
        int scheduleId,
        DateTime travelDate,
        List<string> seatNumbers
    );
    Task<Result<BookingEntity>> BookSeatsAsync(
        int scheduleId,
        int customerId,
        DateTime travelDate,
        List<string> seatNumbers,
        List<(string Name, int Age, Gender Gender)> passengers,
        decimal totalAmount,
        int boardingStopId,
        int droppingStopId
    );
    Task<Result<(string BusName, string Route)>> GetBusInfoAsync(int scheduleId);
    Task<Result<List<RouteStopEntity>>> GetRouteStopsAsync(int scheduleId);
    Task<Result<List<SeatLayoutDetail>>> ValidateSeatsAndStopsAsync(
        int scheduleId,
        DateTime travelDate,
        List<string> seatNumbers,
        int boardingStopId,
        int droppingStopId
    );
    Task<Result<string>> ConfirmBookingAsync(
        int bookingId,
        string paymentReferenceId,
        bool isPaymentSuccessful
    );
    Task<Result<int>> ExpirePendingBookingsAsync();
    Task<Result<BookingEntity>> GetBookingDetailsAsync(int bookingId);
}
