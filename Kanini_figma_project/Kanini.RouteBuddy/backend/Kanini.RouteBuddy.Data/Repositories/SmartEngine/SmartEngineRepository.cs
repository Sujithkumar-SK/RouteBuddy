using System.Data;
using System.Data.SqlClient;
using Kanini.RouteBuddy.Common;
using Kanini.RouteBuddy.Common.Utility;
using Kanini.RouteBuddy.Domain.Entities;
using Kanini.RouteBuddy.Domain.Enums;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using VendorEntity = Kanini.RouteBuddy.Domain.Entities.Vendor;
using RouteEntity = Kanini.RouteBuddy.Domain.Entities.Route;
using BookingEntity = Kanini.RouteBuddy.Domain.Entities.Booking;
using CustomerEntity = Kanini.RouteBuddy.Domain.Entities.Customer;

namespace Kanini.RouteBuddy.Data.Repositories.SmartEngine;

public class SmartEngineRepository : ISmartEngineRepository
{
    private readonly string _connectionString;
    private readonly ILogger<SmartEngineRepository> _logger;

    public SmartEngineRepository(
        IConfiguration configuration,
        ILogger<SmartEngineRepository> logger
    )
    {
        _connectionString = configuration.GetConnectionString("DatabaseConnectionString")!;
        _logger = logger;
    }

    public async Task<Result<List<BusSchedule>>> FindConnectingRoutesAsync(
        string source,
        string destination,
        DateTime travelDate,
        string toggle
    )
    {
        try
        {
            _logger.LogInformation(
                MagicStrings.LogMessages.ConnectingRoutesSearchStarted,
                source,
                destination,
                travelDate
            );

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                MagicStrings.StoredProcedures.FindConnectingRoutes,
                connection
            )
            {
                CommandType = CommandType.StoredProcedure,
            };

            command.Parameters.AddWithValue("@Source", source);
            command.Parameters.AddWithValue("@Destination", destination);
            command.Parameters.AddWithValue("@TravelDate", travelDate.Date);
            command.Parameters.AddWithValue("@Toggle", toggle);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var busSchedules = new List<BusSchedule>();
            while (await reader.ReadAsync())
            {
                // First segment
                var firstSchedule = new BusSchedule
                {
                    ScheduleId = reader.GetInt32("FirstScheduleId"),
                    TravelDate = reader.GetDateTime("FirstTravelDate"),
                    DepartureTime = reader.GetTimeSpan(reader.GetOrdinal("FirstDepartureTime")),
                    ArrivalTime = reader.GetTimeSpan(reader.GetOrdinal("FirstArrivalTime")),
                    AvailableSeats = reader.GetInt32("FirstAvailableSeats"),
                    Bus = new Bus
                    {
                        BusName = reader.GetString("FirstBusName"),
                        Vendor = new VendorEntity { AgencyName = reader.GetString("FirstVendorName") },
                    },
                    Route = new RouteEntity
                    {
                        Source = reader.GetString("FirstSource"),
                        Destination = reader.GetString("FirstDestination"),
                        BasePrice = reader.GetDecimal("FirstPrice"),
                    },
                };

                // Second segment
                var secondSchedule = new BusSchedule
                {
                    ScheduleId = reader.GetInt32("SecondScheduleId"),
                    TravelDate = reader.GetDateTime("SecondTravelDate"),
                    DepartureTime = reader.GetTimeSpan(reader.GetOrdinal("SecondDepartureTime")),
                    ArrivalTime = reader.GetTimeSpan(reader.GetOrdinal("SecondArrivalTime")),
                    AvailableSeats = reader.GetInt32("SecondAvailableSeats"),
                    Bus = new Bus
                    {
                        BusName = reader.GetString("SecondBusName"),
                        Vendor = new VendorEntity { AgencyName = reader.GetString("SecondVendorName") },
                    },
                    Route = new RouteEntity
                    {
                        Source = reader.GetString("SecondSource"),
                        Destination = reader.GetString("SecondDestination"),
                        BasePrice = reader.GetDecimal("SecondPrice"),
                    },
                };

                busSchedules.Add(firstSchedule);
                busSchedules.Add(secondSchedule);
            }

            _logger.LogInformation(
                MagicStrings.LogMessages.ConnectingRoutesSearchCompleted,
                busSchedules.Count / 2
            );
            return Result.Success(busSchedules);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.ConnectingRoutesSearchFailed, ex.Message);
            return Result.Failure<List<BusSchedule>>(
                Error.Failure("ConnectingRoutes.Failed", MagicStrings.ErrorMessages.DatabaseError)
            );
        }
    }

    public async Task<Result<BookingEntity>> BookConnectingRouteAsync(
        int customerId,
        DateTime travelDate,
        decimal totalAmount,
        string segmentData
    )
    {
        try
        {
            _logger.LogInformation(
                MagicStrings.LogMessages.ConnectingBookingStarted,
                customerId,
                0,
                travelDate
            );

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                MagicStrings.StoredProcedures.BookConnectingRoute,
                connection
            )
            {
                CommandType = CommandType.StoredProcedure,
            };

            command.Parameters.AddWithValue("@CustomerId", customerId);
            command.Parameters.AddWithValue("@TravelDate", travelDate.Date);
            command.Parameters.AddWithValue("@TotalAmount", totalAmount);
            command.Parameters.AddWithValue("@SegmentData", segmentData);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                var result = reader.GetString("Result");

                if (result == "SUCCESS")
                {
                    var booking = new BookingEntity
                    {
                        BookingId = reader.GetInt32("BookingId"),
                        PNRNo = reader.GetString("PNR"),
                        TotalAmount = reader.GetDecimal("TotalAmount"),
                        TravelDate = travelDate,
                        CreatedOn = DateTime.UtcNow,
                        Customer = new CustomerEntity { CustomerId = customerId },
                    };

                    _logger.LogInformation(
                        MagicStrings.LogMessages.ConnectingBookingCompleted,
                        booking.PNRNo,
                        booking.BookingId
                    );
                    return Result.Success(booking);
                }
                else
                {
                    var errorMessage = reader.IsDBNull("ErrorMessage")
                        ? "Unknown error"
                        : reader.GetString("ErrorMessage");
                    _logger.LogError(
                        MagicStrings.LogMessages.ConnectingBookingFailed,
                        errorMessage
                    );
                    return Result.Failure<BookingEntity>(
                        Error.Failure("ConnectingBooking.Failed", errorMessage)
                    );
                }
            }

            _logger.LogError(
                MagicStrings.LogMessages.ConnectingBookingFailed,
                "No result returned"
            );
            return Result.Failure<BookingEntity>(
                Error.Failure(
                    "ConnectingBooking.NoResult",
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.ConnectingBookingFailed, ex.Message);
            return Result.Failure<BookingEntity>(
                Error.Failure("ConnectingBooking.Failed", MagicStrings.ErrorMessages.DatabaseError)
            );
        }
    }

    public async Task<Result<string>> ConfirmConnectingBookingAsync(
        int bookingId,
        string paymentReferenceId,
        bool isPaymentSuccessful
    )
    {
        try
        {
            _logger.LogInformation(
                MagicStrings.LogMessages.ConnectingBookingConfirmationStarted,
                bookingId
            );

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                MagicStrings.StoredProcedures.ConfirmBooking,
                connection
            )
            {
                CommandType = CommandType.StoredProcedure,
            };

            command.Parameters.AddWithValue("@BookingId", bookingId);
            command.Parameters.AddWithValue("@PaymentReferenceId", paymentReferenceId);
            command.Parameters.AddWithValue("@IsPaymentSuccessful", isPaymentSuccessful);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                var result = reader.GetString("Result");

                switch (result)
                {
                    case "CONFIRMED":
                        _logger.LogInformation(
                            MagicStrings.LogMessages.ConnectingBookingConfirmationCompleted,
                            bookingId
                        );
                        return Result.Success("Connecting booking confirmed successfully");

                    case "BOOKING_NOT_FOUND":
                        return Result.Failure<string>(
                            Error.NotFound(
                                "ConnectingBooking.NotFound",
                                MagicStrings.ErrorMessages.BookingNotFound
                            )
                        );

                    case "ALREADY_CONFIRMED":
                        return Result.Failure<string>(
                            Error.Failure(
                                "ConnectingBooking.AlreadyConfirmed",
                                MagicStrings.ErrorMessages.BookingAlreadyConfirmed
                            )
                        );

                    case "BOOKING_EXPIRED":
                        return Result.Failure<string>(
                            Error.Failure(
                                "ConnectingBooking.Expired",
                                MagicStrings.ErrorMessages.BookingExpired
                            )
                        );

                    case "PAYMENT_FAILED":
                        return Result.Success(
                            "Connecting booking cancelled due to payment failure"
                        );

                    default:
                        return Result.Failure<string>(
                            Error.Failure(
                                "ConnectingBooking.UnknownResult",
                                MagicStrings.ErrorMessages.UnexpectedError
                            )
                        );
                }
            }

            return Result.Failure<string>(
                Error.Failure(
                    "ConnectingBooking.NoResult",
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                MagicStrings.LogMessages.ConnectingBookingConfirmationFailed,
                ex.Message
            );
            return Result.Failure<string>(
                Error.Failure(
                    "ConnectingBooking.ConfirmationFailed",
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }
}
