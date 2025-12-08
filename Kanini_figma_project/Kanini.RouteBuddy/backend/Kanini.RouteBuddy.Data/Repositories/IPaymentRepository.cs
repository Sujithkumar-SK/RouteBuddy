using Kanini.RouteBuddy.Common.Utility;
using Kanini.RouteBuddy.Domain.Entities;

namespace Kanini.RouteBuddy.Data.Repositories;

public interface IPaymentRepository
{
    Task<Result<Payment>> CreatePaymentAsync(Payment payment);
    Task<Result<Payment>> GetPaymentByIdAsync(int paymentId);
    Task<Result<Payment>> GetPaymentByTransactionIdAsync(string transactionId);
    Task<Result<List<Payment>>> GetPaymentsByBookingIdAsync(int bookingId);
    Task<Result<Payment>> UpdatePaymentStatusAsync(int paymentId, int status, string? razorpayPaymentId, string updatedBy);
}