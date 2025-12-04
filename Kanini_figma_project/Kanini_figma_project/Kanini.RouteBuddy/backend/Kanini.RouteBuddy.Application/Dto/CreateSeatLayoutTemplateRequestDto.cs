using System.ComponentModel.DataAnnotations;
using Kanini.RouteBuddy.Domain.Enums;

namespace Kanini.RouteBuddy.Application.Dto;

public class CreateSeatLayoutTemplateRequestDto
{
    [Required(ErrorMessage = "Template name is required")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Template name must be between 3 and 100 characters")]
    public string TemplateName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Total seats is required")]
    [Range(1, 100, ErrorMessage = "Total seats must be between 1 and 100")]
    public int TotalSeats { get; set; }

    [Required(ErrorMessage = "Bus type is required")]
    public BusType BusType { get; set; }

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Seat details are required")]
    [MinLength(1, ErrorMessage = "At least one seat detail is required")]
    public List<SeatLayoutDetailRequestDto> SeatDetails { get; set; } = new();
}