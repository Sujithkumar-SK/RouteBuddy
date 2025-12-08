using System.ComponentModel.DataAnnotations;

namespace Kanini.RouteBuddy.Application.Dto.Auth;

public class RegisterWithOtpRequestDto
{
    [Required, EmailAddress]
    public string Email { get; set; } = null!;

    [Required, MinLength(8)]
    public string Password { get; set; } = null!;

    [Required, RegularExpression(@"^[6-9]\d{9}$")]
    public string Phone { get; set; } = null!;

    [Required, Range(1, 2)]
    public int Role { get; set; } // 1=Customer, 2=Vendor

    [Required]
    public string RecaptchaToken { get; set; } = null!;
}
