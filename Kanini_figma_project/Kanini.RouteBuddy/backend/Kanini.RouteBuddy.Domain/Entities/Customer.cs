using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kanini.RouteBuddy.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Kanini.RouteBuddy.Domain.Entities;

[Table("Customers")]
public class Customer : BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int CustomerId { get; set; }

    [Required]
    [MaxLength(50)]
    [Comment("Customer's first name")]
    public string FirstName { get; set; } = null!;

    [MaxLength(50)]
    [Comment("Customer's middle name")]
    public string? MiddleName { get; set; }

    [Required]
    [MaxLength(50)]
    [Comment("Customer's last name")]
    public string LastName { get; set; } = null!;

    [Required]
    [Comment("Customer's date of birth")]
    public DateTime DateOfBirth { get; set; }

    // Computed Property
    [NotMapped]
    public string FullName => $"{FirstName} {MiddleName} {LastName}".Replace("  ", " ").Trim();

    [NotMapped]
    public int Age =>
        DateTime.Today.Year
        - DateOfBirth.Year
        - (DateTime.Today.DayOfYear < DateOfBirth.DayOfYear ? 1 : 0);

    [Required]
    [Column(TypeName = "INT")]
    public Gender Gender { get; set; } = Gender.Other;

    [Column(TypeName = "VARBINARY(MAX)")]
    public byte[]? ProfilePicture { get; set; }

    [Column(TypeName = "BIT")]
    public bool IsActive { get; set; } = false;

    [Required]
    [ForeignKey(nameof(User))]
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    [InverseProperty(nameof(Booking.Customer))]
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    [InverseProperty(nameof(Review.Customer))]
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
