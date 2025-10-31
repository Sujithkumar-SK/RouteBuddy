using Kanini.RouteBuddy.Domain.Enums;

namespace Kanini.RouteBuddy.Application.Dto;

public class BookingResponseDto
{
    public int BookingId { get; set; }
    public string PNR { get; set; } = string.Empty;
    public int ScheduleId { get; set; }
    public DateTime TravelDate { get; set; }
    public List<string> SeatNumbers { get; set; } = new();
    public decimal TotalAmount { get; set; }
    public BookingStatus Status { get; set; }
    public DateTime BookedAt { get; set; }
    public string BusName { get; set; } = string.Empty;
    public string Route { get; set; } = string.Empty;
    public string BoardingPoint { get; set; } = string.Empty;
    public string DroppingPoint { get; set; } = string.Empty;
    public TimeSpan? BoardingTime { get; set; }
    public TimeSpan? DroppingTime { get; set; }
    public DateTime ReservationExpiryTime { get; set; }
}
