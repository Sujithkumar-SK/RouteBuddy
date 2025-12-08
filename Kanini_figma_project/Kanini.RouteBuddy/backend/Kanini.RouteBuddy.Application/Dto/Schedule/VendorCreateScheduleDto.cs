using System.ComponentModel.DataAnnotations;

namespace Kanini.RouteBuddy.Application.Dto.Schedule;

public class VendorCreateScheduleDto : IValidatableObject
{
    [Required(ErrorMessage = "Bus selection is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select a valid bus")]
    public int BusId { get; set; }

    [Required(ErrorMessage = "Route selection is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select a valid route")]
    public int RouteId { get; set; }

    [Required(ErrorMessage = "Travel date is required")]
    public DateTime TravelDate { get; set; }

    [Required(ErrorMessage = "Departure time is required")]
    public TimeSpan DepartureTime { get; set; }

    [Required(ErrorMessage = "Arrival time is required")]
    public TimeSpan ArrivalTime { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var today = DateTime.Today;
        var maxDate = today.AddDays(90); // Allow scheduling up to 90 days in advance

        if (TravelDate.Date < today)
        {
            yield return new ValidationResult(
                "Travel date cannot be in the past", 
                new[] { nameof(TravelDate) });
        }

        if (TravelDate.Date > maxDate)
        {
            yield return new ValidationResult(
                "Travel date cannot be more than 90 days from today", 
                new[] { nameof(TravelDate) });
        }

        // Handle overnight journeys
        var timeDifference = ArrivalTime > DepartureTime 
            ? ArrivalTime - DepartureTime 
            : TimeSpan.FromDays(1) - DepartureTime + ArrivalTime;

        if (timeDifference.TotalMinutes < 30)
        {
            yield return new ValidationResult(
                "Journey must be at least 30 minutes long", 
                new[] { nameof(ArrivalTime) });
        }

        if (timeDifference.TotalHours > 24)
        {
            yield return new ValidationResult(
                "Journey cannot exceed 24 hours", 
                new[] { nameof(ArrivalTime) });
        }

        // Validate reasonable departure times (not too early or too late)
        if (DepartureTime.Hours < 4 || DepartureTime.Hours > 23)
        {
            yield return new ValidationResult(
                "Departure time should be between 4:00 AM and 11:00 PM", 
                new[] { nameof(DepartureTime) });
        }
    }
}