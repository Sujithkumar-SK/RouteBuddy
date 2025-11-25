using Kanini.RouteBuddy.Application.Dto.Booking;
using Kanini.RouteBuddy.Application.Dto.Review;
using Kanini.RouteBuddy.Domain.Enums;

namespace Kanini.RouteBuddy.Application.Dto.Admin
{
    public class AdminCustomerDTO
    {
        public int CustomerId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public Gender Gender { get; set; }

        public int Age { get; set; }

        public bool IsActive { get; set; }

        public int TotalBookings { get; set; }

        public int TotalReviews { get; set; }

        public DateTime? LastBookingDate { get; set; }

        // Optional: Quick overview lists (if needed)
        public ICollection<BookingSummaryDTO>? RecentBookings { get; set; }

        public ICollection<ReviewSummaryDTO>? RecentReviews { get; set; }
    }
}
