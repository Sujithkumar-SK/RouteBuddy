namespace Kanini.RouteBuddy.Application.Dto;

public class RouteStopDto
{
    public int RouteStopId { get; set; }
    public int StopId { get; set; }
    public string StopName { get; set; } = string.Empty;
    public string? Landmark { get; set; }
    public int OrderNumber { get; set; }
    public TimeSpan? ArrivalTime { get; set; }
    public TimeSpan? DepartureTime { get; set; }
}
