using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kanini.RouteBuddy.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Kanini.RouteBuddy.Domain.Entities;

[Table("BookedSeats")]
[Comment("Individual seat bookings with passenger details")]
public class BookedSeat : BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int BookedSeatId { get; set; }

    [Required]
    [Column(TypeName = "DATE")]
    public DateTime TravelDate { get; set; }

    [Required]
    [MaxLength(10)]
    [Column(TypeName = "NVARCHAR(10)")]
    public string SeatNumber { get; set; } = null!;

    [Required]
    [Column(TypeName = "INT")]
    public SeatType SeatType { get; set; }

    [Required]
    [Column(TypeName = "INT")]
    public SeatPosition SeatPosition { get; set; }

    [Required]
    [MaxLength(100)]
    [Column(TypeName = "NVARCHAR(100)")]
    public string PassengerName { get; set; } = null!;

    [Required]
    [Range(1, 120)]
    [Column(TypeName = "INT")]
    public int PassengerAge { get; set; }

    [Required]
    [Column(TypeName = "INT")]
    public Gender PassengerGender { get; set; }

    [Required]
    [ForeignKey(nameof(Booking))]
    public int BookingId { get; set; }
    public Booking Booking { get; set; } = null!;

    [Required]
    [ForeignKey(nameof(BookingSegment))]
    public int BookingSegmentId { get; set; }
    public BookingSegment BookingSegment { get; set; } = null!;
}
