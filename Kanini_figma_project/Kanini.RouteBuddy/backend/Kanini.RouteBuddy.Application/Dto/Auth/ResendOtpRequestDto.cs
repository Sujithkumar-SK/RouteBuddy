using System.ComponentModel.DataAnnotations;

namespace Kanini.RouteBuddy.Application.Dto.Auth;

public class ResendOtpRequestDto
{
    [Required, EmailAddress]
    public string Email { get; set; } = null!;

    [Required, MinLength(8)]
    public string Password { get; set; } = null!;

    [Required, RegularExpression(@"^[6-9]\d{9}$")]
    public string Phone { get; set; } = null!;

    [Required, Range(1, 2)]
    public int Role { get; set; }
}
