using System.ComponentModel.DataAnnotations;

namespace Kanini.RouteBuddy.Application.Dto.Route;

public class UpdateRouteDto
{
    [Required, MaxLength(100)]
    public string Source { get; set; } = null!;

    [Required, MaxLength(100)]
    public string Destination { get; set; } = null!;

    [Required, Range(1, 5000)]
    public double Distance { get; set; }

    [Required]
    public TimeSpan Duration { get; set; }

    [Required, Range(1, 10000)]
    public decimal BasePrice { get; set; }
}