using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kanini.RouteBuddy.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Kanini.RouteBuddy.Domain.Entities;

[Table("Payments")]
[Index(nameof(TransactionId), Name = "IX_Payments_TransactionId")]
[Comment("Payment processing and transaction details")]
public class Payment : BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int PaymentId { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    [Column(TypeName = "DECIMAL(18,2)")]
    public decimal Amount { get; set; }

    [Required]
    [Column(TypeName = "INT")]
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Mock;

    [Required]
    [Column(TypeName = "INT")]
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

    [Column(TypeName = "DATETIME2")]
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

    [Required]
    [MaxLength(100)]
    [Column(TypeName = "NVARCHAR(100)")]
    public string TransactionId { get; set; } = null!;

    [Column(TypeName = "BIT")]
    public bool IsActive { get; set; } = false;

    [Required]
    [ForeignKey(nameof(Booking))]
    public int BookingId { get; set; }
    public Booking Booking { get; set; } = null!;

    [InverseProperty(nameof(Refund.Payment))]
    public ICollection<Refund> Refunds { get; set; } = new List<Refund>();
}
