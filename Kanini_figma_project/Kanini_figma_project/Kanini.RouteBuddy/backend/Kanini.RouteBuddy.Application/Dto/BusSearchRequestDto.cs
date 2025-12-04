using System.ComponentModel.DataAnnotations;

namespace Kanini.RouteBuddy.Application.Dto;

public class BusSearchRequestDto
{
    [Required(ErrorMessage = "Source is required")]
    [MaxLength(100, ErrorMessage = "Source cannot exceed 100 characters")]
    [MinLength(2, ErrorMessage = "Source must be at least 2 characters")]
    public string Source { get; set; } = string.Empty;

    [Required(ErrorMessage = "Destination is required")]
    [MaxLength(100, ErrorMessage = "Destination cannot exceed 100 characters")]
    [MinLength(2, ErrorMessage = "Destination must be at least 2 characters")]
    public string Destination { get; set; } = string.Empty;

    [Required(ErrorMessage = "Travel date is required")]
    public DateTime TravelDate { get; set; }
}
