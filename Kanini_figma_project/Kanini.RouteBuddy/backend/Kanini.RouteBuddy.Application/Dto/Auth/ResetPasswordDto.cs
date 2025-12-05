using System.ComponentModel.DataAnnotations;
using Kanini.RouteBuddy.Common.Validators;

namespace Kanini.RouteBuddy.Application.Dto.Auth;

public class ResetPasswordDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    [StrongPassword]
    public string NewPassword { get; set; } = string.Empty;
}