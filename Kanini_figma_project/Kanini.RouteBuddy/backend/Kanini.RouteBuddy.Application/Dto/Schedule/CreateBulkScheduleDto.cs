using System.ComponentModel.DataAnnotations;

namespace Kanini.RouteBuddy.Application.Dto.Schedule;

public class CreateBulkScheduleDto : IValidatableObject
{
    [Required]
    public int BusId { get; set; }

    [Required]
    public int RouteId { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    [Required]
    public TimeSpan DepartureTime { get; set; }

    [Required]
    public TimeSpan ArrivalTime { get; set; }

    public List<DayOfWeek> OperatingDays { get; set; } = new List<DayOfWeek>();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var today = DateTime.Today;
        var maxDate = today.AddDays(30);
        
        if (StartDate.Date < today)
        {
            yield return new ValidationResult("Start date cannot be in the past", new[] { nameof(StartDate) });
        }
        
        if (EndDate.Date > maxDate)
        {
            yield return new ValidationResult("End date cannot be more than 30 days from today", new[] { nameof(EndDate) });
        }
        
        if (EndDate.Date < StartDate.Date)
        {
            yield return new ValidationResult("End date must be after start date", new[] { nameof(EndDate) });
        }
        
        if (ArrivalTime <= DepartureTime)
        {
            yield return new ValidationResult("Arrival time must be after departure time", new[] { nameof(ArrivalTime) });
        }
    }
}