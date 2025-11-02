using System.ComponentModel.DataAnnotations;

namespace Kanini.RouteBuddy.Application.Dto;

public class BookingConfirmationDto
{
    [Required(ErrorMessage = "Booking ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Booking ID must be greater than 0")]
    public int BookingId { get; set; }

    [Required(ErrorMessage = "Payment reference ID is required")]
    [MaxLength(100, ErrorMessage = "Payment reference ID cannot exceed 100 characters")]
    public string PaymentReferenceId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Payment status is required")]
    public bool IsPaymentSuccessful { get; set; }
}
