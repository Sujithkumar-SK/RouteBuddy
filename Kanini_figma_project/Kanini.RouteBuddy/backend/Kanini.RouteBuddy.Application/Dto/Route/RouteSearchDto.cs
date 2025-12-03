namespace Kanini.RouteBuddy.Application.Dto.Route;

public class RouteSearchDto
{
    public int RouteId { get; set; }
    public string Source { get; set; } = null!;
    public string Destination { get; set; } = null!;
    public decimal Distance { get; set; }
    public TimeSpan Duration { get; set; }
    public decimal BasePrice { get; set; }
    public int TotalStops { get; set; }
    public bool IsActive { get; set; }
    public string RouteDescription => $"{Source} → {Destination}";
    public string DurationText => $"{Duration.Hours}h {Duration.Minutes}m";
    public string DistanceText => $"{Distance:F1} km";
}