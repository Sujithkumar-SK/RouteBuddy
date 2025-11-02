using System.Data;
using Kanini.RouteBuddy.Common;
using Kanini.RouteBuddy.Data.DatabaseContext;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Kanini.RouteBuddy.Data.Repositories.Email;

public class SmartEmailRepository : ISmartEmailRepository
{
    private readonly string _connectionString;
    private readonly ILogger<SmartEmailRepository> _logger;

    public SmartEmailRepository(IConfiguration configuration, ILogger<SmartEmailRepository> logger)
    {
        _connectionString = configuration.GetConnectionString(MagicStrings.ConfigKeys.DatabaseConnectionString) 
            ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger;
    }

    public async Task<ConnectingBookingEmailData?> GetConnectingBookingDetailsForEmailAsync(int bookingId)
    {
        try
        {
            _logger.LogInformation(
                MagicStrings.LogMessages.SmartBookingEmailDataRetrievalStarted,
                bookingId
            );

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(MagicStrings.StoredProcedures.GetConnectingBookingDetailsForEmail, connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@BookingId", bookingId);

            using var reader = await command.ExecuteReaderAsync();

            ConnectingBookingEmailData? bookingData = null;
            var segments = new Dictionary<int, ConnectingSegmentEmailData>();

            while (await reader.ReadAsync())
            {
                if (bookingData == null)
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
                        PaymentMethod = reader.GetInt32("PaymentMethod"),
                        TransactionId = reader.GetString("TransactionId"),
                        PaymentDate = reader.GetDateTime("PaymentDate")
                    };
                }

                var segmentOrder = reader.GetInt32("SegmentOrder");
                if (!segments.ContainsKey(segmentOrder))
                {
                    segments[segmentOrder] = new ConnectingSegmentEmailData
                    {
                        SegmentOrder = segmentOrder,
                        BusName = reader.GetString("BusName"),
                        RegistrationNo = reader.GetString("RegistrationNo"),
                        Source = reader.GetString("Source"),
                        Destination = reader.GetString("Destination"),
                        DepartureTime = reader.GetTimeSpan(reader.GetOrdinal("DepartureTime")),
                        ArrivalTime = reader.GetTimeSpan(reader.GetOrdinal("ArrivalTime")),
                        VendorName = reader.GetString("VendorName"),
                        BoardingStopName = reader.GetString("BoardingStopName"),
                        BoardingStopLandmark = reader.GetString("BoardingStopLandmark"),
                        DroppingStopName = reader.GetString("DroppingStopName"),
                        DroppingStopLandmark = reader.GetString("DroppingStopLandmark"),
                        SegmentAmount = reader.GetDecimal("SegmentAmount")
                    };
                }

                var seatNumber = reader.GetString("SeatNumber");
                if (!segments[segmentOrder].SeatNumbers.Contains(seatNumber))
                {
                    segments[segmentOrder].SeatNumbers.Add(seatNumber);
                }

                var passenger = new ConnectingPassengerEmailData
                {
                    SeatNumber = seatNumber,
                    PassengerName = reader.GetString("PassengerName"),
                    PassengerAge = reader.GetInt32("PassengerAge"),
                    PassengerGender = reader.GetInt32("PassengerGender"),
                    SeatType = reader.GetInt32("SeatType"),
                    SeatPosition = reader.GetInt32("SeatPosition")
                };

                if (!segments[segmentOrder].Passengers.Any(p => p.SeatNumber == seatNumber))
                {
                    segments[segmentOrder].Passengers.Add(passenger);
                }
            }

            if (bookingData != null)
            {
                bookingData.Segments = segments.Values.OrderBy(s => s.SegmentOrder).ToList();
                
                _logger.LogInformation(
                    MagicStrings.LogMessages.SmartBookingEmailDataRetrievalCompleted,
                    bookingId,
                    bookingData.Segments.Sum(s => s.Passengers.Count)
                );
            }

            return bookingData;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                MagicStrings.LogMessages.SmartBookingEmailDataRetrievalFailed,
                bookingId,
                ex.Message
            );
            throw;
        }
    }
}