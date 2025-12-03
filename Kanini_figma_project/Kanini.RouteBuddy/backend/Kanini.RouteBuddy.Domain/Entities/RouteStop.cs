using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Kanini.RouteBuddy.Domain.Entities;

[Table("RouteStops")]
[Comment("Junction table linking routes and stops with order and timing")]
public class RouteStop : BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int RouteStopId { get; set; }

    [Required]
    [ForeignKey(nameof(Route))]
    public int RouteId { get; set; }
    public Route Route { get; set; } = null!;

    [Required]
    [ForeignKey(nameof(Stop))]
    public int StopId { get; set; }
    public Stop Stop { get; set; } = null!;

    [Required]
    [Range(1, int.MaxValue)]
    [Comment("Defines the order of the stop in the route")]
    public int OrderNumber { get; set; }

    [Column(TypeName = "TIME")]
    public TimeSpan? ArrivalTime { get; set; }

    [Column(TypeName = "TIME")]
    public TimeSpan? DepartureTime { get; set; }

    [ForeignKey(nameof(Schedule))]
    public int? ScheduleId { get; set; }
    public BusSchedule? Schedule { get; set; }
}
