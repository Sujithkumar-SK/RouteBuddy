using Kanini.RouteBuddy.Domain.Enums;

namespace Kanini.RouteBuddy.Application.Dto.Auth;

public class AuthResponseDto
{
    public int UserId { get; set; }
    public string Email { get; set; } = null!;
    public UserRole Role { get; set; }
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public DateTime AccessTokenExpiresAt { get; set; }
    public DateTime RefreshTokenExpiresAt { get; set; }
    public int? VendorId { get; set; }
    public string? VendorName { get; set; }
}