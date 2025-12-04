using System.ComponentModel.DataAnnotations;
using Kanini.RouteBuddy.Common.Validators;

namespace Kanini.RouteBuddy.Application.Dto.Vendor;

public class UpdateVendorProfileDto
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    [MaxLength(150, ErrorMessage = "Email cannot exceed 150 characters")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Phone number is required")]
    [RegularExpression(@"^(\+91[\-\s]?)?[6-9]\d{9}$", ErrorMessage = "Please enter a valid Indian mobile number")]
    [MaxLength(15, ErrorMessage = "Phone number cannot exceed 15 characters")]
    public string Phone { get; set; } = null!;

    [Required(ErrorMessage = "Agency name is required")]
    [MinLength(2, ErrorMessage = "Agency name must be at least 2 characters long")]
    [MaxLength(150, ErrorMessage = "Agency name cannot exceed 150 characters")]
    [RegularExpression(@"^[a-zA-Z0-9\s\-\.&]+$", ErrorMessage = "Agency name contains invalid characters")]
    public string AgencyName { get; set; } = null!;

    [Required(ErrorMessage = "Owner name is required")]
    [MinLength(2, ErrorMessage = "Owner name must be at least 2 characters long")]
    [MaxLength(100, ErrorMessage = "Owner name cannot exceed 100 characters")]
    [RegularExpression(@"^[a-zA-Z\s\.]+$", ErrorMessage = "Owner name can only contain letters, spaces, and dots")]
    public string OwnerName { get; set; } = null!;

    [Required(ErrorMessage = "Office address is required")]
    [MinLength(10, ErrorMessage = "Office address must be at least 10 characters long")]
    [MaxLength(500, ErrorMessage = "Office address cannot exceed 500 characters")]
    public string OfficeAddress { get; set; } = null!;

    [Required(ErrorMessage = "Fleet size is required")]
    [Range(1, 1000, ErrorMessage = "Fleet size must be between 1 and 1000 vehicles")]
    public int FleetSize { get; set; }

    [EmailAddress(ErrorMessage = "Please enter a valid support email address")]
    [MaxLength(150, ErrorMessage = "Support email cannot exceed 150 characters")]
    public string? SupportContactEmail { get; set; }

    [RegularExpression(@"^[0-9]{2}[A-Z]{5}[0-9]{4}[A-Z]{1}[1-9A-Z]{1}Z[0-9A-Z]{1}$", ErrorMessage = "Invalid GST number format (e.g., 22AAAAA0000A1Z5)")]
    public string? GstTaxId { get; set; }
}