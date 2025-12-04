using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kanini.RouteBuddy.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Kanini.RouteBuddy.Domain.Entities;

[Table("SeatLayoutDetails")]
[Comment("Individual seat definitions within seat layout templates")]
public class SeatLayoutDetail : BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int SeatLayoutDetailId { get; set; }

    [Required]
    [ForeignKey(nameof(SeatLayoutTemplate))]
    public int SeatLayoutTemplateId { get; set; }
    public SeatLayoutTemplate SeatLayoutTemplate { get; set; } = null!;

    [Required]
    [MaxLength(10)]
    [Column(TypeName = "NVARCHAR(10)")]
    [Comment("Seat identifier like 'A1', 'B2U', 'C3L'")]
    public string SeatNumber { get; set; } = null!;

    [Required]
    [Column(TypeName = "INT")]
    [Comment("Type of seat - Seater, SleeperUpper, SleeperLower, etc.")]
    public SeatType SeatType { get; set; }

    [Required]
    [Column(TypeName = "INT")]
    [Comment("Position of seat - Window, Aisle, Middle")]
    public SeatPosition SeatPosition { get; set; }

    [Required]
    [Range(1, 50)]
    [Column(TypeName = "INT")]
    [Comment("Physical row number in the bus")]
    public int RowNumber { get; set; }

    [Required]
    [Range(1, 10)]
    [Column(TypeName = "INT")]
    [Comment("Position within the row (1, 2, 3, 4)")]
    public int ColumnNumber { get; set; }

    [Required]
    [Column(TypeName = "INT")]
    [Comment("Pricing tier for this seat - Base, Premium, Luxury")]
    public PriceTier PriceTier { get; set; } = PriceTier.Base;

    [NotMapped]
    [Comment("Runtime property to indicate if seat is booked for specific travel date")]
    public bool IsBooked { get; set; }

    [NotMapped]
    [Comment("Runtime property for base price calculation")]
    public decimal BasePrice { get; set; }

    [NotMapped]
    [Comment("Runtime property for bus type calculation")]
    public BusType BusTypeForPricing { get; set; }

    [NotMapped]
    [Comment("Runtime property for amenities calculation")]
    public BusAmenities AmenitiesForPricing { get; set; }
}
