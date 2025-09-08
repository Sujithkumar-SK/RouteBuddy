using System.ComponentModel.DataAnnotations;
public class Route : BaseEntity
{
  [Key]
  public int RouteId { get; set; }

  [Required, MaxLength(100)]
  public string Source { get; set; } = string.Empty;

  [Required, MaxLength(100)]
  public string Destination { get; set; } = string.Empty;

  [Range(0, double.MaxValue)]
  public double Distance { get; set; }

  public TimeSpan Duration { get; set; }

  public ICollection<BusSchedule> Schedules { get; set; } = new List<BusSchedule>();

  public ICollection<Stop> Stops { get; set; } = new List<Stop>();
}