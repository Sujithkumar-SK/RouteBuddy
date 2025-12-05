using Kanini.RouteBuddy.Domain.Enums;

namespace Kanini.RouteBuddy.Application.Dto.Bus;

public class BusResponseDto
{
    public int BusId { get; set; }
    public string BusName { get; set; } = null!;
    public BusType BusType { get; set; }
    public int TotalSeats { get; set; }
    public string RegistrationNo { get; set; } = null!;

    public BusStatus Status { get; set; }
    public BusAmenities Amenities { get; set; }
    public string? DriverName { get; set; }
    public string? DriverContact { get; set; }
    public bool IsActive { get; set; }
    public int VendorId { get; set; }
    public string VendorName { get; set; } = null!;
    public int? SeatLayoutTemplateId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }
}