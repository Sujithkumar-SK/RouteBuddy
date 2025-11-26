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

    public async Task<ConnectingBookingEmailData?> GetConnectingBookingDetailsForEmailAsync(int bookingId)
    {
        try
        {
            _logger.LogInformation(
                "Connecting booking email data retrieval started for BookingId: {BookingId}",
                bookingId
            );

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                "sp_GetConnectingBookingDetailsForEmail",
                connection
            )
            {
                CommandType = CommandType.StoredProcedure,
            };

            command.Parameters.AddWithValue("@BookingId", bookingId);
            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            ConnectingBookingEmailData? bookingData = null;
            var segments = new List<SegmentEmailData>();

            // First result set: booking info
            if (await reader.ReadAsync())
            {
                bookingData = new ConnectingBookingEmailData
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
                    OverallSource = reader.GetString("OverallSource"),
                    OverallDestination = reader.GetString("OverallDestination"),
                    PaymentMethod = reader.IsDBNull("PaymentMethod") ? 0 : reader.GetInt32("PaymentMethod"),
                    TransactionId = reader.IsDBNull("TransactionId") ? string.Empty : reader.GetString("TransactionId"),
                    PaymentDate = reader.IsDBNull("PaymentDate") ? DateTime.MinValue : reader.GetDateTime("PaymentDate")
                };
            }

            // Second result set: segments
            if (await reader.NextResultAsync())
            {
                var segmentDict = new Dictionary<int, SegmentEmailData>();
                
                while (await reader.ReadAsync())
                {
                    var segmentOrder = reader.GetInt32("SegmentOrder");
                    
                    if (!segmentDict.ContainsKey(segmentOrder))
                    {
                        segmentDict[segmentOrder] = new SegmentEmailData
                        {
                            SegmentOrder = segmentOrder,
                            BusName = reader.GetString("BusName"),
                            Source = reader.GetString("Source"),
                            Destination = reader.GetString("Destination"),
                            DepartureTime = reader.GetTimeSpan(reader.GetOrdinal("DepartureTime")),
                            ArrivalTime = reader.GetTimeSpan(reader.GetOrdinal("ArrivalTime")),
                            SegmentAmount = reader.GetDecimal("SegmentAmount"),
                            SeatNumbers = new List<string>()
                        };
                    }
                    
                    segmentDict[segmentOrder].SeatNumbers.Add(reader.GetString("SeatNumber"));
                }
                
                segments = segmentDict.Values.OrderBy(s => s.SegmentOrder).ToList();
            }

            if (bookingData != null)
            {
                bookingData.Segments = segments;
                _logger.LogInformation(
                    "Connecting booking email data retrieval completed for BookingId: {BookingId}, Segments: {SegmentCount}",
                    bookingId,
                    segments.Count
                );
            }

            return bookingData;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Connecting booking email data retrieval failed for BookingId: {BookingId}: {Error}",
                bookingId,
                ex.Message
            );
            return null;
        }
    }
}
