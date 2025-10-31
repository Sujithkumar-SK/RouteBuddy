namespace Kanini.RouteBuddy.Application.Dto;

public class ConnectingRouteSegmentDto
{
    public int ScheduleId { get; set; }
    public string BusName { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public DateTime TravelDate { get; set; }
    public TimeSpan DepartureTime { get; set; }
    public TimeSpan ArrivalTime { get; set; }
    public decimal Price { get; set; }
    public string VendorName { get; set; } = string.Empty;
    public int AvailableSeats { get; set; }
}
