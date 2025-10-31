using System.ComponentModel.DataAnnotations;

namespace Kanini.RouteBuddy.Application.Dto;

public class SeatLayoutRequestDto
{
    [Required(ErrorMessage = "Schedule ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Schedule ID must be greater than 0")]
    public int ScheduleId { get; set; }

    [Required(ErrorMessage = "Travel date is required")]
    public DateTime TravelDate { get; set; }
}
