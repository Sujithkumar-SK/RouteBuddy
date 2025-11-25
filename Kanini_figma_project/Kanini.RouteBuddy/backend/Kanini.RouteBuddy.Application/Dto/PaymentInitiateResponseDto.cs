namespace Kanini.RouteBuddy.Application.Dto;

public class PaymentInitiateResponseDto
{
    public string RazorpayOrderId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string PrefillName { get; set; } = string.Empty;
    public string PrefillEmail { get; set; } = string.Empty;
    public string PrefillContact { get; set; } = string.Empty;
    public int BookingId { get; set; }
}