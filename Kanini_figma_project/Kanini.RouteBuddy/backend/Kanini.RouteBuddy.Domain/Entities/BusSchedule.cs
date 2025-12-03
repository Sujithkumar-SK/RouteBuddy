using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kanini.RouteBuddy.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Kanini.RouteBuddy.Domain.Entities;

[Table("BusSchedules")]
[Comment("Bus schedule and timing information")]
public class BusSchedule : BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ScheduleId { get; set; }

    [Required]
    [Column(TypeName = "DATE")]
    public DateTime TravelDate { get; set; }

    [Required]
    [Column(TypeName = "TIME")]
    public TimeSpan DepartureTime { get; set; }

    [Required]
    [Column(TypeName = "TIME")]
    public TimeSpan ArrivalTime { get; set; }

    [Required]
    [Column(TypeName = "INT")]
    public ScheduleStatus Status { get; set; } = ScheduleStatus.Scheduled;

    [Column(TypeName = "BIT")]
    public bool IsActive { get; set; } = true;

    [Required]
    [Column(TypeName = "INT")]
    public int AvailableSeats { get; set; }

    [Required]
    [ForeignKey(nameof(Bus))]
    public int BusId { get; set; }
    public Bus Bus { get; set; } = null!;

    [Required]
    [ForeignKey(nameof(Route))]
    public int RouteId { get; set; }
    public Route Route { get; set; } = null!;

    [InverseProperty(nameof(BookingSegment.Schedule))]
    public ICollection<BookingSegment> Segments { get; set; } = new List<BookingSegment>();

    [InverseProperty(nameof(RouteStop.Schedule))]
    public ICollection<RouteStop> RouteStops { get; set; } = new List<RouteStop>();
}
