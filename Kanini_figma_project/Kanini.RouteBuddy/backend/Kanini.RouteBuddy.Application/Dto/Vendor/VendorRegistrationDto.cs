using System.ComponentModel.DataAnnotations;

namespace Kanini.RouteBuddy.Application.Dto.Vendor;

public class VendorRegistrationDto
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    [MaxLength(150, ErrorMessage = "Email cannot exceed 150 characters")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Password is required")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", 
        ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character")]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "Phone number is required")]
    [RegularExpression(@"^(\+91[\-\s]?)?[6-9]\d{9}$", 
        ErrorMessage = "Please enter a valid Indian mobile number (10 digits starting with 6-9, optionally with +91 prefix)")]
    [MaxLength(15, ErrorMessage = "Phone number cannot exceed 15 characters")]
    public string Phone { get; set; } = null!;

    [Required(ErrorMessage = "Agency name is required")]
    [MinLength(2, ErrorMessage = "Agency name must be at least 2 characters long")]
    [MaxLength(150, ErrorMessage = "Agency name cannot exceed 150 characters")]
    [RegularExpression(@"^[a-zA-Z0-9\s\-\.&]+$", 
        ErrorMessage = "Agency name can only contain letters, numbers, spaces, hyphens, dots, and ampersands")]
    public string AgencyName { get; set; } = null!;

    [Required(ErrorMessage = "Owner name is required")]
    [MinLength(2, ErrorMessage = "Owner name must be at least 2 characters long")]
    [MaxLength(100, ErrorMessage = "Owner name cannot exceed 100 characters")]
    [RegularExpression(@"^[a-zA-Z\s\.]+$", 
        ErrorMessage = "Owner name can only contain letters, spaces, and dots")]
    public string OwnerName { get; set; } = null!;

    [Required(ErrorMessage = "Business license number is required")]
    [MinLength(5, ErrorMessage = "Business license number must be at least 5 characters long")]
    [MaxLength(50, ErrorMessage = "Business license number cannot exceed 50 characters")]
    [RegularExpression(@"^[A-Z0-9\-\/]+$", 
        ErrorMessage = "Business license number can only contain uppercase letters, numbers, hyphens, and forward slashes")]
    public string BusinessLicenseNumber { get; set; } = null!;

    [Required(ErrorMessage = "Office address is required")]
    [MinLength(10, ErrorMessage = "Office address must be at least 10 characters long")]
    [MaxLength(300, ErrorMessage = "Office address cannot exceed 300 characters")]
    public string OfficeAddress { get; set; } = null!;

    [Required(ErrorMessage = "Fleet size is required")]
    [Range(1, 1000, ErrorMessage = "Fleet size must be between 1 and 1000 vehicles")]
    public int FleetSize { get; set; }

    [MaxLength(50, ErrorMessage = "Tax registration number cannot exceed 50 characters")]
    [RegularExpression(@"^[A-Z0-9]+$", 
        ErrorMessage = "Tax registration number can only contain uppercase letters and numbers")]
    public string? TaxRegistrationNumber { get; set; }
}