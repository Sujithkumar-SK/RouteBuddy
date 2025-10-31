namespace Kanini.RouteBuddy.Application.Dto;

public class SeatLayoutResponseDto
{
    public int ScheduleId { get; set; }
    public DateTime TravelDate { get; set; }
    public BusInfoDto Bus { get; set; } = new();
    public List<SeatDto> Seats { get; set; } = new();
}
