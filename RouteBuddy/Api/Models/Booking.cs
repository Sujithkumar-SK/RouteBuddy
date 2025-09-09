using System.ComponentModel.DataAnnotations;
public class Booking : BaseEntity
{
    [Key]
    public int BookingId { get; set; }

    [Required, MaxLength(12)]
    public string PNRNo { get; set; } = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();

    [Required]
    public DateTime TravelDate { get; set; }

    [Required, MaxLength(20)]
    [RegularExpression("^(Pending|Confirmed|Cancelled)$", ErrorMessage = "Invalid booking status")]
    public string Status { get; set; } = "Pending";

    public DateTime BookedAt { get; set; } = DateTime.UtcNow;

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int BusId { get; set; }
    public Bus Bus { get; set; } = null!;

    public int ScheduleId { get; set; }
    public BusSchedule Schedule { get; set; } = null!;


    public Payment Payment { get; set; } = null!;
    public ICollection<BookedSeat> BookedSeats { get; set; } = new List<BookedSeat>();
    public Cancellation? Cancellation { get; set; }

    public ICollection<BookingSegment> Segments { get; set; } = new List<BookingSegment>();
}
