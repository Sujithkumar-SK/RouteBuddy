using System.ComponentModel.DataAnnotations;

namespace Kanini.RouteBuddy.Application.Dto;

public class SmartBookingValidationDto
{
    [Required(ErrorMessage = "Booking ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Booking ID must be greater than 0")]
    public int BookingId { get; set; }

    [Required(ErrorMessage = "Customer email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address format")]
    [MaxLength(255, ErrorMessage = "Email address cannot exceed 255 characters")]
    public string CustomerEmail { get; set; } = string.Empty;

    [Required(ErrorMessage = "PNR number is required")]
    [StringLength(12, MinimumLength = 6, ErrorMessage = "PNR must be between 6 and 12 characters")]
    public string PNRNo { get; set; } = string.Empty;

    [Required(ErrorMessage = "At least one segment is required")]
    [MinLength(1, ErrorMessage = "At least one segment is required for connecting route")]
    public List<SmartSegmentValidationDto> Segments { get; set; } = new();

    [Range(0.01, double.MaxValue, ErrorMessage = "Total amount must be greater than 0")]
    public decimal TotalAmount { get; set; }
}

public class SmartSegmentValidationDto
{
    [Required(ErrorMessage = "Segment order is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Segment order must be greater than 0")]
    public int SegmentOrder { get; set; }

    [Required(ErrorMessage = "Bus name is required")]
    [MaxLength(100, ErrorMessage = "Bus name cannot exceed 100 characters")]
    public string BusName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Source is required")]
    [MaxLength(100, ErrorMessage = "Source cannot exceed 100 characters")]
    public string Source { get; set; } = string.Empty;

    [Required(ErrorMessage = "Destination is required")]
    [MaxLength(100, ErrorMessage = "Destination cannot exceed 100 characters")]
    public string Destination { get; set; } = string.Empty;

    [Required(ErrorMessage = "At least one seat is required")]
    [MinLength(1, ErrorMessage = "At least one seat number is required")]
    public List<string> SeatNumbers { get; set; } = new();

    [Range(0.01, double.MaxValue, ErrorMessage = "Segment amount must be greater than 0")]
    public decimal SegmentAmount { get; set; }
}