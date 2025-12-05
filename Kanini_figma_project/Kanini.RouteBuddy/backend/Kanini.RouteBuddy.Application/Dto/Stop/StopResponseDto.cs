namespace Kanini.RouteBuddy.Application.Dto.Stop;

public class StopResponseDto
{
    public int StopId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Landmark { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedOn { get; set; }
}