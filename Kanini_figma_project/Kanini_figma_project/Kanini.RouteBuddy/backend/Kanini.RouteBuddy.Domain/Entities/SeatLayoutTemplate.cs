using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kanini.RouteBuddy.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Kanini.RouteBuddy.Domain.Entities;

[Table("SeatLayoutTemplates")]
[Comment("Master templates for different bus seat layouts")]
public class SeatLayoutTemplate : BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int SeatLayoutTemplateId { get; set; }

    [Required]
    [MaxLength(100)]
    [Column(TypeName = "NVARCHAR(100)")]
    [Comment("Template name like 'AC Seater 2x2', 'Sleeper 2x1'")]
    public string TemplateName { get; set; } = null!;

    [Required]
    [Range(1, 100)]
    [Column(TypeName = "INT")]
    public int TotalSeats { get; set; }

    [Required]
    [Column(TypeName = "INT")]
    public BusType BusType { get; set; }

    [MaxLength(500)]
    [Column(TypeName = "NVARCHAR(500)")]
    public string? Description { get; set; }

    [Column(TypeName = "BIT")]
    public bool IsActive { get; set; } = true;

    [InverseProperty(nameof(SeatLayoutDetail.SeatLayoutTemplate))]
    public ICollection<SeatLayoutDetail> SeatLayoutDetails { get; set; } =
        new List<SeatLayoutDetail>();

    [InverseProperty(nameof(Bus.SeatLayoutTemplate))]
    public ICollection<Bus> Buses { get; set; } = new List<Bus>();
}
