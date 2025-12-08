using System.ComponentModel.DataAnnotations;

namespace Kanini.RouteBuddy.Application.Dto.Schedule;

public class UpdateScheduleDto : IValidatableObject
{
    [Required]
    public DateTime TravelDate { get; set; }

    [Required]
    public TimeSpan DepartureTime { get; set; }

    [Required]
    public TimeSpan ArrivalTime { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var today = DateTime.Today;
        var maxDate = today.AddDays(30);
        
        if (TravelDate.Date < today)
        {
            yield return new ValidationResult("Travel date cannot be in the past", new[] { nameof(TravelDate) });
        }
        
        if (TravelDate.Date > maxDate)
        {
            yield return new ValidationResult("Travel date cannot be more than 30 days from today", new[] { nameof(TravelDate) });
        }
    }
}