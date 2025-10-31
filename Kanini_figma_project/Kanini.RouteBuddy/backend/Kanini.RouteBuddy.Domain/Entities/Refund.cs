using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kanini.RouteBuddy.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Kanini.RouteBuddy.Domain.Entities;

[Table("Refunds")]
[Comment("Payment refund processing and status tracking")]
public class Refund : BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int RefundId { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    [Column(TypeName = "DECIMAL(18,2)")]
    public decimal RefundAmount { get; set; }

    [Required]
    [Column(TypeName = "INT")]
    public PaymentMethod RefundMethod { get; set; } = PaymentMethod.Mock;

    [Required]
    [Column(TypeName = "INT")]
    public PaymentStatus RefundStatus { get; set; } = PaymentStatus.Pending;

    [Column(TypeName = "DATETIME2")]
    public DateTime? RefundedOn { get; set; }

    [Column(TypeName = "BIT")]
    public bool IsActive { get; set; } = false;

    [Required]
    [ForeignKey(nameof(Payment))]
    public int PaymentId { get; set; }
    public Payment Payment { get; set; } = null!;
}
