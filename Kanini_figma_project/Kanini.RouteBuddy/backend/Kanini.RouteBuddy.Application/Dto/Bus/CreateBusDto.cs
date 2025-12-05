using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.AspNetCore.Http;
using Kanini.RouteBuddy.Domain.Enums;

namespace Kanini.RouteBuddy.Application.Dto.Bus;

public class CreateBusDto
{
    [Required(ErrorMessage = "Bus name is required")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Bus name must be between 3 and 100 characters")]
    [RegularExpression(@"^[a-zA-Z0-9\s\-_]+$", ErrorMessage = "Bus name can only contain letters, numbers, spaces, hyphens and underscores")]
    [DisplayName("Bus Name")]
    [Description("e.g., Volvo Express, City Connect, Night Rider")]
    public string BusName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Bus type is required")]
    [EnumDataType(typeof(BusType), ErrorMessage = "Invalid bus type")]
    [DisplayName("Bus Type")]
    [Description("1=AC, 2=NonAC, 3=Sleeper, 4=SemiSleeper, 5=Volvo, 6=Luxury")]
    public BusType BusType { get; set; }

    [Required(ErrorMessage = "Total seats is required")]
    [Range(10, 200, ErrorMessage = "Total seats must be between 10 and 200")]
    [DisplayName("Total Seats")]
    [Description("e.g., 45, 32, 50")]
    public int TotalSeats { get; set; }

    [Required(ErrorMessage = "Registration number is required")]
    [StringLength(15, MinimumLength = 8, ErrorMessage = "Registration number must be between 8 and 15 characters")]
    [RegularExpression(@"^[A-Z]{2}[0-9]{2}[A-Z]{1,2}[0-9]{4}$", ErrorMessage = "Invalid registration number format (e.g., MH12AB1234)")]
    [DisplayName("Registration Number")]
    [Description("e.g., MH12AB1234, KA05CD5678, TN09EF9012")]
    public string RegistrationNo { get; set; } = null!;

    [EnumDataType(typeof(BusAmenities), ErrorMessage = "Invalid amenities selection")]
    [DisplayName("Amenities")]
    [Description("e.g., 1539 for AC+WiFi+Charging+USB+ReadingLight+RecliningSeats")]
    public BusAmenities Amenities { get; set; } = BusAmenities.None;

    [Required(ErrorMessage = "Seat layout template is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Valid seat layout template must be selected")]
    [DisplayName("Seat Layout Template ID")]
    [Description("e.g., 1, 2, 3 (Required for booking functionality)")]
    public int SeatLayoutTemplateId { get; set; }

    [StringLength(100, MinimumLength = 2, ErrorMessage = "Driver name must be between 2 and 100 characters")]
    [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Driver name can only contain letters and spaces")]
    [DisplayName("Driver Name")]
    [Description("e.g., Rajesh Kumar, Suresh Patel (Optional)")]
    public string? DriverName { get; set; }

    [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Driver contact must be a valid 10-digit mobile number starting with 6-9")]
    [DisplayName("Driver Contact")]
    [Description("e.g., 9876543210, 8765432109 (Optional)")]
    public string? DriverContact { get; set; }

    [Required(ErrorMessage = "Registration certificate is required")]
    [DisplayName("Registration Certificate")]
    [Description("Upload PDF, JPG, JPEG, or PNG file (Max 5MB)")]
    public IFormFile RegistrationCertificate { get; set; } = null!;
}