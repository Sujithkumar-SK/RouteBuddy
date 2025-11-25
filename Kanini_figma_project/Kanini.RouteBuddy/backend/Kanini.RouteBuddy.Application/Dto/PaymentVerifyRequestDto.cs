using System.ComponentModel.DataAnnotations;

namespace Kanini.RouteBuddy.Application.Dto;

public class PaymentVerifyRequestDto
{
    [Required(ErrorMessage = "Razorpay payment ID is required")]
    public string RazorpayPaymentId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Razorpay order ID is required")]
    public string RazorpayOrderId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Razorpay signature is required")]
    public string RazorpaySignature { get; set; } = string.Empty;

    [Required(ErrorMessage = "Booking ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Booking ID must be greater than 0")]
    public int BookingId { get; set; }
}