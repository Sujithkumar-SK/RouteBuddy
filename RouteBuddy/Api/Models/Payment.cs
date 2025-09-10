using System.ComponentModel.DataAnnotations;
public class Payment : BaseEntity
{
  [Key]
  public int PaymentId { get; set; }

  [Range(0, double.MaxValue)]
  public decimal Amount { get; set; }

  [Required, MaxLength(50)]
  [RegularExpression("^(Mock|UPI|Card|NetBanking)$", ErrorMessage = "Invalid payment method")]
  public string PaymentMethod { get; set; } = "Mock";

  [Required, MaxLength(20)]
  [RegularExpression("^(Pending|Success|Failed)$", ErrorMessage = "Invalid payment status")]
  public string PaymentStatus { get; set; } = "Pending";

  public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

  public int BookingId { get; set; }
  public Booking Booking { get; set; } = null!;

  public ICollection<Refund> Refunds { get; set; } = new List<Refund>();
}
