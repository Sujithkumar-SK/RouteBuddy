using System.ComponentModel.DataAnnotations;

namespace Kanini.RouteBuddy.Application.Dto;

public class ConnectingBookingRequestDto
{
    [Required(ErrorMessage = "Customer ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Customer ID must be greater than 0")]
    public int CustomerId { get; set; }

    [Required(ErrorMessage = "Travel date is required")]
    public DateTime TravelDate { get; set; }

    [Required(ErrorMessage = "At least one segment is required")]
    [MinLength(2, ErrorMessage = "Connecting routes must have at least 2 segments")]
    [MaxLength(5, ErrorMessage = "Maximum 5 segments allowed")]
    public List<ConnectingSegmentBookingDto> Segments { get; set; } = new();
}
