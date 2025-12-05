using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kanini.RouteBuddy.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Kanini.RouteBuddy.Domain.Entities;

[Table("Buses")]
[Index(nameof(RegistrationNo), IsUnique = true, Name = "IX_RegistrationNo")]
[Comment("Bus details and information")]
public class Bus : BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int BusId { get; set; }

    [Required]
    [MaxLength(100)]
    [Column(TypeName = "NVARCHAR(100)")]
    public string BusName { get; set; } = null!;

    [Required]
    [Column(TypeName = "INT")]
    public BusType BusType { get; set; }

    [Required]
    [Range(1, 100)]
    [Column(TypeName = "INT")]
    public int TotalSeats { get; set; }

    [Required]
    [MaxLength(50)]
    [Column(TypeName = "NVARCHAR(50)")]
    public string RegistrationNo { get; set; } = null!;

    [MaxLength(500)]
    [Column(TypeName = "NVARCHAR(500)")]
    public string RegistrationPath { get; set; } = null!;

    [Required]
    [Column(TypeName = "INT")]
    public BusStatus Status { get; set; } = BusStatus.PendingApproval;

    [Required]
    [Column(TypeName = "INT")]
    public BusAmenities Amenities { get; set; } = BusAmenities.None;

    [MaxLength(100)]
    [Column(TypeName = "NVARCHAR(100)")]
    public string DriverName { get; set; } = null!;

    [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Invalid mobile number")]
    [Column(TypeName = "NVARCHAR(10)")]
    public string DriverContact { get; set; } = null!;

    [Column(TypeName = "BIT")]
    public bool IsActive { get; set; } = false;

    [Comment("Optional seat layout template for this bus")]
    [ForeignKey(nameof(SeatLayoutTemplate))]
    public int? SeatLayoutTemplateId { get; set; }
    public SeatLayoutTemplate? SeatLayoutTemplate { get; set; }

    [Required]
    [ForeignKey(nameof(Vendor))]
    public int VendorId { get; set; }
    public Vendor Vendor { get; set; } = null!;

    [InverseProperty(nameof(BusPhoto.Bus))]
    public ICollection<BusPhoto> Photos { get; set; } = new List<BusPhoto>();

    [InverseProperty(nameof(BusSchedule.Bus))]
    public ICollection<BusSchedule> Schedules { get; set; } = new List<BusSchedule>();

    [InverseProperty(nameof(Review.Bus))]
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}