using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Kanini.RouteBuddy.Domain.Entities;

[Table("Routes")]
[Comment("Route details and information")]
public class Route : BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int RouteId { get; set; }

    [Required, MaxLength(100)]
    [Column(TypeName = "NVARCHAR(100)")]
    public string Source { get; set; } = null!;

    [Required, MaxLength(100)]
    [Column(TypeName = "NVARCHAR(100)")]
    public string Destination { get; set; } = null!;

    [Range(0, (double)decimal.MaxValue)]
    public decimal Distance { get; set; }

    [Column(TypeName = "TIME")]
    public TimeSpan Duration { get; set; }

    [Range(0, double.MaxValue)]
    [Column(TypeName = "decimal(18,3)")]
    public decimal BasePrice { get; set; }

    [Column(TypeName = "BIT")]
    public bool IsActive { get; set; } = true;

    [InverseProperty(nameof(BusSchedule.Route))]
    public ICollection<BusSchedule> Schedules { get; set; } = new List<BusSchedule>();

    [InverseProperty(nameof(RouteStop.Route))]
    public ICollection<RouteStop> RouteStops { get; set; } = new List<RouteStop>();
}
