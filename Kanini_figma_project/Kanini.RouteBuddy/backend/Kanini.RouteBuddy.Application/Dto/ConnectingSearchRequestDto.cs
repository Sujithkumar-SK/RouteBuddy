using System.ComponentModel.DataAnnotations;

namespace Kanini.RouteBuddy.Application.Dto;

public class ConnectingSearchRequestDto
{
    [Required(ErrorMessage = "Source is required")]
    [MaxLength(100, ErrorMessage = "Source cannot exceed 100 characters")]
    public string Source { get; set; } = string.Empty;

    [Required(ErrorMessage = "Destination is required")]
    [MaxLength(100, ErrorMessage = "Destination cannot exceed 100 characters")]
    public string Destination { get; set; } = string.Empty;

    [Required(ErrorMessage = "Travel date is required")]
    public DateTime TravelDate { get; set; }

    [RegularExpression(
        "^(cheapest|fastest)$",
        ErrorMessage = "Toggle must be 'cheapest' or 'fastest'"
    )]
    public string Toggle { get; set; } = "cheapest";
}
