using System.ComponentModel.DataAnnotations;

namespace Kanini.RouteBuddy.Application.Dto.Stop;

public class PlaceAutocompleteRequestDto
{
    [Required(ErrorMessage = "Query is required")]
    [MinLength(1, ErrorMessage = "Query must be at least 1 character")]
    [MaxLength(50, ErrorMessage = "Query cannot exceed 50 characters")]
    public string Query { get; set; } = string.Empty;

    [Range(1, 20, ErrorMessage = "Limit must be between 1 and 20")]
    public int Limit { get; set; } = 10;
}

public class PlaceAutocompleteResponseDto
{
    public int StopId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Landmark { get; set; }
    public string DisplayText => string.IsNullOrEmpty(Landmark) ? Name : $"{Name} - {Landmark}";
}