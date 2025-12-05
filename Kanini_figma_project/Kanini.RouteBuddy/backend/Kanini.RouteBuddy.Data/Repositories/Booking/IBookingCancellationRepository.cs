using Kanini.RouteBuddy.Domain.Entities;

namespace Kanini.RouteBuddy.Data.Repositories.Booking;

public interface IBookingCancellationRepository
{
    Task<BookingCancellationData?> GetBookingForCancellationAsync(int bookingId, int customerId);
    Task<bool> CancelBookingAsync(int bookingId, string reason, decimal penaltyAmount, string cancelledBy);
}

public class BookingCancellationData
{
    public int BookingId { get; set; }
    public int CustomerId { get; set; }
    public string PNRNo { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTime TravelDate { get; set; }
    public DateTime BookedAt { get; set; }
    public int BookingStatus { get; set; }
    public int PaymentMethod { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public int HoursUntilTravel { get; set; }
}