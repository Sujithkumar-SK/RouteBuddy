using System.ComponentModel.DataAnnotations;

namespace Kanini.RouteBuddy.Application.Dto;

public class CreateUserRequestDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [RegularExpression(@"^[6-9]\d{9}$")]
    public string Phone { get; set; } = string.Empty;
}