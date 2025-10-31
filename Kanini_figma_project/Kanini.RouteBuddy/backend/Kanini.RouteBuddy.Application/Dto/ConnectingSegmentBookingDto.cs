using System.ComponentModel.DataAnnotations;

namespace Kanini.RouteBuddy.Application.Dto;

public class ConnectingSegmentBookingDto
{
    [Required(ErrorMessage = "Schedule ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Schedule ID must be greater than 0")]
    public int ScheduleId { get; set; }

    [Required(ErrorMessage = "At least one seat must be selected")]
    [MinLength(1, ErrorMessage = "At least one seat must be selected")]
    public List<string> SeatNumbers { get; set; } = new();

    [Required(ErrorMessage = "Passenger details are required")]
    [MinLength(1, ErrorMessage = "At least one passenger is required")]
    public List<PassengerDto> Passengers { get; set; } = new();

    [Required(ErrorMessage = "Boarding stop is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Boarding stop ID must be greater than 0")]
    public int BoardingStopId { get; set; }

    [Required(ErrorMessage = "Dropping stop is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Dropping stop ID must be greater than 0")]
    public int DroppingStopId { get; set; }
}
