using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Kanini.RouteBuddy.Domain.Entities;

[Table("Cancellations")]
[Comment("Booking cancellation details and penalty information")]
public class Cancellation : BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int CancellationId { get; set; }

    [Column(TypeName = "DATETIME2")]
    public DateTime CancelledOn { get; set; } = DateTime.UtcNow;

    [Required]
    [MaxLength(50)]
    [Column(TypeName = "NVARCHAR(50)")]
    public string CancelledBy { get; set; } = null!;

    [MaxLength(250)]
    [Column(TypeName = "NVARCHAR(250)")]
    public string Reason { get; set; } = null!;

    [Range(0, double.MaxValue)]
    [Column(TypeName = "DECIMAL(18,2)")]
    public decimal? PenaltyAmount { get; set; } = 0;

    [Column(TypeName = "BIT")]
    public bool IsActive { get; set; } = false;

    [Required]
    [ForeignKey(nameof(Booking))]
    public int BookingId { get; set; }
    public Booking Booking { get; set; } = null!;
}
