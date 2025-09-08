using System.ComponentModel.DataAnnotations;

public class BookedSeat : BaseEntity
{
  [Key]
  public int BookedSeatId { get; set; }
  public DateTime TravelDate { get; set; }

  [Required, MaxLength(10)]
  public string SeatNumber { get; set; } = string.Empty;

  public int BookingId { get; set; }
  public Booking Booking { get; set; } = null!;

  public int BusId { get; set; }
  public Bus Bus { get; set; } = null!;
}