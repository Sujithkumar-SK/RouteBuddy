using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Kanini.RouteBuddy.Domain.Entities;

[Table("Stops")]
[Comment("Route stops and landmarks information")]
public class Stop : BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int StopId { get; set; }

    [Required]
    [MaxLength(150)]
    [Column(TypeName = "NVARCHAR(150)")]
    public string Name { get; set; } = null!;

    [MaxLength(250)]
    [Column(TypeName = "NVARCHAR(250)")]
    public string? Landmark { get; set; }

    [Column(TypeName = "BIT")]
    public bool IsActive { get; set; } = true;

    [InverseProperty(nameof(RouteStop.Stop))]
    public ICollection<RouteStop> RouteStops { get; set; } = new List<RouteStop>();
}
