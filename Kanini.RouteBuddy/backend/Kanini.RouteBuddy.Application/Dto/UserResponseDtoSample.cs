namespace Kanini.RouteBuddy.Application.Dto;

public class UserResponseDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public DateTime DateOfBirth { get; set; }

    public int Age { get; set; }
}
