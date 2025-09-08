using System.ComponentModel.DataAnnotations;

public abstract class BaseEntity
{
  [MaxLength(100)]
  public string CreadtedBy { get; set; } = string.Empty;
  public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

  [MaxLength(100)]
  public string? UpdateBy { get; set; }/// int accept Id
  public DateTime? UpdatedOn { get; set; }
}
