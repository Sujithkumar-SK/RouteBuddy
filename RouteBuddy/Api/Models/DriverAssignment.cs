using System.ComponentModel.DataAnnotations;

public class DriverAssignment : BaseEntity
{
  [Key]
  public int AssignmentId { get; set; }
  public int ScheduleId { get; set; }
  public BusSchedule Schedule { get; set; } = null!;

  public int DriverId { get; set; }
  public Driver Driver { get; set; } = null!;
}
