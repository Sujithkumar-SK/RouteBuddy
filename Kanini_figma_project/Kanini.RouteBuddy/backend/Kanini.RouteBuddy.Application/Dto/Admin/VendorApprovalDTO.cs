using Kanini.RouteBuddy.Application.Dto.Vendor;

namespace Kanini.RouteBuddy.Application.Dto.Admin;

public class VendorApprovalDTO
{
    public int VendorId { get; set; }
    public int UserId { get; set; }
    public string AgencyName { get; set; } = string.Empty;
    public string OwnerName { get; set; } = string.Empty;
    public string BusinessLicenseNumber { get; set; } = string.Empty;
    public string OfficeAddress { get; set; } = string.Empty;
    public int FleetSize { get; set; }
    public string? TaxRegistrationNumber { get; set; }
    public int Status { get; set; }
    public string StatusText { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedOn { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public List<VendorDocumentDTO> Documents { get; set; } = new();
}