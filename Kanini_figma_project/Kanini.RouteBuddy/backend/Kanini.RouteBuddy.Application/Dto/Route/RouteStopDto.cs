using System.ComponentModel.DataAnnotations;

namespace Kanini.RouteBuddy.Application.Dto.Route;

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

public class CreateRouteStopDto
{
    [Required]
    public int StopId { get; set; }
    
    [Required, Range(1, int.MaxValue)]
    public int OrderNumber { get; set; }
    
    public TimeSpan? ArrivalTime { get; set; }
    public TimeSpan? DepartureTime { get; set; }
}

public class UpdateRouteStopDto
{
    [Required, Range(1, int.MaxValue)]
    public int OrderNumber { get; set; }
    
    public TimeSpan? ArrivalTime { get; set; }
    public TimeSpan? DepartureTime { get; set; }
}