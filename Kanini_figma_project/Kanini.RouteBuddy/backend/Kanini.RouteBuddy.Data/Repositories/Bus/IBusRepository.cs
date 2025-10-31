using Kanini.RouteBuddy.Common.Utility;
using Kanini.RouteBuddy.Domain.Entities;
using Kanini.RouteBuddy.Domain.Enums;

namespace Kanini.RouteBuddy.Data.Repositories.Buses;

public interface IBusRepository
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
    Task<Result<Booking>> BookSeatsAsync(
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
    Task<Result<List<RouteStop>>> GetRouteStopsAsync(int scheduleId);
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
}
