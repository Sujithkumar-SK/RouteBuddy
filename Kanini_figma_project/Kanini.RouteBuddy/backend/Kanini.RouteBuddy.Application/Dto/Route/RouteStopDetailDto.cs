namespace Kanini.RouteBuddy.Application.Dto.Route;

public class RouteStopDetailDto
{
    public int RouteStopId { get; set; }
    public int StopId { get; set; }
    public string StopName { get; set; } = null!;
    public string? Landmark { get; set; }
    public int OrderNumber { get; set; }
    public TimeSpan? ArrivalTime { get; set; }
    public TimeSpan? DepartureTime { get; set; }
    public bool IsTerminal => OrderNumber == 1 || DepartureTime == null;
    public string StopType => OrderNumber == 1 ? "Origin" : (DepartureTime == null ? "Destination" : "Intermediate");
    public string? ArrivalTimeText => ArrivalTime?.ToString(@"hh\:mm");
    public string? DepartureTimeText => DepartureTime?.ToString(@"hh\:mm");
    public string DisplayName => string.IsNullOrEmpty(Landmark) ? StopName : $"{StopName} ({Landmark})";
}