using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
public class User : BaseEntity
{
  [Key]
  public int UserId { get; set; }

  [Required, MaxLength(100)]
  public string UserName { get; set; } = string.Empty;

  [Required, MaxLength(150), EmailAddress]
  public string Email { get; set; } = string.Empty;

  [Required, MaxLength(256)]
  public string PasswordHash { get; set; } = string.Empty;

  [Required, RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Invalid mobile number")]
  public string Phone { get; set; } = string.Empty;

  [Required, MaxLength(20)]
  [RegularExpression("^(Admin|Vendor|Customer)$", ErrorMessage = "Role must be Admin, Vendor or Customer")]
  public string Role { get; set; } = "Customer";

  [Required, MaxLength(10)]
  [RegularExpression("^(Male|Female|Other)$", ErrorMessage = "Gender must be Male, Female or Other")]
  public string Gender { get; set; } = "Other";
  [Required]
  public DateTime DateOfBirth { get; set; }
  [Column(TypeName = "VARBINARY(MAX)")]
  public byte[]? ProfilePicture { get; set; }
  [Required]
  public bool IsActive { get; set; } = true;
  public bool IsDeleted { get; set; } = false;

  public ICollection<Booking> Bookings
  { get; set; } = new List<Booking>();
  public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
