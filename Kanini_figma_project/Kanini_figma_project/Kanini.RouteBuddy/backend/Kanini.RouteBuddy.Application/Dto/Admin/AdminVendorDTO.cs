using Kanini.RouteBuddy.Application.Dto.Vendor;
using Kanini.RouteBuddy.Domain.Enums;

namespace Kanini.RouteBuddy.Application.Dto.Admin
{
    public class AdminVendorDTO
    {
        public int VendorId { get; set; }
        public string AgencyName { get; set; } = string.Empty;
        public string OwnerName { get; set; } = string.Empty;
        public VendorStatus Status { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public int TotalBuses { get; set; }
        public ICollection<VendorDocumentDTO>? Documents { get; set; }
    }
}