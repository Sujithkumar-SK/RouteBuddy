namespace Kanini.RouteBuddy.Application.Dto;

public class ConnectingBookingResponseDto
{
    public int BookingId { get; set; }
    public string PNR { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTime TravelDate { get; set; }
    public DateTime ReservationExpiryTime { get; set; }
    public string RouteDescription { get; set; } = string.Empty;
    public List<ConnectingSegmentBookingDto> Segments { get; set; } = new();
    public string Message { get; set; } = string.Empty;
}
