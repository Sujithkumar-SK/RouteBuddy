using System.ComponentModel.DataAnnotations;
public class Refund : BaseEntity
{
  [Key]
  public int RefundId { get; set; }

  [Range(0, double.MaxValue)]
  public decimal RefundAmount { get; set; }

  [Required, MaxLength(50)]
  [RegularExpression("^(Mock|UPI|Card|Wallet)$", ErrorMessage = "Invalid refund method")]
  public string RefundMethod { get; set; } = "Mock";

  [Required, MaxLength(20)]
  [RegularExpression("^(Pending|Processed|Failed)$", ErrorMessage = "Invalid refund status")]
  public string RefundStatus { get; set; } = "Pending";

  public DateTime? RefundedOn { get; set; }

  public int PaymentId { get; set; }
  public Payment Payment { get; set; } = null!;
}
