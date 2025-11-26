using System.ComponentModel.DataAnnotations;

namespace Kanini.RouteBuddy.Application.Dto.Vendor;

public class VendorRejectionDTO
{
    [Required]
    [MaxLength(500)]
    public string RejectionReason { get; set; } = string.Empty;
}