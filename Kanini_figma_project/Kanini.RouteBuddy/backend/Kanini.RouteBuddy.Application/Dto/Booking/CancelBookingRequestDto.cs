using System.ComponentModel.DataAnnotations;

namespace Kanini.RouteBuddy.Application.Dto.Booking;

public class CancelBookingRequestDto
{
    [Required(ErrorMessage = "Booking ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Invalid booking ID")]
    public int BookingId { get; set; }

    [Required(ErrorMessage = "Cancellation reason is required")]
    [StringLength(250, MinimumLength = 10, ErrorMessage = "Reason must be between 10 and 250 characters")]
    public string Reason { get; set; } = string.Empty;
}

public class CancelBookingResponseDto
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public decimal RefundAmount { get; set; }
    public decimal PenaltyAmount { get; set; }
    public string RefundMethod { get; set; } = string.Empty;
    public DateTime ProcessedAt { get; set; }
}