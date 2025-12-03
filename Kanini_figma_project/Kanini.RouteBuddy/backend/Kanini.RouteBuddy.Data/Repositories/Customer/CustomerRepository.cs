using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Kanini.RouteBuddy.Common;
using Kanini.RouteBuddy.Data.DatabaseContext;
using Kanini.RouteBuddy.Data.Infrastructure;
using Kanini.RouteBuddy.Data.Models;
using Kanini.RouteBuddy.Domain.Enums;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using BookingEntity = Kanini.RouteBuddy.Domain.Entities.Booking;
using CustomerEntity = Kanini.RouteBuddy.Domain.Entities.Customer;

namespace Kanini.RouteBuddy.Data.Repositories.Customer
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly RouteBuddyDatabaseContext _context;
        private readonly IDbReader _dbReader;
        private readonly ILogger<CustomerRepository> _logger;
        private readonly string _connectionString;

        public CustomerRepository(
            RouteBuddyDatabaseContext context,
            IDbReader dbReader,
            ILogger<CustomerRepository> logger,
            IConfiguration configuration
        )
        {
            _context = context;
            _dbReader = dbReader;
            _logger = logger;
            _connectionString = configuration.GetConnectionString("DatabaseConnectionString")!;
        }

        // ✅ EF Core Write
        public async Task AddCustomerAsync(CustomerEntity customer)
        {
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
        }

        // ✅ EF Core Write
        public async Task CreateCustomerAsync(CustomerEntity customer)
        {
            await AddCustomerAsync(customer);
        }

        // ✅ ADO.NET Read
        public async Task<CustomerEntity?> GetCustomerByEmailAsync(string email)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@Email", SqlDbType.NVarChar, 150) { Value = email },
                };

                var table = await _dbReader.ExecuteStoredProcedureAsync(
                    "sp_GetCustomerByEmail",
                    parameters
                );
                if (table.Rows.Count == 0)
                    return null;

                var row = table.Rows[0];
                return new CustomerEntity
                {
                    CustomerId = Convert.ToInt32(row["CustomerId"]),
                    FirstName = Convert.ToString(row["FirstName"]) ?? "",
                    MiddleName = row["MiddleName"] as string,
                    LastName = Convert.ToString(row["LastName"]) ?? "",
                    DateOfBirth = Convert.ToDateTime(row["DateOfBirth"]),
                    Gender = (Kanini.RouteBuddy.Domain.Enums.Gender)Convert.ToInt32(row["Gender"]),
                    UserId = Convert.ToInt32(row["UserId"]),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching customer by email {Email}", email);
                throw;
            }
        }

        // ADO.NET Read with stored procedure
        public async Task<IEnumerable<CustomerEntity>> GetAllCustomersAsync()
        {
            _logger.LogInformation("GetAllCustomersAsync started");

            try
            {
                _logger.LogInformation("Creating connection");
                using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand(MagicStrings.StoredProcedures.GetAllCustomersWithSummary, connection)
                {
                    CommandType = CommandType.StoredProcedure,
                };

                _logger.LogInformation("Opening connection");
                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();

                _logger.LogInformation("Reading customer data");
                var customers = new List<CustomerEntity>();
                while (await reader.ReadAsync())
                {
                    customers.Add(
                        new CustomerEntity
                        {
                            CustomerId = reader.GetInt32("CustomerId"),
                            FirstName = reader.GetString("FullName").Split(' ')[0],
                            LastName = reader.GetString("FullName").Split(' ').Last(),
                            Gender = (Kanini.RouteBuddy.Domain.Enums.Gender)
                                reader.GetInt32("Gender"),
                            DateOfBirth = DateTime.Today.AddYears(-reader.GetInt32("Age")),
                            IsActive = reader.GetBoolean("IsActive"),
                        }
                    );
                }

                _logger.LogInformation("Data processing completed");
                _logger.LogInformation("GetAllCustomersAsync completed successfully");
                return customers;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAllCustomersAsync failed");
                return Enumerable.Empty<CustomerEntity>();
            }
        }

        public async Task<IEnumerable<CustomerEntity>> FilterCustomersAsync(
            string? searchName,
            bool? isActive,
            int? minAge,
            int? maxAge
        )
        {
            _logger.LogInformation("FilterCustomersAsync started");

            try
            {
                _logger.LogInformation("Creating connection with parameters");
                using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand(MagicStrings.StoredProcedures.FilterCustomersWithSummary, connection)
                {
                    CommandType = CommandType.StoredProcedure,
                };

                command.Parameters.Add(
                    new SqlParameter("@SearchName", searchName ?? (object)DBNull.Value)
                );
                command.Parameters.Add(
                    new SqlParameter("@IsActive", isActive ?? (object)DBNull.Value)
                );
                command.Parameters.Add(new SqlParameter("@MinAge", minAge ?? (object)DBNull.Value));
                command.Parameters.Add(new SqlParameter("@MaxAge", maxAge ?? (object)DBNull.Value));

                _logger.LogInformation("Opening connection");
                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();

                _logger.LogInformation("Reading filtered data");
                var customers = new List<CustomerEntity>();
                while (await reader.ReadAsync())
                {
                    customers.Add(
                        new CustomerEntity
                        {
                            CustomerId = reader.GetInt32("CustomerId"),
                            FirstName = reader.GetString("FullName").Split(' ')[0],
                            LastName = reader.GetString("FullName").Split(' ').Last(),
                            Gender = (Kanini.RouteBuddy.Domain.Enums.Gender)
                                reader.GetInt32("Gender"),
                            DateOfBirth = DateTime.Today.AddYears(-reader.GetInt32("Age")),
                            IsActive = reader.GetBoolean("IsActive"),
                        }
                    );
                }

                _logger.LogInformation("Filter processing completed");
                _logger.LogInformation("FilterCustomersAsync completed successfully");
                return customers;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "FilterCustomersAsync failed");
                return Enumerable.Empty<CustomerEntity>();
            }
        }

        // ✅ ADO.NET Read
        public async Task<CustomerEntity?> GetCustomerByIdAsync(int customerId)
        {
            _logger.LogInformation("GetCustomerByIdAsync started for CustomerId: {CustomerId}", customerId);

            try
            {
                using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand(MagicStrings.StoredProcedures.GetCustomerById, connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.Add(new SqlParameter("@CustomerId", customerId));
                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    var customer = new CustomerEntity
                    {
                        CustomerId = reader.GetInt32("CustomerId"),
                        FirstName = reader.GetString("FirstName"),
                        MiddleName = reader.IsDBNull("MiddleName") ? null : reader.GetString("MiddleName"),
                        LastName = reader.GetString("LastName"),
                        DateOfBirth = reader.GetDateTime("DateOfBirth"),
                        Gender = (Kanini.RouteBuddy.Domain.Enums.Gender)reader.GetInt32("Gender"),
                        IsActive = reader.GetBoolean("IsActive"),
                        CreatedOn = reader.GetDateTime("CreatedOn"),
                        UpdatedOn = reader.GetDateTime("UpdatedOn"),
                        CreatedBy = reader.IsDBNull("CreatedBy") ? null : reader.GetString("CreatedBy"),
                        UpdatedBy = reader.IsDBNull("UpdatedBy") ? null : reader.GetString("UpdatedBy"),
                        UserId = reader.GetInt32("UserId")
                    };

                    _logger.LogInformation("GetCustomerByIdAsync completed successfully for CustomerId: {CustomerId}", customerId);
                    return customer;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetCustomerByIdAsync failed for CustomerId: {CustomerId}: {Error}", customerId, ex.Message);
                throw;
            }
        }

        // ✅ ADO.NET Write (using stored procedure)
        public async Task<bool> SoftDeleteCustomerAsync(int customerId)
        {
            _logger.LogInformation("SoftDeleteCustomerAsync started for CustomerId: {CustomerId}", customerId);

            try
            {
                using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand(MagicStrings.StoredProcedures.SoftDeleteCustomer, connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.Add(new SqlParameter("@CustomerId", customerId));
                await connection.OpenAsync();
                var result = await command.ExecuteNonQueryAsync();
                
                _logger.LogInformation("SoftDeleteCustomerAsync completed successfully for CustomerId: {CustomerId}", customerId);
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SoftDeleteCustomerAsync failed for CustomerId: {CustomerId}: {Error}", customerId, ex.Message);
                return false;
            }
        }

        // ✅ ADO.NET Read
        public async Task<CustomerEntity?> GetCustomerProfileByUserIdAsync(int userId)
        {
            _logger.LogInformation(
                "Customer profile retrieval by UserId started: {UserId}",
                userId
            );

            try
            {
                using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand(
                    MagicStrings.StoredProcedures.GetCustomerProfileByUserId,
                    connection
                )
                {
                    CommandType = CommandType.StoredProcedure,
                };

                command.Parameters.Add(new SqlParameter("@UserId", userId));

                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    var customer = new CustomerEntity
                    {
                        CustomerId = reader.GetInt32("CustomerId"),
                        FirstName = reader.GetString("FirstName"),
                        MiddleName = reader.IsDBNull("MiddleName")
                            ? null
                            : reader.GetString("MiddleName"),
                        LastName = reader.GetString("LastName"),
                        DateOfBirth = reader.GetDateTime("DateOfBirth"),
                        Gender = (Kanini.RouteBuddy.Domain.Enums.Gender)reader.GetInt32("Gender"),
                        ProfilePicture = reader.IsDBNull("ProfilePicture")
                            ? null
                            : (byte[])reader["ProfilePicture"],
                        IsActive = reader.GetBoolean("IsActive"),
                        CreatedOn = reader.IsDBNull("CreatedOn")
                            ? DateTime.UtcNow
                            : reader.GetDateTime("CreatedOn"),
                        UpdatedOn = reader.IsDBNull("UpdatedOn")
                            ? DateTime.UtcNow
                            : reader.GetDateTime("UpdatedOn"),
                        User = new Kanini.RouteBuddy.Domain.Entities.User
                        {
                            Email = reader.GetString("Email"),
                            Phone = reader.GetString("Phone"),
                        },
                    };

                    _logger.LogInformation(
                        "Customer profile retrieval by UserId completed: {UserId}",
                        userId
                    );
                    return customer;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Customer profile retrieval by UserId failed: {UserId}: {Error}",
                    userId,
                    ex.Message
                );
                throw;
            }
        }

        // ✅ ADO.NET Read
        public async Task<CustomerEntity?> GetCustomerProfileByIdAsync(int customerId)
        {
            _logger.LogInformation(
                "Customer profile retrieval started for CustomerId: {CustomerId}",
                customerId
            );

            try
            {
                using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand(
                    MagicStrings.StoredProcedures.GetCustomerProfileById,
                    connection
                )
                {
                    CommandType = CommandType.StoredProcedure,
                };

                command.Parameters.Add(new SqlParameter("@CustomerId", customerId));

                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    var customer = new CustomerEntity
                    {
                        CustomerId = reader.GetInt32("CustomerId"),
                        FirstName = reader.GetString("FirstName"),
                        MiddleName = reader.IsDBNull("MiddleName")
                            ? null
                            : reader.GetString("MiddleName"),
                        LastName = reader.GetString("LastName"),
                        DateOfBirth = reader.GetDateTime("DateOfBirth"),
                        Gender = (Kanini.RouteBuddy.Domain.Enums.Gender)reader.GetInt32("Gender"),
                        IsActive = reader.GetBoolean("IsActive"),
                        CreatedOn = reader.GetDateTime("CreatedOn"),
                        UpdatedOn = reader.GetDateTime("UpdatedOn"),
                        User = new Kanini.RouteBuddy.Domain.Entities.User
                        {
                            Email = reader.GetString("Email"),
                            Phone = reader.GetString("Phone"),
                        },
                    };

                    _logger.LogInformation(
                        "Customer profile retrieved successfully for CustomerId: {CustomerId}",
                        customerId
                    );
                    return customer;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Customer profile retrieval failed for CustomerId: {CustomerId}: {Error}",
                    customerId,
                    ex.Message
                );
                throw;
            }
        }

        // ✅ EF Core Write
        public async Task<bool> UpdateCustomerProfilePictureAsync(
            int customerId,
            byte[] profilePicture
        )
        {
            _logger.LogInformation(
                "Customer profile picture update started for CustomerId: {CustomerId}",
                customerId
            );

            try
            {
                var customer = await _context.Customers.FirstOrDefaultAsync(c =>
                    c.CustomerId == customerId && c.IsActive
                );

                if (customer == null)
                {
                    _logger.LogWarning(
                        "Customer not found for CustomerId: {CustomerId}",
                        customerId
                    );
                    return false;
                }

                customer.ProfilePicture = profilePicture;
                customer.UpdatedOn = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Customer profile picture updated successfully for CustomerId: {CustomerId}",
                    customerId
                );
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Customer profile picture update failed for CustomerId: {CustomerId}: {Error}",
                    customerId,
                    ex.Message
                );
                return false;
            }
        }

        // ✅ EF Core Write
        public async Task<bool> UpdateCustomerProfileAsync(
            int customerId,
            string firstName,
            string? middleName,
            string lastName,
            DateTime dateOfBirth,
            int gender,
            string phone
        )
        {
            _logger.LogInformation(
                "Customer profile update started for CustomerId: {CustomerId}",
                customerId
            );

            try
            {
                // Find customer and user entities
                var customer = await _context
                    .Customers.Include(c => c.User)
                    .FirstOrDefaultAsync(c => c.CustomerId == customerId && c.IsActive);

                if (customer == null)
                {
                    _logger.LogWarning(
                        "Customer not found for CustomerId: {CustomerId}",
                        customerId
                    );
                    return false;
                }

                // Update customer entity
                customer.FirstName = firstName;
                customer.MiddleName = middleName;
                customer.LastName = lastName;
                customer.DateOfBirth = dateOfBirth;
                customer.Gender = (Kanini.RouteBuddy.Domain.Enums.Gender)gender;
                customer.UpdatedOn = DateTime.UtcNow;
                customer.UpdatedBy = customer.User?.UserId.ToString() ?? "System"; // Set UpdatedBy to the user who is updating

                // Update user phone
                if (customer.User != null)
                {
                    customer.User.Phone = phone;
                    customer.User.UpdatedOn = DateTime.UtcNow;
                    customer.User.UpdatedBy = customer.User.UserId.ToString(); // Set UpdatedBy for user entity
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Customer profile updated successfully for CustomerId: {CustomerId}",
                    customerId
                );
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Customer profile update failed for CustomerId: {CustomerId}: {Error}",
                    customerId,
                    ex.Message
                );
                return false;
            }
        }

        // ✅ ADO.NET Read
        public async Task<IEnumerable<BookingEntity>> GetCustomerBookingsAsync(
            int customerId,
            BookingStatus? status,
            DateTime? fromDate,
            DateTime? toDate
        )
        {
            _logger.LogInformation("Getting bookings for customer: {CustomerId}", customerId);

            try
            {
                using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand(MagicStrings.StoredProcedures.GetCustomerBookings, connection)
                {
                    CommandType = CommandType.StoredProcedure,
                };

                command.Parameters.Add(new SqlParameter("@CustomerId", customerId));
                command.Parameters.Add(
                    new SqlParameter(
                        "@Status",
                        status.HasValue ? (int)status.Value : (object)DBNull.Value
                    )
                );
                command.Parameters.Add(
                    new SqlParameter("@FromDate", fromDate ?? (object)DBNull.Value)
                );
                command.Parameters.Add(new SqlParameter("@ToDate", toDate ?? (object)DBNull.Value));

                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();

                var bookings = new List<BookingEntity>();
                while (await reader.ReadAsync())
                {
                    var booking = new BookingEntity
                    {
                        BookingId = reader.GetInt32("BookingId"),
                        PNRNo = reader.GetString("PNRNo"),
                        TotalSeats = reader.GetInt32("TotalSeats"),
                        TotalAmount = reader.GetDecimal("TotalAmount"),
                        TravelDate = reader.GetDateTime("TravelDate"),
                        Status = (BookingStatus)reader.GetInt32("Status"),
                        BookedAt = reader.GetDateTime("BookedAt"),
                        CustomerId = customerId,
                        IsActive = true,
                        CreatedOn = reader.GetDateTime("BookedAt"),
                        CreatedBy = "System",
                    };
                    bookings.Add(booking);
                }

                _logger.LogInformation(
                    "Retrieved {Count} bookings for customer: {CustomerId}",
                    bookings.Count,
                    customerId
                );
                return bookings;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error getting bookings for customer: {CustomerId}",
                    customerId
                );
                return Enumerable.Empty<BookingEntity>();
            }
        }

        // ✅ ADO.NET Read - Get bookings with additional details
        public async Task<IEnumerable<BookingWithDetails>> GetCustomerBookingsWithDetailsAsync(
            int customerId,
            BookingStatus? status,
            DateTime? fromDate,
            DateTime? toDate
        )
        {
            _logger.LogInformation(
                "Getting bookings with details for customer: {CustomerId}",
                customerId
            );

            try
            {
                using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand(MagicStrings.StoredProcedures.GetCustomerBookings, connection)
                {
                    CommandType = CommandType.StoredProcedure,
                };

                command.Parameters.Add(new SqlParameter("@CustomerId", customerId));
                command.Parameters.Add(
                    new SqlParameter(
                        "@Status",
                        status.HasValue ? (int)status.Value : (object)DBNull.Value
                    )
                );
                command.Parameters.Add(
                    new SqlParameter("@FromDate", fromDate ?? (object)DBNull.Value)
                );
                command.Parameters.Add(new SqlParameter("@ToDate", toDate ?? (object)DBNull.Value));

                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();

                var bookingsWithDetails = new List<BookingWithDetails>();
                while (await reader.ReadAsync())
                {
                    var booking = new BookingEntity
                    {
                        BookingId = reader.GetInt32("BookingId"),
                        PNRNo = reader.GetString("PNRNo"),
                        TotalSeats = reader.GetInt32("TotalSeats"),
                        TotalAmount = reader.GetDecimal("TotalAmount"),
                        TravelDate = reader.GetDateTime("TravelDate"),
                        Status = (BookingStatus)reader.GetInt32("Status"),
                        BookedAt = reader.GetDateTime("BookedAt"),
                        CustomerId = customerId,
                        IsActive = true,
                        CreatedOn = reader.GetDateTime("BookedAt"),
                        CreatedBy = "System",
                    };

                    var bookingWithDetails = new BookingWithDetails
                    {
                        Booking = booking,
                        BusName = reader.GetString("BusName"),
                        Route = reader.GetString("Route"),
                        VendorName = reader.GetString("VendorName"),
                        DepartureTime = reader.GetTimeSpan(reader.GetOrdinal("DepartureTime")),
                        ArrivalTime = reader.GetTimeSpan(reader.GetOrdinal("ArrivalTime")),
                        PaymentMethod = (PaymentMethod)reader.GetInt32("PaymentMethod"),
                        IsPaymentCompleted = reader.GetInt32("IsPaymentCompleted") == 1,
                    };

                    bookingsWithDetails.Add(bookingWithDetails);
                }

                _logger.LogInformation(
                    "Retrieved {Count} bookings with details for customer: {CustomerId}",
                    bookingsWithDetails.Count,
                    customerId
                );
                return bookingsWithDetails;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error getting bookings with details for customer: {CustomerId}",
                    customerId
                );
                return Enumerable.Empty<BookingWithDetails>();
            }
        }
    }
}
