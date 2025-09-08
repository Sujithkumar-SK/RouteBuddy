using System.ComponentModel.DataAnnotations;

public class Driver : BaseEntity
{
  [Key]
  public int DriverId { get; set; }
  [Required, MaxLength(100)]
  public string DriverName { get; set; } = string.Empty;

  [Required, MaxLength(15)]
  public string LicenseNumber { get; set; } = string.Empty;

  public DateTime LicenseExpiry { get; set; }

  [MaxLength(15)]
  public string Phone { get; set; } = string.Empty;

  public bool IsActive { get; set; } = true;

  public ICollection<DriverAssignment> Assignments { get; set; } = new List<DriverAssignment>();
}
