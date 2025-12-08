using Kanini.RouteBuddy.Domain.Enums;

namespace Kanini.RouteBuddy.Application.Dto.Admin
{
    public class AdminBookingDTO
    {
        public int BookingId { get; set; }
        public string PNRNo { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public int TotalSeats { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime TravelDate { get; set; }
        public BookingStatus Status { get; set; }
        public DateTime BookedAt { get; set; }
        public string BusName { get; set; } = string.Empty;
        public string Route { get; set; } = string.Empty;
        public PaymentStatus PaymentStatus { get; set; }
    }
}