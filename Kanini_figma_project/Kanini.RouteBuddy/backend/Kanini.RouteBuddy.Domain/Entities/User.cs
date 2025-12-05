using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kanini.RouteBuddy.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Kanini.RouteBuddy.Domain.Entities;

[Table("Users")]
[Index(nameof(Email), IsUnique = true, Name = "IX_Users_Email")]
[Index(nameof(Phone), IsUnique = true, Name = "IX_Customers_Phone")]
[Comment("Stores user authentication and login information")]
public class User : BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int UserId { get; set; }

    [Required]
    [MaxLength(150)]
    [EmailAddress]
    [Column(TypeName = "NVARCHAR(150)")]
    public string Email { get; set; } = null!;

    [Required]
    [MaxLength(256)]
    [Column(TypeName = "NVARCHAR(256)")]
    public string PasswordHash { get; set; } = null!;

    [Comment("Last successful login timestamp")]
    public DateTime? LastLogin { get; set; }

    [Comment("Number of failed login attempts")]
    public int FailedLoginAttempts { get; set; } = 0;

    [Comment("Account lockout until this time")]
    public DateTime? LockoutEnd { get; set; }

    [Required]
    [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Invalid mobile number")]
    [Column(TypeName = "NVARCHAR(10)")]
    public string Phone { get; set; } = null!;

    [Required]
    [Column(TypeName = "INT")]
    public UserRole Role { get; set; } = UserRole.Customer;

    [Column(TypeName = "BIT")]
    public bool IsEmailVerified { get; set; } = false;

    [Column(TypeName = "BIT")]
    public bool IsActive { get; set; } = true;

    [InverseProperty(nameof(Customer.User))]
    public Customer? Customer { get; set; }

    [InverseProperty(nameof(Vendor.User))]
    public Vendor? Vendor { get; set; }
}
