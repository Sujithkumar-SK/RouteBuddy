using System.Data;
using Kanini.RouteBuddy.Common;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Kanini.RouteBuddy.Data.Repositories.Email;

public class EmailRepository : IEmailRepository
{
    private readonly string _connectionString;
    private readonly ILogger<EmailRepository> _logger;

    public EmailRepository(IConfiguration configuration, ILogger<EmailRepository> logger)
    {
        _connectionString =
            configuration.GetConnectionString(MagicStrings.ConfigKeys.DatabaseConnectionString)
            ?? string.Empty;
        _logger = logger;
        if (string.IsNullOrEmpty(_connectionString))
        {
            _logger.LogError("Database connection string not found");
        }
    }

    public async Task<BookingEmailData?> GetBookingDetailsForEmailAsync(int bookingId)
    {
        try
        {
            _logger.LogInformation(
                MagicStrings.LogMessages.BookingEmailDataRetrievalStarted,
                bookingId
            );

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                MagicStrings.StoredProcedures.GetBookingDetailsForEmail,
                connection
            )
            {
                CommandType = CommandType.StoredProcedure,
            };

            command.Parameters.AddWithValue("@BookingId", bookingId);
            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            BookingEmailData? bookingData = null;
            var passengers = new List<PassengerEmailData>();

            if (await reader.ReadAsync())
            {
                bookingData = new BookingEmailData
                {
                    BookingId = reader.GetInt32("BookingId"),
                    PNRNo = reader.GetString("PNRNo"),
                    TotalAmount = reader.GetDecimal("TotalAmount"),
                    TravelDate = reader.GetDateTime("TravelDate"),
                    BookedAt = reader.GetDateTime("BookedAt"),
                    FirstName = reader.GetString("FirstName"),
                    LastName = reader.GetString("LastName"),
                    CustomerEmail = reader.GetString("CustomerEmail"),
                    CustomerPhone = reader.GetString("CustomerPhone"),
                    BusName = reader.GetString("BusName"),
                    BusType = reader.GetInt32("BusType"),
                    RegistrationNo = reader.GetString("RegistrationNo"),
                    Source = reader.GetString("Source"),
                    Destination = reader.GetString("Destination"),
                    DepartureTime = reader.GetTimeSpan(reader.GetOrdinal("DepartureTime")),
                    ArrivalTime = reader.GetTimeSpan(reader.GetOrdinal("ArrivalTime")),
                    VendorName = reader.GetString("VendorName"),
                    PaymentMethod = reader.IsDBNull("PaymentMethod")
                        ? 0
                        : reader.GetInt32("PaymentMethod"),
                    TransactionId = reader.IsDBNull("TransactionId")
                        ? string.Empty
                        : reader.GetString("TransactionId"),
                    PaymentDate = reader.IsDBNull("PaymentDate")
                        ? DateTime.MinValue
                        : reader.GetDateTime("PaymentDate"),
                    BoardingStopName = reader.IsDBNull("BoardingStopName")
                        ? string.Empty
                        : reader.GetString("BoardingStopName"),
                    BoardingStopLandmark = reader.IsDBNull("BoardingStopLandmark")
                        ? string.Empty
                        : reader.GetString("BoardingStopLandmark"),
                    DroppingStopName = reader.IsDBNull("DroppingStopName")
                        ? string.Empty
                        : reader.GetString("DroppingStopName"),
                    DroppingStopLandmark = reader.IsDBNull("DroppingStopLandmark")
                        ? string.Empty
                        : reader.GetString("DroppingStopLandmark"),
                };
            }

            if (await reader.NextResultAsync())
            {
                while (await reader.ReadAsync())
                {
                    passengers.Add(
                        new PassengerEmailData
                        {
                            SeatNumber = reader.GetString("SeatNumber"),
                            PassengerName = reader.GetString("PassengerName"),
                            PassengerAge = reader.GetInt32("PassengerAge"),
                            PassengerGender = reader.GetInt32("PassengerGender"),
                            SeatType = reader.GetInt32("SeatType"),
                            SeatPosition = reader.GetInt32("SeatPosition"),
                            SegmentOrder = reader.GetInt32("SegmentOrder"),
                        }
                    );
                }
            }

            if (bookingData != null)
            {
                bookingData.Passengers = passengers;
                _logger.LogInformation(
                    MagicStrings.LogMessages.BookingEmailDataRetrievalCompleted,
                    bookingId,
                    passengers.Count
                );
            }

            return bookingData;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                MagicStrings.LogMessages.BookingEmailDataRetrievalFailed,
                bookingId,
                ex.Message
            );
            return null;
        }
    }
}
