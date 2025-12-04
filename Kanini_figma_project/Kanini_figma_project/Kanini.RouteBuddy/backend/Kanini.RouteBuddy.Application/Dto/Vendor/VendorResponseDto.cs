using Kanini.RouteBuddy.Domain.Enums;

namespace Kanini.RouteBuddy.Application.Dto.Vendor;

public class VendorResponseDto
{
    public int VendorId { get; set; }
    public string AgencyName { get; set; } = null!;
    public string OwnerName { get; set; } = null!;
    public string BusinessLicenseNumber { get; set; } = null!;
    public string OfficeAddress { get; set; } = null!;
    public int FleetSize { get; set; }
    public string? TaxRegistrationNumber { get; set; }
    public VendorStatus Status { get; set; }
    public bool IsActive { get; set; }
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}