using Kanini.RouteBuddy.Data.DatabaseContext;
using Kanini.RouteBuddy.Domain.Enums;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;
using Entities = Kanini.RouteBuddy.Domain.Entities;

namespace Kanini.RouteBuddy.Data.Repositories.Booking
{
    public class BookingRepository : IBookingRepository
    {
        private readonly RouteBuddyDatabaseContext _context;
        private readonly string _connectionString;
        private readonly ILogger<BookingRepository> _logger;

        public BookingRepository(RouteBuddyDatabaseContext context, ILogger<BookingRepository> logger)
        {
            _context = context;
            _connectionString = _context.Database.GetConnectionString()!;
            _logger = logger;
        }

        public async Task<IEnumerable<Entities.Booking>> GetAllBookingsAsync()
        {
            _logger.LogInformation("GetAllBookingsAsync started");
            var bookings = new List<Entities.Booking>();
            
            try
            {
                _logger.LogInformation("Creating connection");
                using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand("sp_GetAllBookings", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                
                _logger.LogInformation("Opening connection");
                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();
                
                _logger.LogInformation("Reading data");
                while (await reader.ReadAsync())
                {
                    bookings.Add(new Entities.Booking
                    {
                        BookingId = reader.GetInt32("BookingId"),
                        PNRNo = reader.GetString("PNRNo"),
                        TotalSeats = reader.GetInt32("TotalSeats"),
                        TotalAmount = reader.GetDecimal("TotalAmount"),
                        TravelDate = reader.GetDateTime("TravelDate"),
                        Status = (BookingStatus)reader.GetInt32("Status"),
                        BookedAt = reader.GetDateTime("BookedAt"),
                        Customer = new Entities.Customer
                        {
                            FirstName = reader.GetString("CustomerName").Split(' ')[0],
                            LastName = reader.GetString("CustomerName").Contains(' ') ? reader.GetString("CustomerName").Split(' ')[1] : "",
                            User = new Entities.User
                            {
                                Email = reader.GetString("CustomerEmail"),
                                Phone = reader.GetString("CustomerPhone")
                            }
                        },
                        Payment = new Entities.Payment
                        {
                            PaymentStatus = reader.IsDBNull("PaymentStatus") ? PaymentStatus.Pending : (PaymentStatus)reader.GetInt32("PaymentStatus")
                        },
                        Segments = new List<Entities.BookingSegment>
                        {
                            new Entities.BookingSegment
                            {
                                Schedule = new Entities.BusSchedule
                                {
                                    Bus = new Entities.Bus
                                    {
                                        BusName = reader.IsDBNull("BusName") ? "" : reader.GetString("BusName")
                                    },
                                    Route = new Entities.Route
                                    {
                                        Source = reader.IsDBNull("Route") ? "" : reader.GetString("Route").Split('-')[0].Trim(),
                                        Destination = reader.IsDBNull("Route") ? "" : reader.GetString("Route").Contains('-') ? reader.GetString("Route").Split('-')[1].Trim() : ""
                                    }
                                }
                            }
                        }
                    });
                }
                
                _logger.LogInformation("Data processing completed");
                _logger.LogInformation("GetAllBookingsAsync completed successfully");
                return bookings;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "GetAllBookingsAsync failed");
                return Enumerable.Empty<Entities.Booking>();
            }
        }

        public async Task<IEnumerable<Entities.Booking>> FilterBookingsAsync(string? searchName, int? status, DateTime? fromDate, DateTime? toDate)
        {
            _logger.LogInformation("FilterBookingsAsync started");
            var bookings = new List<Entities.Booking>();
            
            try
            {
                _logger.LogInformation("Creating connection with parameters");
                using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand("sp_FilterBookings", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                
                command.Parameters.Add(new SqlParameter("@SearchName", searchName ?? (object)DBNull.Value));
                command.Parameters.Add(new SqlParameter("@Status", status ?? (object)DBNull.Value));
                command.Parameters.Add(new SqlParameter("@FromDate", fromDate ?? (object)DBNull.Value));
                command.Parameters.Add(new SqlParameter("@ToDate", toDate ?? (object)DBNull.Value));
                
                _logger.LogInformation("Opening connection");
                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();
                
                _logger.LogInformation("Reading filtered data");
                while (await reader.ReadAsync())
                {
                    bookings.Add(new Entities.Booking
                    {
                        BookingId = reader.GetInt32("BookingId"),
                        PNRNo = reader.GetString("PNRNo"),
                        TotalSeats = reader.GetInt32("TotalSeats"),
                        TotalAmount = reader.GetDecimal("TotalAmount"),
                        TravelDate = reader.GetDateTime("TravelDate"),
                        Status = (BookingStatus)reader.GetInt32("Status"),
                        BookedAt = reader.GetDateTime("BookedAt"),
                        Customer = new Entities.Customer
                        {
                            FirstName = reader.GetString("CustomerName").Split(' ')[0],
                            LastName = reader.GetString("CustomerName").Contains(' ') ? reader.GetString("CustomerName").Split(' ')[1] : "",
                            User = new Entities.User
                            {
                                Email = reader.GetString("CustomerEmail"),
                                Phone = reader.GetString("CustomerPhone")
                            }
                        },
                        Payment = new Entities.Payment
                        {
                            PaymentStatus = reader.IsDBNull("PaymentStatus") ? PaymentStatus.Pending : (PaymentStatus)reader.GetInt32("PaymentStatus")
                        },
                        Segments = new List<Entities.BookingSegment>
                        {
                            new Entities.BookingSegment
                            {
                                Schedule = new Entities.BusSchedule
                                {
                                    Bus = new Entities.Bus
                                    {
                                        BusName = reader.IsDBNull("BusName") ? "" : reader.GetString("BusName")
                                    },
                                    Route = new Entities.Route
                                    {
                                        Source = reader.IsDBNull("Route") ? "" : reader.GetString("Route").Split('-')[0].Trim(),
                                        Destination = reader.IsDBNull("Route") ? "" : reader.GetString("Route").Contains('-') ? reader.GetString("Route").Split('-')[1].Trim() : ""
                                    }
                                }
                            }
                        }
                    });
                }
                
                _logger.LogInformation("Filter processing completed");
                _logger.LogInformation("FilterBookingsAsync completed successfully");
                return bookings;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "FilterBookingsAsync failed");
                return Enumerable.Empty<Entities.Booking>();
            }
        }

        public async Task<Entities.Booking?> GetBookingByIdAsync(int bookingId)
        {
            _logger.LogInformation("GetBookingByIdAsync started");
            
            try
            {
                _logger.LogInformation("Creating connection for booking {BookingId}", bookingId);
                using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand("sp_GetBookingById", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                
                command.Parameters.AddWithValue("@BookingId", bookingId);
                
                _logger.LogInformation("Opening connection");
                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();
                
                _logger.LogInformation("Reading booking data");
                if (await reader.ReadAsync())
                {
                    _logger.LogInformation("Booking found, creating object");
                    var booking = new Entities.Booking
                    {
                        BookingId = reader.GetInt32("BookingId"),
                        PNRNo = reader.GetString("PNRNo"),
                        TotalSeats = reader.GetInt32("TotalSeats"),
                        TotalAmount = reader.GetDecimal("TotalAmount"),
                        TravelDate = reader.GetDateTime("TravelDate"),
                        Status = (BookingStatus)reader.GetInt32("Status"),
                        BookedAt = reader.GetDateTime("BookedAt"),
                        Customer = new Entities.Customer
                        {
                            FirstName = reader.GetString("CustomerName").Split(' ')[0],
                            LastName = reader.GetString("CustomerName").Contains(' ') ? reader.GetString("CustomerName").Split(' ')[1] : "",
                            User = new Entities.User
                            {
                                Email = reader.GetString("CustomerEmail"),
                                Phone = reader.GetString("CustomerPhone")
                            }
                        },
                        Payment = new Entities.Payment
                        {
                            PaymentStatus = reader.IsDBNull("PaymentStatus") ? PaymentStatus.Pending : (PaymentStatus)reader.GetInt32("PaymentStatus")
                        },
                        Segments = new List<Entities.BookingSegment>
                        {
                            new Entities.BookingSegment
                            {
                                Schedule = new Entities.BusSchedule
                                {
                                    Bus = new Entities.Bus
                                    {
                                        BusName = reader.IsDBNull("BusName") ? "" : reader.GetString("BusName")
                                    },
                                    Route = new Entities.Route
                                    {
                                        Source = reader.IsDBNull("Route") ? "" : reader.GetString("Route").Split('-')[0].Trim(),
                                        Destination = reader.IsDBNull("Route") ? "" : reader.GetString("Route").Contains('-') ? reader.GetString("Route").Split('-')[1].Trim() : ""
                                    }
                                }
                            }
                        }
                    };
                    
                    _logger.LogInformation("GetBookingByIdAsync completed successfully");
                    return booking;
                }
                
                _logger.LogInformation("Booking not found");
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "GetBookingByIdAsync failed");
            }
            
            return null;
        }

        public async Task<(int Pending, int Confirmed, int Cancelled, int Total, decimal Revenue)> GetBookingStatusSummaryAsync()
        {
            _logger.LogInformation("GetBookingStatusSummaryAsync started");
            
            try
            {
                _logger.LogInformation("Creating connection for summary");
                using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand("sp_GetBookingStatusSummary", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                
                _logger.LogInformation("Opening connection");
                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();
                
                _logger.LogInformation("Reading summary data");
                if (await reader.ReadAsync())
                {
                    _logger.LogInformation("Summary data found, creating result");
                    var result = (
                        reader.GetInt32("PendingBookings"),
                        reader.GetInt32("ConfirmedBookings"),
                        reader.GetInt32("CancelledBookings"),
                        reader.GetInt32("TotalBookings"),
                        reader.GetDecimal("TotalRevenue")
                    );
                    
                    _logger.LogInformation("GetBookingStatusSummaryAsync completed successfully");
                    return result;
                }
                
                _logger.LogInformation("No summary data found");
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "GetBookingStatusSummaryAsync failed");
            }
            
            return (0, 0, 0, 0, 0);
        }


    }
}