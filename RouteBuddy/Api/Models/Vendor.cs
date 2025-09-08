using System.ComponentModel.DataAnnotations;
public class Vendor : BaseEntity
{
  [Key]
  public int VendorId { get; set; }

  [Required, MaxLength(150)]
  public string VendorName { get; set; } = string.Empty;

  [Required, MaxLength(150), EmailAddress]
  public string Email { get; set; } = string.Empty;

  [Required, MaxLength(20)]
  [RegularExpression("^(Active|Inactive)$", ErrorMessage = "Status must be Active or Inactive")]
  public string Status { get; set; } = "Active";

  public ICollection<Bus> Buses { get; set; } = new List<Bus>();
}
