using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kanini.RouteBuddy.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Kanini.RouteBuddy.Domain.Entities;

[Table("Vendors")]
[Index(nameof(BusinessLicenseNumber), IsUnique = true, Name = "IX_BusinessLicenseNumber")]
[Index(nameof(TaxRegistrationNumber), IsUnique = true, Name = "IX_TaxRegistrationNumber")]
[Comment("Vendor information")]
public class Vendor : BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int VendorId { get; set; }

    [Required]
    [MaxLength(150)]
    [Column(TypeName = "NVARCHAR(150)")]
    public string AgencyName { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    [Column(TypeName = "NVARCHAR(100)")]
    public string OwnerName { get; set; } = null!;

    [Required]
    [MaxLength(50)]
    [Column(TypeName = "NVARCHAR(50)")]
    public string BusinessLicenseNumber { get; set; } = null!;

    [Required]
    [MaxLength(300)]
    [Column(TypeName = "NVARCHAR(300)")]
    public string OfficeAddress { get; set; } = null!;

    [Range(1, 1000)]
    [Column(TypeName = "INT")]
    public int FleetSize { get; set; }

    [MaxLength(50)]
    [Column(TypeName = "NVARCHAR(50)")]
    public string? TaxRegistrationNumber { get; set; }

    [Column(TypeName = "INT")]
    public VendorStatus Status { get; set; } = VendorStatus.PendingApproval;

    [Column(TypeName = "BIT")]
    public bool IsActive { get; set; } = false;

    [Required]
    [ForeignKey(nameof(User))]
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    [InverseProperty(nameof(Bus.Vendor))]
    public ICollection<Bus> Buses { get; set; } = new List<Bus>();

    [InverseProperty(nameof(VendorDocument.Vendor))]
    public ICollection<VendorDocument> Documents { get; set; } = new List<VendorDocument>();
}
