using Kanini.RouteBuddy.Domain.Enums;

namespace Kanini.RouteBuddy.Application.Dto.Booking
{
    public class BookingSummaryDTO
    {
        public int BookingId { get; set; }

        public string PNRNo { get; set; } = string.Empty;

        public DateTime TravelDate { get; set; }

        public BookingStatus Status { get; set; }

        public decimal TotalAmount { get; set; }
    }
}
