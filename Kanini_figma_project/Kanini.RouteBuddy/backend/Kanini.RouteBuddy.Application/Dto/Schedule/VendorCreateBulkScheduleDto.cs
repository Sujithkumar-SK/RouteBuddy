using System.ComponentModel.DataAnnotations;

namespace Kanini.RouteBuddy.Application.Dto.Schedule;

public class VendorCreateBulkScheduleDto : IValidatableObject
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
        var maxDate = today.AddDays(90); // Allow 90 days for bulk scheduling
        
        if (StartDate.Date < today)
        {
            yield return new ValidationResult("Start date cannot be in the past", new[] { nameof(StartDate) });
        }
        
        if (EndDate.Date > maxDate)
        {
            yield return new ValidationResult("End date cannot be more than 90 days from today", new[] { nameof(EndDate) });
        }
        
        if (EndDate.Date < StartDate.Date)
        {
            yield return new ValidationResult("End date must be after start date", new[] { nameof(EndDate) });
        }

        var daysDifference = (EndDate.Date - StartDate.Date).Days;
        if (daysDifference > 30)
        {
            yield return new ValidationResult("Date range cannot exceed 30 days", new[] { nameof(EndDate) });
        }
        
        // Allow overnight journeys - validation will be handled in service layer
        var timeDifference = ArrivalTime > DepartureTime 
            ? ArrivalTime - DepartureTime 
            : TimeSpan.FromDays(1) - DepartureTime + ArrivalTime;

        if (timeDifference.TotalMinutes < 30)
        {
            yield return new ValidationResult("Journey must be at least 30 minutes long", new[] { nameof(ArrivalTime) });
        }

        if (timeDifference.TotalHours > 24)
        {
            yield return new ValidationResult("Journey cannot exceed 24 hours", new[] { nameof(ArrivalTime) });
        }

        if (OperatingDays.Count == 0)
        {
            yield return new ValidationResult("At least one operating day must be selected", new[] { nameof(OperatingDays) });
        }
    }
}