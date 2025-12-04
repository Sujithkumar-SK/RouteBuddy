using Kanini.RouteBuddy.Domain.Enums;

namespace Kanini.RouteBuddy.Application.Dto.Schedule;

public class ScheduleResponseDto
{
    public int ScheduleId { get; set; }
    public int BusId { get; set; }
    public string BusName { get; set; } = null!;
    public int RouteId { get; set; }
    public string Source { get; set; } = null!;
    public string Destination { get; set; } = null!;
    public DateTime TravelDate { get; set; }
    public TimeSpan DepartureTime { get; set; }
    public TimeSpan ArrivalTime { get; set; }
    public int AvailableSeats { get; set; }
    public ScheduleStatus Status { get; set; }
    public bool IsActive { get; set; }
}