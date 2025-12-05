namespace Kanini.RouteBuddy.Application.Dto.Booking
{
    public class BookingStatusSummaryDTO
    {
        public int PendingBookings { get; set; }
        public int ConfirmedBookings { get; set; }
        public int CancelledBookings { get; set; }
        public int TotalBookings { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}