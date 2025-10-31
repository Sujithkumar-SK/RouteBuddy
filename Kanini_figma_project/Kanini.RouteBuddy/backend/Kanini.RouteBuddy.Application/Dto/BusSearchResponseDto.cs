using Kanini.RouteBuddy.Domain.Enums;

namespace Kanini.RouteBuddy.Application.Dto;

public class BusSearchResponseDto
{
    public int ScheduleId { get; set; }
    public int BusId { get; set; }
    public string BusName { get; set; } = string.Empty;
    public BusType BusType { get; set; }
    public int TotalSeats { get; set; }
    public int AvailableSeats { get; set; }
    public string Source { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public DateTime TravelDate { get; set; }
    public TimeSpan DepartureTime { get; set; }
    public TimeSpan ArrivalTime { get; set; }
    public decimal BasePrice { get; set; }
    public BusAmenities Amenities { get; set; }
    public string VendorName { get; set; } = string.Empty;
}
