using System.ComponentModel.DataAnnotations;

public class Stop : BaseEntity
{
    public int StopId { get; set; }
    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(250)]
    public string? Landmark { get; set; }

    public bool IsDeleted { get; set; } = false;

    public int RouteId { get; set; }
    public Route Route { get; set; } = null!;
}
