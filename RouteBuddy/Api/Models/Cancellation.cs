using System.ComponentModel.DataAnnotations;
public class Cancellation : BaseEntity
{
  [Key]
  public int CancellationId { get; set; }
  public DateTime CancelledOn { get; set; } = DateTime.UtcNow;

  [Required, MaxLength(50)]
  public string CancelledBy { get; set; } = string.Empty;

  [MaxLength(250)]
  public string Reason { get; set; } = string.Empty;

  [Range(0, double.MaxValue)]
  public decimal PenaltyAmount { get; set; } = 0;

  public int BookingId { get; set; }
  public Booking Booking { get; set; } = null!;
}
