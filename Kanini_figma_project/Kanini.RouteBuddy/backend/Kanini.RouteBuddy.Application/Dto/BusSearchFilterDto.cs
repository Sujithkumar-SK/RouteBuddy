using System.ComponentModel.DataAnnotations;

namespace Kanini.RouteBuddy.Application.Dto;

public class BusSearchFilterDto
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

    // Filter Options
    public List<int>? BusTypes { get; set; }
    public List<int>? Amenities { get; set; }

    [Range(typeof(TimeSpan), "00:00:00", "23:59:59", ErrorMessage = "Invalid departure time range")]
    public TimeSpan? DepartureTimeFrom { get; set; }

    [Range(typeof(TimeSpan), "00:00:00", "23:59:59", ErrorMessage = "Invalid departure time range")]
    public TimeSpan? DepartureTimeTo { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Minimum price must be greater than or equal to 0")]
    public decimal? MinPrice { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Maximum price must be greater than or equal to 0")]
    public decimal? MaxPrice { get; set; }

    [RegularExpression(
        "^(price_asc|price_desc|time_asc|duration_asc|rating_desc)$",
        ErrorMessage = "Invalid sort option"
    )]
    public string? SortBy { get; set; } = "time_asc";
}
