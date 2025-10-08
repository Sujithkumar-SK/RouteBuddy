using System.ComponentModel.DataAnnotations;
namespace Kanini.RouteBuddy.Domain.Entities;

public class Fare : BaseEntity
{
    [Key]
    public int FareId { get; set; }

    public int ScheduleId { get; set; }
    public BusSchedule Schedule { get; set; } = null!;

    [Required, MaxLength(50)]
    public string SeatType { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }
}
