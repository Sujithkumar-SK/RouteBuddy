using System.ComponentModel.DataAnnotations;

namespace Kanini.RouteBuddy.Application.Dto.Auth;

public class CompleteCustomerProfileDto
{
    [Required]
    public string FirstName { get; set; } = null!;

    public string? MiddleName { get; set; }

    [Required]
    public string LastName { get; set; } = null!;

    [Required]
    public DateTime DateOfBirth { get; set; }

    [Required, Range(1, 3)]
    public int Gender { get; set; }
}
