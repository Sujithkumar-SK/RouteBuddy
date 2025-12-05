namespace Kanini.RouteBuddy.Application.Dto.Auth;

public class LoginResponseDto
{
    public int UserId { get; set; }
    public string Email { get; set; } = null!;
    public string Role { get; set; } = null!;
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public DateTime AccessTokenExpiry { get; set; }
    public DateTime RefreshTokenExpiry { get; set; }
    public string Message { get; set; } = null!;
}
