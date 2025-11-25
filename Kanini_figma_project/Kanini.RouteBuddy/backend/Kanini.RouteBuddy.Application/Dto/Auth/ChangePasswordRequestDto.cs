using System.ComponentModel.DataAnnotations;

namespace Kanini.RouteBuddy.Application.Dto.Auth;

public class ChangePasswordRequestDto
{
    [Required]
    public string CurrentPassword { get; set; } = null!;

    [Required, MinLength(8)]
    public string NewPassword { get; set; } = null!;
}
