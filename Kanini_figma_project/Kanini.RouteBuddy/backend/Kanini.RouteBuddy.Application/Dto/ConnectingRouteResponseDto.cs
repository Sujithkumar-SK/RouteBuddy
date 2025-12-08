namespace Kanini.RouteBuddy.Application.Dto;

public class ConnectingRouteResponseDto
{
    public List<ConnectingRouteSegmentDto> Segments { get; set; } = new();
    public decimal TotalPrice { get; set; }
    public int TotalDurationMinutes { get; set; }
    public string TotalDuration { get; set; } = string.Empty;
    public int NumberOfTransfers { get; set; }
    public string RouteDescription { get; set; } = string.Empty;
}
