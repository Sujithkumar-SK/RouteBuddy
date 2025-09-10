using System.ComponentModel.DataAnnotations;
public class Bus : BaseEntity
{
  [Key]
  public int BusId { get; set; }

  [Required, MaxLength(100)]
  public string BusName { get; set; } = string.Empty;

  [Required, MaxLength(50)]
  public string BusType { get; set; } = string.Empty;

  [Required,Range(1,100)]
  public int TotalSeats { get; set; }

  [Required, MaxLength(50)]
  public string RegistrationNo { get; set; } = string.Empty;

  [Required, MaxLength(20)]
  [RegularExpression("^(Active|Inactive|Maintenance)$", ErrorMessage = "Invalid bus status")]
  public string Status { get; set; } = "Active";

  public int VendorId { get; set; }
  public Vendor Vendor { get; set; } = null!;

  public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
  public ICollection<BusPhoto> Photos { get; set; } = new List<BusPhoto>();

  public ICollection<BusSchedule> Schedules { get; set; } = new List<BusSchedule>();
  public ICollection<Review> Reviews { get; set; } = new List<Review>();
  public ICollection<BookedSeat> BookedSeats { get; set; } = new List<BookedSeat>();
}
