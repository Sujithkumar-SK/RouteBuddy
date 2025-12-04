using System.ComponentModel.DataAnnotations;
using Kanini.RouteBuddy.Domain.Enums;

namespace Kanini.RouteBuddy.Application.Dto;

public class BookingConfirmationDto
{
    [Required(ErrorMessage = "Booking ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Booking ID must be greater than 0")]
    public int BookingId { get; set; }

    [Required(ErrorMessage = "Payment reference ID is required")]
    [MaxLength(100, ErrorMessage = "Payment reference ID cannot exceed 100 characters")]
    public string PaymentReferenceId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Payment method is required")]
    public PaymentMethod PaymentMethod { get; set; }

    public bool IsPaymentSuccessful { get; set; } = true;
}
