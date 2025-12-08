namespace Kanini.RouteBuddy.Application.Dto.Auth;

public class RegistrationResponseDto
{
    public int UserId { get; set; }
    public string Email { get; set; } = null!;
    public string Role { get; set; } = null!;
    public int? CustomerId { get; set; }
    public bool RequiresVendorProfile { get; set; }
    public string Message { get; set; } = null!;
}
