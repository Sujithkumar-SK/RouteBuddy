using Kanini.RouteBuddy.Application.Dto;
using Kanini.RouteBuddy.Common.Utility;

namespace Kanini.RouteBuddy.Application.Services.Buses;

public interface IBusService
{
    Task<Result<List<BusSearchResponseDto>>> SearchBusesAsync(BusSearchRequestDto request);
    Task<Result<List<BusSearchResponseDto>>> SearchBusesFilteredAsync(BusSearchFilterDto request);
    Task<Result<SeatLayoutResponseDto>> GetSeatLayoutAsync(SeatLayoutRequestDto request);
    Task<Result<BookingResponseDto>> BookSeatsAsync(BookingRequestDto request);
    Task<Result<List<RouteStopDto>>> GetRouteStopsAsync(int scheduleId);
    Task<Result<string>> ConfirmBookingAsync(BookingConfirmationDto request);
    Task<Result<int>> ExpirePendingBookingsAsync();
}
