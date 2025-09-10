using System.ComponentModel.DataAnnotations;
public class BusSchedule : BaseEntity
{
  [Key]
  public int ScheduleId { get; set; }
  public DateTime TravelDate { get; set; }
  public TimeSpan DepartureTime { get; set; }
  public TimeSpan ArrivalTime { get; set; }

  [Range(0, double.MaxValue)]
  public decimal Fare { get; set; }

  [Required, MaxLength(20)]
  [RegularExpression("^(Scheduled|Cancelled|Completed|Delayed)$")]
  public string Status { get; set; } = "Scheduled";


  public int BusId { get; set; }
  public Bus Bus { get; set; } = null!;

  public int RouteId { get; set; }
  public Route Route { get; set; } = null!;

  public ICollection<DriverAssignment> DriverAssignments { get; set; } = new List<DriverAssignment>();

  public ICollection<Booking> Bookings { get; set; } = new List<Booking>();

  public ICollection<BookingSegment> Segments { get; set; } = new List<BookingSegment>();
  
  public ICollection<Fare> Fares { get; set; } = new List<Fare>();
}