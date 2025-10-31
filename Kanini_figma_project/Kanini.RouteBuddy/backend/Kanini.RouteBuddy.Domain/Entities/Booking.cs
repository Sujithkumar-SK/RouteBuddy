using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kanini.RouteBuddy.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Kanini.RouteBuddy.Domain.Entities;

[Table("Bookings")]
[Comment("Customer booking details and management")]
[Index(nameof(PNRNo), IsUnique = true, Name = "IX_PNRNo")]
public class Booking : BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int BookingId { get; set; }

    [Required]
    [MaxLength(12)]
    [Column(TypeName = "NVARCHAR(12)")]
    public string PNRNo { get; set; } = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();

    [Required]
    [ForeignKey(nameof(Customer))]
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    [Required]
    [Range(1, 50)]
    [Column(TypeName = "INT")]
    public int TotalSeats { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    [Column(TypeName = "DECIMAL(18,2)")]
    public decimal TotalAmount { get; set; }

    [Required]
    [Column(TypeName = "DATE")]
    public DateTime TravelDate { get; set; }

    [Required]
    [Column(TypeName = "INT")]
    public BookingStatus Status { get; set; } = BookingStatus.Pending;

    [Column(TypeName = "DATETIME2")]
    public DateTime BookedAt { get; set; } = DateTime.UtcNow;

    [Column(TypeName = "DATETIME2")]
    public DateTime? CancelledAt { get; set; }

    [Column(TypeName = "BIT")]
    public bool IsActive { get; set; } = true;

    [InverseProperty(nameof(Payment.Booking))]
    public Payment? Payment { get; set; }

    [InverseProperty(nameof(BookedSeat.Booking))]
    public ICollection<BookedSeat> BookedSeats { get; set; } = new List<BookedSeat>();

    [InverseProperty(nameof(Cancellation.Booking))]
    public Cancellation? Cancellation { get; set; }

    [InverseProperty(nameof(BookingSegment.Booking))]
    public ICollection<BookingSegment> Segments { get; set; } = new List<BookingSegment>();
}
