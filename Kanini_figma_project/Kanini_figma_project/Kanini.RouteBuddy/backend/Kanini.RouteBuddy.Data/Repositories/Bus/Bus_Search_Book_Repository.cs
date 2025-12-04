using System.Data;
using Kanini.RouteBuddy.Common;
using Kanini.RouteBuddy.Common.Utility;
using Kanini.RouteBuddy.Data.DatabaseContext;
using Kanini.RouteBuddy.Domain.Entities;
using Kanini.RouteBuddy.Domain.Enums;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RouteEntity = Kanini.RouteBuddy.Domain.Entities.Route;
using RouteStopEntity = Kanini.RouteBuddy.Domain.Entities.RouteStop;
using StopEntity = Kanini.RouteBuddy.Domain.Entities.Stop;
using VendorEntity = Kanini.RouteBuddy.Domain.Entities.Vendor;
using BookingEntity = Kanini.RouteBuddy.Domain.Entities.Booking;

namespace Kanini.RouteBuddy.Data.Repositories.Buses;

public class Bus_Search_Book_Repository : IBus_Search_Book_Repository
{
    private readonly string _connectionString;
    private readonly RouteBuddyDatabaseContext _context;
    private readonly ILogger<Bus_Search_Book_Repository> _logger;

    public Bus_Search_Book_Repository(
        IConfiguration configuration,
        RouteBuddyDatabaseContext context,
        ILogger<Bus_Search_Book_Repository> logger
    )
    {
        _connectionString = configuration.GetConnectionString("DatabaseConnectionString")!;
        _context = context;
        _logger = logger;
    }

    public async Task<Result<List<BusSchedule>>> SearchBusesAsync(
        string source,
        string destination,
        DateTime travelDate
    )
    {
        try
        {
            _logger.LogInformation(
                MagicStrings.LogMessages.BusSearchStarted,
                source,
                destination,
                travelDate
            );

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                MagicStrings.StoredProcedures.SearchBuses,
                connection
            )
            {
                CommandType = CommandType.StoredProcedure,
            };

            command.Parameters.AddWithValue("@Source", source);
            command.Parameters.AddWithValue("@Destination", destination);
            command.Parameters.AddWithValue("@TravelDate", travelDate.Date);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var schedules = new List<BusSchedule>();
            while (await reader.ReadAsync())
            {
                var schedule = new BusSchedule
                {
                    ScheduleId = reader.GetInt32("ScheduleId"),
                    TravelDate = reader.GetDateTime("TravelDate"),
                    DepartureTime = reader.GetTimeSpan(reader.GetOrdinal("DepartureTime")),
                    ArrivalTime = reader.GetTimeSpan(reader.GetOrdinal("ArrivalTime")),
                    AvailableSeats = reader.GetInt32("AvailableSeats"),
                    Bus = new Bus
                    {
                        BusId = reader.GetInt32("BusId"),
                        BusName = reader.GetString("BusName"),
                        BusType = (BusType)reader.GetInt32("BusType"),
                        TotalSeats = reader.GetInt32("TotalSeats"),
                        Amenities = (BusAmenities)reader.GetInt32("Amenities"),
                        Vendor = new VendorEntity { AgencyName = reader.GetString("VendorName") },
                    },
                    Route = new RouteEntity
                    {
                        Source = reader.GetString("Source"),
                        Destination = reader.GetString("Destination"),
                        BasePrice = reader.GetDecimal("BasePrice"),
                    },
                };
                schedules.Add(schedule);
            }

            _logger.LogInformation(MagicStrings.LogMessages.BusSearchCompleted, schedules.Count);
            return Result.Success(schedules);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.BusSearchFailed, ex.Message);
            return Result.Failure<List<BusSchedule>>(
                Error.Failure("BusSearch.Failed", MagicStrings.ErrorMessages.DatabaseError)
            );
        }
    }

    public async Task<Result<List<SeatLayoutDetail>>> GetSeatLayoutAsync(
        int scheduleId,
        DateTime travelDate
    )
    {
        try
        {
            _logger.LogInformation(
                MagicStrings.LogMessages.SeatLayoutStarted,
                scheduleId,
                travelDate
            );

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                MagicStrings.StoredProcedures.GetBusSeatLayout,
                connection
            )
            {
                CommandType = CommandType.StoredProcedure,
            };

            command.Parameters.AddWithValue("@ScheduleId", scheduleId);
            command.Parameters.AddWithValue("@TravelDate", travelDate.Date);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var seatDetails = new List<SeatLayoutDetail>();
            BusSchedule? busSchedule = null;

            while (await reader.ReadAsync())
            {
                if (busSchedule == null)
                {
                    busSchedule = new BusSchedule
                    {
                        ScheduleId = reader.GetInt32("ScheduleId"),
                        Bus = new Bus
                        {
                            BusName = reader.GetString("BusName"),
                            BusType = (BusType)reader.GetInt32("BusType"),
                            Amenities = (BusAmenities)reader.GetInt32("Amenities"),
                            TotalSeats = reader.GetInt32("TotalSeats"),
                        },
                        Route = new RouteEntity { BasePrice = reader.GetDecimal("BasePrice") },
                        AvailableSeats = reader.GetInt32("AvailableSeats"),
                    };
                }

                var isBooked = !reader.IsDBNull("BookedSeatId");
                var seatDetail = new SeatLayoutDetail
                {
                    SeatLayoutDetailId = reader.GetInt32("ScheduleId"),
                    SeatNumber = reader.GetString("SeatNumber"),
                    SeatType = (SeatType)reader.GetInt32("SeatType"),
                    SeatPosition = (SeatPosition)reader.GetInt32("SeatPosition"),
                    RowNumber = reader.GetInt32("RowNumber"),
                    ColumnNumber = reader.GetInt32("ColumnNumber"),
                    PriceTier = (PriceTier)reader.GetInt32("PriceTier"),
                    IsBooked = isBooked,
                };

                seatDetail.SeatLayoutTemplate = new SeatLayoutTemplate
                {
                    BusType = busSchedule.Bus.BusType,
                };

                seatDetails.Add(seatDetail);
            }

            if (seatDetails.Any() && busSchedule != null)
            {
                seatDetails[0].CreatedBy =
                    $"{busSchedule.Bus.BusName}|{(int)busSchedule.Bus.BusType}|{(int)busSchedule.Bus.Amenities}|{busSchedule.Route.BasePrice}|{busSchedule.AvailableSeats}|{busSchedule.Bus.TotalSeats - busSchedule.AvailableSeats}";
            }

            if (!seatDetails.Any())
            {
                _logger.LogWarning(
                    MagicStrings.LogMessages.SeatLayoutFailed,
                    MagicStrings.ErrorMessages.ScheduleNotFound
                );
                return Result.Failure<List<SeatLayoutDetail>>(
                    Error.NotFound(
                        "SeatLayout.NotFound",
                        MagicStrings.ErrorMessages.ScheduleNotFound
                    )
                );
            }

            _logger.LogInformation(MagicStrings.LogMessages.SeatLayoutCompleted, seatDetails.Count);
            return Result.Success(seatDetails);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.SeatLayoutFailed, ex.Message);
            return Result.Failure<List<SeatLayoutDetail>>(
                Error.Failure("SeatLayout.Failed", MagicStrings.ErrorMessages.DatabaseError)
            );
        }
    }

    public async Task<Result<List<SeatLayoutDetail>>> ValidateSeatsAvailabilityAsync(
        int scheduleId,
        DateTime travelDate,
        List<string> seatNumbers
    )
    {
        try
        {
            _logger.LogInformation(
                MagicStrings.LogMessages.SeatValidationStarted,
                seatNumbers.Count
            );

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                MagicStrings.StoredProcedures.ValidateSeatsAvailability,
                connection
            )
            {
                CommandType = CommandType.StoredProcedure,
            };

            command.Parameters.AddWithValue("@ScheduleId", scheduleId);
            command.Parameters.AddWithValue("@TravelDate", travelDate.Date);
            command.Parameters.AddWithValue("@SeatNumbers", string.Join(",", seatNumbers));

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var seatDetails = new List<SeatLayoutDetail>();
            while (await reader.ReadAsync())
            {
                var seatDetail = new SeatLayoutDetail
                {
                    SeatNumber = reader.GetString("SeatNumber"),
                    SeatType = (SeatType)reader.GetInt32("SeatType"),
                    SeatPosition = (SeatPosition)reader.GetInt32("SeatPosition"),
                    PriceTier = (PriceTier)reader.GetInt32("PriceTier"),
                    BasePrice = reader.GetDecimal("BasePrice"),
                    BusTypeForPricing = (BusType)reader.GetInt32("BusType"),
                    AmenitiesForPricing = (BusAmenities)reader.GetInt32("Amenities"),
                    IsBooked = reader.GetInt32("IsBooked") == 1,
                };
                seatDetails.Add(seatDetail);
            }

            var bookedSeats = seatDetails.Where(s => s.IsBooked).ToList();
            if (bookedSeats.Any())
            {
                _logger.LogWarning(
                    MagicStrings.LogMessages.SeatValidationFailed,
                    MagicStrings.ErrorMessages.SeatsNotAvailable
                );
                return Result.Failure<List<SeatLayoutDetail>>(
                    Error.Failure(
                        "SeatValidation.Failed",
                        MagicStrings.ErrorMessages.SeatsNotAvailable
                    )
                );
            }

            _logger.LogInformation(MagicStrings.LogMessages.SeatValidationCompleted);
            return Result.Success(seatDetails);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.SeatValidationFailed, ex.Message);
            return Result.Failure<List<SeatLayoutDetail>>(
                Error.Failure("SeatValidation.Failed", MagicStrings.ErrorMessages.DatabaseError)
            );
        }
    }

    public async Task<Result<BookingEntity>> BookSeatsAsync(
        int scheduleId,
        int customerId,
        DateTime travelDate,
        List<string> seatNumbers,
        List<(string Name, int Age, Gender Gender)> passengers,
        decimal totalAmount,
        int boardingStopId,
        int droppingStopId
    )
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var pnr = Guid.NewGuid().ToString()[..8].ToUpper();

            var booking = new BookingEntity
            {
                PNRNo = pnr,
                CustomerId = customerId,
                TotalSeats = seatNumbers.Count,
                TotalAmount = totalAmount,
                TravelDate = travelDate,
                Status = BookingStatus.Pending,
                BookedAt = DateTime.UtcNow,
                IsActive = true,
                CreatedBy = "System",
                CreatedOn = DateTime.UtcNow,
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            var bookingSegment = new BookingSegment
            {
                BookingId = booking.BookingId,
                ScheduleId = scheduleId,
                SeatsBooked = seatNumbers.Count,
                SegmentAmount = totalAmount,
                SegmentOrder = 1,
                BoardingStopId = boardingStopId,
                DroppingStopId = droppingStopId,
                CreatedBy = "System",
                CreatedOn = DateTime.UtcNow,
            };

            _context.BookingSegments.Add(bookingSegment);
            await _context.SaveChangesAsync();

            var seatValidationResult = await ValidateSeatsAndStopsAsync(
                scheduleId,
                travelDate,
                seatNumbers,
                boardingStopId,
                droppingStopId
            );
            if (seatValidationResult.IsFailure)
            {
                await transaction.RollbackAsync();
                return Result.Failure<BookingEntity>(seatValidationResult.Error);
            }

            for (int i = 0; i < seatNumbers.Count; i++)
            {
                var seat = seatValidationResult.Value.First(s => s.SeatNumber == seatNumbers[i]);
                var passenger = passengers[i];

                var bookedSeat = new BookedSeat
                {
                    TravelDate = travelDate,
                    SeatNumber = seat.SeatNumber,
                    SeatType = seat.SeatType,
                    SeatPosition = seat.SeatPosition,
                    PassengerName = passenger.Name,
                    PassengerAge = passenger.Age,
                    PassengerGender = passenger.Gender,
                    BookingId = booking.BookingId,
                    BookingSegmentId = bookingSegment.BookingSegmentId,
                    CreatedBy = "System",
                    CreatedOn = DateTime.UtcNow,
                };

                _context.BookedSeats.Add(bookedSeat);
            }

            var schedule = await _context.BusSchedules.FindAsync(scheduleId);
            if (schedule != null)
            {
                schedule.AvailableSeats -= seatNumbers.Count;
                schedule.UpdatedBy = "System";
                schedule.UpdatedOn = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Result.Success(booking);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, MagicStrings.LogMessages.BookingFailed, ex.Message);
            return Result.Failure<BookingEntity>(
                Error.Failure("Booking.Failed", MagicStrings.ErrorMessages.BookingFailed)
            );
        }
    }

    public async Task<Result<(string BusName, string Route)>> GetBusInfoAsync(int scheduleId)
    {
        try
        {
            var busSchedule = await _context
                .BusSchedules.Include(bs => bs.Bus)
                .Include(bs => bs.Route)
                .FirstOrDefaultAsync(bs => bs.ScheduleId == scheduleId);

            if (busSchedule == null)
                return Result.Failure<(string, string)>(
                    Error.NotFound("Schedule.NotFound", "Schedule not found")
                );

            return Result.Success(
                (
                    busSchedule.Bus.BusName,
                    $"{busSchedule.Route.Source} to {busSchedule.Route.Destination}"
                )
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get bus info: {Message}", ex.Message);
            return Result.Failure<(string, string)>(
                Error.Failure("BusInfo.Failed", "Failed to get bus info")
            );
        }
    }

    public async Task<Result<List<RouteStopEntity>>> GetRouteStopsAsync(int scheduleId)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.RouteStopsStarted, scheduleId);

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                MagicStrings.StoredProcedures.GetRouteStops,
                connection
            )
            {
                CommandType = CommandType.StoredProcedure,
            };

            command.Parameters.AddWithValue("@ScheduleId", scheduleId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var routeStops = new List<RouteStopEntity>();
            while (await reader.ReadAsync())
            {
                var routeStop = new RouteStopEntity
                {
                    RouteStopId = reader.GetInt32("RouteStopId"),
                    StopId = reader.GetInt32("StopId"),
                    OrderNumber = reader.GetInt32("OrderNumber"),
                    ArrivalTime = reader.IsDBNull(reader.GetOrdinal("ArrivalTime"))
                        ? null
                        : reader.GetTimeSpan(reader.GetOrdinal("ArrivalTime")),
                    DepartureTime = reader.IsDBNull(reader.GetOrdinal("DepartureTime"))
                        ? null
                        : reader.GetTimeSpan(reader.GetOrdinal("DepartureTime")),
                    Stop = new StopEntity
                    {
                        StopId = reader.GetInt32("StopId"),
                        Name = reader.GetString("StopName"),
                        Landmark = reader.IsDBNull("Landmark")
                            ? null
                            : reader.GetString("Landmark"),
                    },
                };
                routeStops.Add(routeStop);
            }

            _logger.LogInformation(MagicStrings.LogMessages.RouteStopsCompleted, routeStops.Count);
            return Result.Success(routeStops);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.RouteStopsFailed, ex.Message);
            return Result.Failure<List<RouteStopEntity>>(
                Error.Failure("RouteStops.Failed", MagicStrings.ErrorMessages.DatabaseError)
            );
        }
    }

    public async Task<Result<List<SeatLayoutDetail>>> ValidateSeatsAndStopsAsync(
        int scheduleId,
        DateTime travelDate,
        List<string> seatNumbers,
        int boardingStopId,
        int droppingStopId
    )
    {
        try
        {
            _logger.LogInformation(
                MagicStrings.LogMessages.SeatValidationStarted,
                seatNumbers.Count
            );

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                MagicStrings.StoredProcedures.ValidateSeatsAndStops,
                connection
            )
            {
                CommandType = CommandType.StoredProcedure,
            };

            command.Parameters.AddWithValue("@ScheduleId", scheduleId);
            command.Parameters.AddWithValue("@TravelDate", travelDate.Date);
            command.Parameters.AddWithValue("@SeatNumbers", string.Join(",", seatNumbers));
            command.Parameters.AddWithValue("@BoardingStopId", boardingStopId);
            command.Parameters.AddWithValue("@DroppingStopId", droppingStopId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                if (reader.FieldCount == 1 && reader.GetName(0) == "ValidationResult")
                {
                    var validationResult = reader.GetString("ValidationResult");
                    var errorMessage = validationResult switch
                    {
                        "INVALID_STOPS" => MagicStrings.ErrorMessages.InvalidBoardingStop,
                        "INVALID_STOP_ORDER" => MagicStrings.ErrorMessages.InvalidStopOrder,
                        _ => MagicStrings.ErrorMessages.SeatsNotAvailable,
                    };

                    _logger.LogWarning(MagicStrings.LogMessages.SeatValidationFailed, errorMessage);
                    return Result.Failure<List<SeatLayoutDetail>>(
                        Error.Failure("SeatValidation.Failed", errorMessage)
                    );
                }
            }

            var seatDetails = new List<SeatLayoutDetail>();
            do
            {
                var seatDetail = new SeatLayoutDetail
                {
                    SeatNumber = reader.GetString("SeatNumber"),
                    SeatType = (SeatType)reader.GetInt32("SeatType"),
                    SeatPosition = (SeatPosition)reader.GetInt32("SeatPosition"),
                    PriceTier = (PriceTier)reader.GetInt32("PriceTier"),
                    BasePrice = reader.GetDecimal("BasePrice"),
                    BusTypeForPricing = (BusType)reader.GetInt32("BusType"),
                    AmenitiesForPricing = (BusAmenities)reader.GetInt32("Amenities"),
                    IsBooked = reader.GetInt32("IsBooked") == 1,
                };
                seatDetails.Add(seatDetail);
            } while (await reader.ReadAsync());

            var bookedSeats = seatDetails.Where(s => s.IsBooked).ToList();
            if (bookedSeats.Any())
            {
                _logger.LogWarning(
                    MagicStrings.LogMessages.SeatValidationFailed,
                    MagicStrings.ErrorMessages.SeatsNotAvailable
                );
                return Result.Failure<List<SeatLayoutDetail>>(
                    Error.Failure(
                        "SeatValidation.Failed",
                        MagicStrings.ErrorMessages.SeatsNotAvailable
                    )
                );
            }

            _logger.LogInformation(MagicStrings.LogMessages.SeatValidationCompleted);
            return Result.Success(seatDetails);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.SeatValidationFailed, ex.Message);
            return Result.Failure<List<SeatLayoutDetail>>(
                Error.Failure("SeatValidation.Failed", MagicStrings.ErrorMessages.DatabaseError)
            );
        }
    }

    public async Task<Result<string>> ConfirmBookingAsync(
        int bookingId,
        string paymentReferenceId,
        bool isPaymentSuccessful
    )
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.BookingConfirmationStarted, bookingId);

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
                            MagicStrings.LogMessages.BookingConfirmationCompleted,
                            bookingId
                        );
                        return Result.Success("Booking confirmed successfully");

                    case "BOOKING_NOT_FOUND":
                        _logger.LogWarning(
                            MagicStrings.LogMessages.BookingConfirmationFailed,
                            MagicStrings.ErrorMessages.BookingNotFound
                        );
                        return Result.Failure<string>(
                            Error.NotFound(
                                "Booking.NotFound",
                                MagicStrings.ErrorMessages.BookingNotFound
                            )
                        );

                    case "ALREADY_CONFIRMED":
                        _logger.LogWarning(
                            MagicStrings.LogMessages.BookingConfirmationFailed,
                            MagicStrings.ErrorMessages.BookingAlreadyConfirmed
                        );
                        return Result.Failure<string>(
                            Error.Failure(
                                "Booking.AlreadyConfirmed",
                                MagicStrings.ErrorMessages.BookingAlreadyConfirmed
                            )
                        );

                    case "BOOKING_EXPIRED":
                        _logger.LogWarning(
                            MagicStrings.LogMessages.BookingConfirmationFailed,
                            MagicStrings.ErrorMessages.BookingExpired
                        );
                        return Result.Failure<string>(
                            Error.Failure(
                                "Booking.Expired",
                                MagicStrings.ErrorMessages.BookingExpired
                            )
                        );

                    case "PAYMENT_FAILED":
                        _logger.LogInformation(
                            "Booking cancelled due to payment failure for BookingId: {BookingId}",
                            bookingId
                        );
                        return Result.Success("Booking cancelled due to payment failure");

                    default:
                        _logger.LogError(
                            MagicStrings.LogMessages.BookingConfirmationFailed,
                            "Unknown result: " + result
                        );
                        return Result.Failure<string>(
                            Error.Failure(
                                "Booking.UnknownResult",
                                MagicStrings.ErrorMessages.UnexpectedError
                            )
                        );
                }
            }

            _logger.LogError(
                MagicStrings.LogMessages.BookingConfirmationFailed,
                "No result returned from stored procedure"
            );
            return Result.Failure<string>(
                Error.Failure("Booking.NoResult", MagicStrings.ErrorMessages.DatabaseError)
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.BookingConfirmationFailed, ex.Message);
            return Result.Failure<string>(
                Error.Failure(
                    "Booking.ConfirmationFailed",
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result<int>> ExpirePendingBookingsAsync()
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.BookingExpiryStarted);

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                MagicStrings.StoredProcedures.ExpirePendingBookings,
                connection
            )
            {
                CommandType = CommandType.StoredProcedure,
            };

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                var expiredCount = reader.GetInt32("ExpiredCount");
                if (expiredCount > 0)
                {
                    _logger.LogInformation(
                        MagicStrings.LogMessages.BookingExpiryCompleted,
                        expiredCount
                    );
                }
                return Result.Success(expiredCount);
            }

            return Result.Success(0);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.BookingExpiryFailed, ex.Message);
            return Result.Failure<int>(
                Error.Failure("BookingExpiry.Failed", MagicStrings.ErrorMessages.DatabaseError)
            );
        }
    }

    public async Task<Result<List<BusSchedule>>> SearchBusesFilteredAsync(
        string source,
        string destination,
        DateTime travelDate,
        List<int>? busTypes,
        List<int>? amenities,
        TimeSpan? departureTimeFrom,
        TimeSpan? departureTimeTo,
        decimal? minPrice,
        decimal? maxPrice,
        string? sortBy
    )
    {
        try
        {
            _logger.LogInformation(
                MagicStrings.LogMessages.FilteredBusSearchStarted,
                source,
                destination,
                travelDate
            );

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                MagicStrings.StoredProcedures.SearchBusesFiltered,
                connection
            )
            {
                CommandType = CommandType.StoredProcedure,
            };

            command.Parameters.AddWithValue("@Source", source);
            command.Parameters.AddWithValue("@Destination", destination);
            command.Parameters.AddWithValue("@TravelDate", travelDate.Date);
            command.Parameters.AddWithValue(
                "@BusTypes",
                busTypes != null ? string.Join(",", busTypes) : (object)DBNull.Value
            );
            command.Parameters.AddWithValue(
                "@Amenities",
                amenities != null ? string.Join(",", amenities) : (object)DBNull.Value
            );
            command.Parameters.AddWithValue(
                "@DepartureTimeFrom",
                departureTimeFrom ?? (object)DBNull.Value
            );
            command.Parameters.AddWithValue(
                "@DepartureTimeTo",
                departureTimeTo ?? (object)DBNull.Value
            );
            command.Parameters.AddWithValue("@MinPrice", minPrice ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@MaxPrice", maxPrice ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@SortBy", sortBy ?? "time_asc");

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var busSchedules = new List<BusSchedule>();
            while (await reader.ReadAsync())
            {
                var busSchedule = new BusSchedule
                {
                    ScheduleId = reader.GetInt32("ScheduleId"),
                    TravelDate = reader.GetDateTime("TravelDate"),
                    DepartureTime = reader.GetTimeSpan(reader.GetOrdinal("DepartureTime")),
                    ArrivalTime = reader.GetTimeSpan(reader.GetOrdinal("ArrivalTime")),
                    AvailableSeats = reader.GetInt32("AvailableSeats"),
                    Bus = new Bus
                    {
                        BusId = reader.GetInt32("BusId"),
                        BusName = reader.GetString("BusName"),
                        BusType = (BusType)reader.GetInt32("BusType"),
                        TotalSeats = reader.GetInt32("TotalSeats"),
                        Amenities = (BusAmenities)reader.GetInt32("Amenities"),
                        Vendor = new VendorEntity { AgencyName = reader.GetString("VendorName") },
                    },
                    Route = new RouteEntity
                    {
                        Source = reader.GetString("Source"),
                        Destination = reader.GetString("Destination"),
                        BasePrice = reader.GetDecimal("BasePrice"),
                    },
                };
                busSchedules.Add(busSchedule);
            }

            _logger.LogInformation(
                MagicStrings.LogMessages.FilteredBusSearchCompleted,
                busSchedules.Count
            );
            return Result.Success(busSchedules);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.FilteredBusSearchFailed, ex.Message);
            return Result.Failure<List<BusSchedule>>(
                Error.Failure("FilteredBusSearch.Failed", MagicStrings.ErrorMessages.DatabaseError)
            );
        }
    }

    public async Task<Result<BookingEntity>> GetBookingDetailsAsync(int bookingId)
    {
        try
        {
            var booking = await _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Segments)
                    .ThenInclude(bs => bs.Schedule)
                        .ThenInclude(s => s.Bus)
                .Include(b => b.Segments)
                    .ThenInclude(bs => bs.Schedule)
                        .ThenInclude(s => s.Route)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);

            if (booking == null)
            {
                return Result.Failure<BookingEntity>(
                    Error.NotFound("Booking.NotFound", MagicStrings.ErrorMessages.BookingNotFound)
                );
            }

            return Result.Success(booking);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get booking details for BookingId: {BookingId}", bookingId);
            return Result.Failure<BookingEntity>(
                Error.Failure("GetBookingDetails.Failed", MagicStrings.ErrorMessages.DatabaseError)
            );
        }
    }
}
