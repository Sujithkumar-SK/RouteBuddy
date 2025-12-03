using System.ComponentModel.DataAnnotations;

namespace Kanini.RouteBuddy.Application.Dto.Route;

public class CreateRouteStopRequest
{
    [Required]
    public int StopId { get; set; }
    
    [Required]
    [Range(1, int.MaxValue)]
    public int OrderNumber { get; set; }
    
    public TimeSpan? ArrivalTime { get; set; }
    
    public TimeSpan? DepartureTime { get; set; }
}