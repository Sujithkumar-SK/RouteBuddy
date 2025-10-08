using System.ComponentModel.DataAnnotations;
namespace Kanini.RouteBuddy.Domain.Entities;

public class BookingSegment
{
  [Key]
  public int BookingSegmentId { get; set; }

  public int BookingId { get; set; }
  public Booking Booking { get; set; } = null!;

  public int ScheduleId { get; set; }
  public BusSchedule Schedule { get; set; } = null!;

  public ICollection<BookedSeat> BookedSeats { get; set; } = new List<BookedSeat>();
}
