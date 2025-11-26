namespace Kanini.RouteBuddy.Application.Dto.Auth;

public class RegistrationDataDto
{
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string Phone { get; set; } = null!;
}
