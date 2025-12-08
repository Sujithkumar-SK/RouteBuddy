using Entities = Kanini.RouteBuddy.Domain.Entities;

namespace Kanini.RouteBuddy.Data.Repositories.Booking
{
    public interface IBookingRepository
    {
        Task<IEnumerable<Entities.Booking>> GetAllBookingsAsync();
        Task<IEnumerable<Entities.Booking>> FilterBookingsAsync(string? searchName, int? status, DateTime? fromDate, DateTime? toDate);
        Task<Entities.Booking?> GetBookingByIdAsync(int bookingId);
        Task<(int Pending, int Confirmed, int Cancelled, int Total, decimal Revenue)> GetBookingStatusSummaryAsync();

    }
}