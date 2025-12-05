using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Kanini.RouteBuddy.Application.Dto.Auth;

public class CompleteVendorProfileDto
{
    [Required, MaxLength(150)]
    public string AgencyName { get; set; } = null!;

    [Required, MaxLength(100)]
    public string OwnerName { get; set; } = null!;

    [Required, MaxLength(50)]
    public string BusinessLicenseNumber { get; set; } = null!;

    [Required, MaxLength(300)]
    public string OfficeAddress { get; set; } = null!;

    [Required, Range(1, 1000)]
    public int FleetSize { get; set; }

    [MaxLength(50)]
    public string? TaxRegistrationNumber { get; set; }

    [Required]
    public IFormFile BusinessLicenseDocument { get; set; } = null!;

    public IFormFile? TaxRegistrationDocument { get; set; }

    public IFormFile? OwnerIdentityDocument { get; set; }
}
