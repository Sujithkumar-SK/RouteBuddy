using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
public class BusPhoto : BaseEntity
{
  [Key]
  public int BusPhotoId { get; set; }

  [Column(TypeName = "VARBINARY(MAX)")]
  public byte[]? ImageData { get; set; } = null!;

  [MaxLength(100)]
  public string? Caption { get; set; }

  public int BusId { get; set; }
  public Bus Bus { get; set; } = null!;
}
