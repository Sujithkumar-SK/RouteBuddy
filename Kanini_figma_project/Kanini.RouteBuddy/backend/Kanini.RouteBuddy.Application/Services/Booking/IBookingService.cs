using Kanini.RouteBuddy.Application.Dto.Admin;
using Kanini.RouteBuddy.Application.Dto.Booking;

namespace Kanini.RouteBuddy.Application.Services.Booking
{
    public interface IBookingService
    {
        Task<IEnumerable<AdminBookingDTO>> GetAllBookingsAsync();
        Task<IEnumerable<AdminBookingDTO>> FilterBookingsAsync(string? searchName, int? status, DateTime? fromDate, DateTime? toDate);
        Task<AdminBookingDTO?> GetBookingByIdAsync(int bookingId);
        Task<BookingStatusSummaryDTO> GetBookingStatusSummaryAsync();

    }
}