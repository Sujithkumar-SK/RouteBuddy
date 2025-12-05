using Kanini.RouteBuddy.Domain.Enums;

namespace Kanini.RouteBuddy.Application.Dto
{
    public class CustomerBookingDto
    {
        public int BookingId { get; set; }
        public string PNRNo { get; set; } = string.Empty;
        public int TotalSeats { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime TravelDate { get; set; }
        public BookingStatus Status { get; set; }
        public DateTime BookedAt { get; set; }
        public string BusName { get; set; } = string.Empty;
        public string Route { get; set; } = string.Empty;
        public string VendorName { get; set; } = string.Empty;
        public TimeSpan DepartureTime { get; set; }
        public TimeSpan ArrivalTime { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public bool IsPaymentCompleted { get; set; }
    }
}