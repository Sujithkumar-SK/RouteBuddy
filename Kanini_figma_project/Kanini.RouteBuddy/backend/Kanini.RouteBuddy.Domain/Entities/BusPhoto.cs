using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Kanini.RouteBuddy.Domain.Entities;

[Table("BusPhotos")]
[Comment("Bus images and photo gallery")]
public class BusPhoto : BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int BusPhotoId { get; set; }

    [Column(TypeName = "NVARCHAR(500)")]
    public string? ImagePath { get; set; }

    [MaxLength(100)]
    [Column(TypeName = "NVARCHAR(100)")]
    public string? Caption { get; set; }

    [Required]
    [ForeignKey(nameof(Bus))]
    public int BusId { get; set; }
    public Bus Bus { get; set; } = null!;
}
