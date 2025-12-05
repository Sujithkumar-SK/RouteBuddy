using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kanini.RouteBuddy.Domain.Entities;
using Kanini.RouteBuddy.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Kanini.RouteBuddy.Domain.Entities;

[Table("VendorDocuments")]
[Comment("Vendor document verification and management")]
public class VendorDocument : BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int DocumentId { get; set; }

    [Required]
    [ForeignKey(nameof(Vendor))]
    public int VendorId { get; set; }
    public Vendor Vendor { get; set; } = null!;

    [Required]
    [Column(TypeName = "INT")]
    public DocumentCategory DocumentFile { get; set; }

    [Required]
    [MaxLength(500)]
    [Column(TypeName = "NVARCHAR(500)")]
    public string DocumentPath { get; set; } = string.Empty;

    [Comment("Document issue date")]
    public DateTime? IssueDate { get; set; }

    [Comment("Document expiry date")]
    public DateTime? ExpiryDate { get; set; }

    [Column(TypeName = "DATETIME2")]
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    [Column(TypeName = "BIT")]
    public bool IsVerified { get; set; } = false;

    [Column(TypeName = "DATETIME2")]
    public DateTime? VerifiedAt { get; set; }

    [MaxLength(100)]
    [Column(TypeName = "NVARCHAR(100)")]
    public string? VerifiedBy { get; set; }

    [MaxLength(500)]
    [Column(TypeName = "NVARCHAR(500)")]
    public string? RejectedReason { get; set; }

    [Required]
    [Column(TypeName = "INT")]
    public DocumentStatus Status { get; set; } = DocumentStatus.Pending;
}
