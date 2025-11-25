using System.ComponentModel.DataAnnotations;

namespace Kanini.RouteBuddy.Application.Dto.Auth;

public class ForgotPasswordDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}