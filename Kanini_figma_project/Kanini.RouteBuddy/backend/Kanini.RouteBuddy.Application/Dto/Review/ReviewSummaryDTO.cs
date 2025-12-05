namespace Kanini.RouteBuddy.Application.Dto.Review
{
    public class ReviewSummaryDTO
    {
        public int ReviewId { get; set; }

        public int Rating { get; set; }

        public string? Comment { get; set; }

        public int BusId { get; set; }

        public string? BusName { get; set; }
    }
}
