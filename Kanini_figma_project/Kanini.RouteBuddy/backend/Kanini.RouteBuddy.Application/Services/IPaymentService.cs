using Kanini.RouteBuddy.Application.Dto;
using Kanini.RouteBuddy.Common.Utility;

namespace Kanini.RouteBuddy.Application.Services;

public interface IPaymentService
{
    Task<Result<PaymentInitiateResponseDto>> InitiatePaymentAsync(PaymentInitiateRequestDto request, int customerId, string createdBy);
    Task<Result<PaymentResponseDto>> VerifyPaymentAsync(PaymentVerifyRequestDto request, string updatedBy);
    Task<Result<List<PaymentResponseDto>>> GetPaymentsByBookingIdAsync(int bookingId);
}