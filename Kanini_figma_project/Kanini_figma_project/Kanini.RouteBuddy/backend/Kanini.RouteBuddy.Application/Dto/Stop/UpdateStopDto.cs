using System.ComponentModel.DataAnnotations;

namespace Kanini.RouteBuddy.Application.Dto.Stop;

public class UpdateStopDto
{
    [Required(ErrorMessage = "Stop name is required")]
    [StringLength(100, ErrorMessage = "Stop name cannot exceed 100 characters")]
    public string Name { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "Landmark cannot exceed 200 characters")]
    public string? Landmark { get; set; }
}