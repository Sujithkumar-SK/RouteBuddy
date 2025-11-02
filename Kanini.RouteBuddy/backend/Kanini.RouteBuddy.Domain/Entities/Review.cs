using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Kanini.RouteBuddy.Domain.Entities;

[Table("Reviews")]
[Comment("Customer reviews and ratings for buses")]
public class Review : BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ReviewId { get; set; }

    [Required]
    [Range(1, 5)]
    [Column(TypeName = "INT")]
    public int Rating { get; set; }

    [MaxLength(500)]
    [Column(TypeName = "NVARCHAR(500)")]
    public string Comment { get; set; } = null!;

    [Required]
    [ForeignKey(nameof(Customer))]
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    [Required]
    [ForeignKey(nameof(Bus))]
    public int BusId { get; set; }
    public Bus Bus { get; set; } = null!;
}
