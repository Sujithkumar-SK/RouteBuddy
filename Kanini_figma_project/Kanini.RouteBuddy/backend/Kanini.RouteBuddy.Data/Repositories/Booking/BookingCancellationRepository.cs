using System.Data;
using Kanini.RouteBuddy.Common;
using Kanini.RouteBuddy.Data.DatabaseContext;
using Kanini.RouteBuddy.Domain.Entities;
using Kanini.RouteBuddy.Domain.Enums;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Kanini.RouteBuddy.Data.Repositories.Booking;

public class BookingCancellationRepository : IBookingCancellationRepository
{
    private readonly RouteBuddyDatabaseContext _context;
    private readonly string _connectionString;
    private readonly ILogger<BookingCancellationRepository> _logger;

    public BookingCancellationRepository(
        RouteBuddyDatabaseContext context,
        IConfiguration configuration,
        ILogger<BookingCancellationRepository> logger)
    {
        _context = context;
        _connectionString = configuration.GetConnectionString(MagicStrings.ConfigKeys.DatabaseConnectionString) ?? string.Empty;
        _logger = logger;
    }

    public async Task<BookingCancellationData?> GetBookingForCancellationAsync(int bookingId, int customerId)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.BookingCancellationDataRetrievalStarted, bookingId, customerId);

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(MagicStrings.StoredProcedures.GetBookingForCancellation, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@BookingId", bookingId);
            command.Parameters.AddWithValue("@CustomerId", customerId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                var data = new BookingCancellationData
                {
                    BookingId = reader.GetInt32("BookingId"),
                    CustomerId = reader.GetInt32("CustomerId"),
                    PNRNo = reader.GetString("PNRNo"),
                    TotalAmount = reader.GetDecimal("TotalAmount"),
                    TravelDate = reader.GetDateTime("TravelDate"),
                    BookedAt = reader.GetDateTime("BookedAt"),
                    BookingStatus = reader.GetInt32("Status"),
                    PaymentMethod = reader.IsDBNull("PaymentMethod") ? 0 : reader.GetInt32("PaymentMethod"),
                    TransactionId = reader.IsDBNull("TransactionId") ? string.Empty : reader.GetString("TransactionId"),
                    HoursUntilTravel = reader.GetInt32("HoursUntilTravel")
                };

                _logger.LogInformation(MagicStrings.LogMessages.BookingCancellationDataRetrievalCompleted, bookingId);
                return data;
            }

            _logger.LogWarning(MagicStrings.LogMessages.BookingNotFoundForCancellation, bookingId, customerId);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.BookingCancellationDataRetrievalFailed, bookingId, customerId, ex.Message);
            throw;
        }
    }

    public async Task<bool> CancelBookingAsync(int bookingId, string reason, decimal penaltyAmount, string cancelledBy)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.BookingCancellationStarted, bookingId);

            // Update booking status to cancelled using EF Core
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking == null)
            {
                _logger.LogWarning(MagicStrings.LogMessages.BookingNotFound, bookingId);
                return false;
            }

            booking.Status = BookingStatus.Cancelled;
            booking.CancelledAt = DateTime.UtcNow;
            booking.UpdatedBy = cancelledBy;
            booking.UpdatedOn = DateTime.UtcNow;

            // Create cancellation record using EF Core
            var cancellation = new Cancellation
            {
                BookingId = bookingId,
                CancelledOn = DateTime.UtcNow,
                CancelledBy = cancelledBy,
                Reason = reason,
                PenaltyAmount = penaltyAmount,
                IsActive = true,
                CreatedBy = cancelledBy,
                CreatedOn = DateTime.UtcNow
            };

            _context.Cancellations.Add(cancellation);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation(MagicStrings.LogMessages.BookingCancellationCompleted, bookingId);
            return true;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, MagicStrings.LogMessages.BookingCancellationFailed, bookingId, ex.Message);
            throw;
        }
    }
}