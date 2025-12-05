namespace Kanini.RouteBuddy.Data.Models;

public class RouteStopCreationModel
{
    public int StopId { get; set; }
    public int OrderNumber { get; set; }
    public TimeSpan? ArrivalTime { get; set; }
    public TimeSpan? DepartureTime { get; set; }
}