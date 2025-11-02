using System.ComponentModel.DataAnnotations;
using Kanini.RouteBuddy.Domain.Enums;

namespace Kanini.RouteBuddy.Application.Dto;

public class PassengerDto
{
    [Required(ErrorMessage = "Passenger name is required")]
    [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Age is required")]
    [Range(1, 120, ErrorMessage = "Age must be between 1 and 120")]
    public int Age { get; set; }

    [Required(ErrorMessage = "Gender is required")]
    public Gender Gender { get; set; }
}
