namespace Kanini.RouteBuddy.Application.Dto.Route;

public class RouteResponseDto
{
    public int RouteId { get; set; }
    public string Source { get; set; } = null!;
    public string Destination { get; set; } = null!;
    public decimal Distance { get; set; }
    public TimeSpan Duration { get; set; }
    public decimal BasePrice { get; set; }
    public bool IsActive { get; set; }
    public List<RouteStopDto> RouteStops { get; set; } = new();
}