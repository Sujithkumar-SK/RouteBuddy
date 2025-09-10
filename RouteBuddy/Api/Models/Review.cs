using System.ComponentModel.DataAnnotations;
public class Review : BaseEntity
{
  [Key]
  public int ReviewId { get; set; }

  [Range(1, 5)]
  public int Rating { get; set; }

  [MaxLength(500)]
  public string Comment { get; set; } = string.Empty;

  public int UserId { get; set; }
  public User User { get; set; } = null!;

  public int BusId { get; set; }
  public Bus Bus { get; set; } = null!;
}