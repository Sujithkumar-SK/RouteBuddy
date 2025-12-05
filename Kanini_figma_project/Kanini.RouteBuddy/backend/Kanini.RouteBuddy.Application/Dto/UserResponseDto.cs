using Kanini.RouteBuddy.Domain.Enums;

namespace Kanini.RouteBuddy.Application.Dto;

public class UserResponseDto
{
    public int UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public bool IsEmailVerified { get; set; }
    public bool IsActive { get; set; }
    public DateTime? LastLogin { get; set; }
    public DateTime CreatedOn { get; set; }
}