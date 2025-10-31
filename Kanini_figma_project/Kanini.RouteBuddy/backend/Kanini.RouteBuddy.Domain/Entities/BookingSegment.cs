using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Kanini.RouteBuddy.Domain.Entities;

[Table("BookingSegments")]
[Comment("Segments of a booking associated with specific bus schedules")]
public class BookingSegment : BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int BookingSegmentId { get; set; }

    [Required]
    [ForeignKey(nameof(Booking))]
    public int BookingId { get; set; }
    public Booking Booking { get; set; } = null!;

    [Required]
    [ForeignKey(nameof(Schedule))]
    public int ScheduleId { get; set; }
    public BusSchedule Schedule { get; set; } = null!;

    [Required]
    [Range(1, 50)]
    [Column(TypeName = "INT")]
    public int SeatsBooked { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    [Column(TypeName = "DECIMAL(18,2)")]
    public decimal SegmentAmount { get; set; }

    [Required]
    [Range(1, 10)]
    [Column(TypeName = "INT")]
    public int SegmentOrder { get; set; }

    [Comment("Boarding stop for this segment")]
    [ForeignKey(nameof(BoardingStop))]
    public int? BoardingStopId { get; set; }
    public RouteStop? BoardingStop { get; set; }

    [Comment("Dropping stop for this segment")]
    [ForeignKey(nameof(DroppingStop))]
    public int? DroppingStopId { get; set; }
    public RouteStop? DroppingStop { get; set; }

    [InverseProperty(nameof(BookedSeat.BookingSegment))]
    public ICollection<BookedSeat> BookedSeats { get; set; } = new List<BookedSeat>();
}
