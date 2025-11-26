using Kanini.RouteBuddy.Common;
using Kanini.RouteBuddy.Common.Utility;
using Kanini.RouteBuddy.Data.DatabaseContext;
using Kanini.RouteBuddy.Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Kanini.RouteBuddy.Data.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly RouteBuddyDatabaseContext _context;
    private readonly ILogger<PaymentRepository> _logger;
    private readonly string _connectionString;

    public PaymentRepository(
        RouteBuddyDatabaseContext context,
        ILogger<PaymentRepository> logger,
        IConfiguration configuration)
    {
        _context = context;
        _logger = logger;
        _connectionString = configuration.GetConnectionString("DatabaseConnectionString")!;
    }

    public async Task<Result<Payment>> CreatePaymentAsync(Payment payment)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.PaymentCreationStarted, payment.BookingId);

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            _logger.LogInformation(MagicStrings.LogMessages.PaymentCreationCompleted, payment.PaymentId);
            return Result.Success(payment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.PaymentCreationFailed, ex.Message);
            return Result.Failure<Payment>(
                Error.Failure("Payment.DatabaseError", MagicStrings.ErrorMessages.DatabaseError));
        }
    }

    public async Task<Result<Payment>> GetPaymentByIdAsync(int paymentId)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_GetPaymentById", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            
            command.Parameters.AddWithValue("@PaymentId", paymentId);
            
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            
            if (await reader.ReadAsync())
            {
                var payment = new Payment
                {
                    PaymentId = reader.GetInt32("PaymentId"),
                    BookingId = reader.GetInt32("BookingId"),
                    Amount = reader.GetDecimal("Amount"),
                    PaymentMethod = (Domain.Enums.PaymentMethod)reader.GetInt32("PaymentMethod"),
                    PaymentStatus = (Domain.Enums.PaymentStatus)reader.GetInt32("PaymentStatus"),
                    PaymentDate = reader.GetDateTime("PaymentDate"),
                    TransactionId = reader.GetString("TransactionId"),
                    IsActive = reader.GetBoolean("IsActive")
                };
                
                return Result.Success(payment);
            }
            
            return Result.Failure<Payment>(Error.NotFound(
                "Payment.NotFound", MagicStrings.ErrorMessages.PaymentNotFound));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.PaymentRetrievalFailed, ex.Message);
            return Result.Failure<Payment>(
                Error.Failure("Payment.DatabaseError", MagicStrings.ErrorMessages.DatabaseError));
        }
    }

    public async Task<Result<Payment>> GetPaymentByTransactionIdAsync(string transactionId)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_GetPaymentByTransactionId", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            
            command.Parameters.AddWithValue("@TransactionId", transactionId);
            
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            
            if (await reader.ReadAsync())
            {
                var payment = new Payment
                {
                    PaymentId = reader.GetInt32("PaymentId"),
                    BookingId = reader.GetInt32("BookingId"),
                    Amount = reader.GetDecimal("Amount"),
                    PaymentMethod = (Domain.Enums.PaymentMethod)reader.GetInt32("PaymentMethod"),
                    PaymentStatus = (Domain.Enums.PaymentStatus)reader.GetInt32("PaymentStatus"),
                    PaymentDate = reader.GetDateTime("PaymentDate"),
                    TransactionId = reader.GetString("TransactionId"),
                    IsActive = reader.GetBoolean("IsActive")
                };
                
                return Result.Success(payment);
            }
            
            return Result.Failure<Payment>(Error.NotFound(
                "Payment.NotFound", MagicStrings.ErrorMessages.PaymentNotFound));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.PaymentRetrievalFailed, ex.Message);
            return Result.Failure<Payment>(
                Error.Failure("Payment.DatabaseError", MagicStrings.ErrorMessages.DatabaseError));
        }
    }

    public async Task<Result<List<Payment>>> GetPaymentsByBookingIdAsync(int bookingId)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_GetPaymentsByBookingId", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            
            command.Parameters.AddWithValue("@BookingId", bookingId);
            
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            
            var payments = new List<Payment>();
            while (await reader.ReadAsync())
            {
                payments.Add(new Payment
                {
                    PaymentId = reader.GetInt32("PaymentId"),
                    BookingId = reader.GetInt32("BookingId"),
                    Amount = reader.GetDecimal("Amount"),
                    PaymentMethod = (Domain.Enums.PaymentMethod)reader.GetInt32("PaymentMethod"),
                    PaymentStatus = (Domain.Enums.PaymentStatus)reader.GetInt32("PaymentStatus"),
                    PaymentDate = reader.GetDateTime("PaymentDate"),
                    TransactionId = reader.GetString("TransactionId"),
                    IsActive = reader.GetBoolean("IsActive")
                });
            }
            
            return Result.Success(payments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.PaymentRetrievalFailed, ex.Message);
            return Result.Failure<List<Payment>>(
                Error.Failure("Payment.DatabaseError", MagicStrings.ErrorMessages.DatabaseError));
        }
    }

    public async Task<Result<Payment>> UpdatePaymentStatusAsync(int paymentId, int status, string? razorpayPaymentId, string updatedBy)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.PaymentUpdateStarted, paymentId);

            var payment = await _context.Payments
                .Where(p => p.PaymentId == paymentId)
                .FirstOrDefaultAsync();
            if (payment == null)
            {
                return Result.Failure<Payment>(Error.NotFound(
                    "Payment.NotFound", MagicStrings.ErrorMessages.PaymentNotFound));
            }

            payment.PaymentStatus = (Domain.Enums.PaymentStatus)status;
            payment.UpdatedBy = updatedBy;
            payment.UpdatedOn = DateTime.UtcNow;
            
            if (!string.IsNullOrEmpty(razorpayPaymentId))
            {
                payment.TransactionId = razorpayPaymentId;
            }

            if (status == (int)Domain.Enums.PaymentStatus.Success)
            {
                payment.IsActive = true;
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation(MagicStrings.LogMessages.PaymentUpdateCompleted, paymentId);
            return Result.Success(payment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.PaymentUpdateFailed, ex.Message);
            return Result.Failure<Payment>(
                Error.Failure("Payment.DatabaseError", MagicStrings.ErrorMessages.DatabaseError));
        }
    }
}