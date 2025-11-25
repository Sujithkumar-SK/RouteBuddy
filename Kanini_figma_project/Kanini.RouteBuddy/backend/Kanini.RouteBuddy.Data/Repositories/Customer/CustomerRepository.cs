using Kanini.RouteBuddy.Data.DatabaseContext;
using Kanini.RouteBuddy.Data.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Collections.Generic;
using CustomerEntity = Kanini.RouteBuddy.Domain.Entities.Customer;

namespace Kanini.RouteBuddy.Data.Repositories.Customer
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly RouteBuddyDatabaseContext _context;
        private readonly IDbReader _dbReader;
        private readonly ILogger<CustomerRepository> _logger;
        private readonly string _connectionString;

        public CustomerRepository(RouteBuddyDatabaseContext context, IDbReader dbReader, ILogger<CustomerRepository> logger, IConfiguration configuration)
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
                    new SqlParameter("@Email", SqlDbType.NVarChar, 150) { Value = email }
                };

                var table = await _dbReader.ExecuteStoredProcedureAsync("sp_GetCustomerByEmail", parameters);
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
                    UserId = Convert.ToInt32(row["UserId"])
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
                using var command = new SqlCommand("sp_GetAllCustomersWithSummary", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                _logger.LogInformation("Opening connection");
                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();

                _logger.LogInformation("Reading customer data");
                var customers = new List<CustomerEntity>();
                while (await reader.ReadAsync())
                {
                    customers.Add(new CustomerEntity
                    {
                        CustomerId = reader.GetInt32("CustomerId"),
                        FirstName = reader.GetString("FullName").Split(' ')[0],
                        LastName = reader.GetString("FullName").Split(' ').Last(),
                        Gender = (Kanini.RouteBuddy.Domain.Enums.Gender)reader.GetInt32("Gender"),
                        DateOfBirth = DateTime.Today.AddYears(-reader.GetInt32("Age")),
                        IsActive = reader.GetBoolean("IsActive")
                    });
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

        public async Task<IEnumerable<CustomerEntity>> FilterCustomersAsync(string? searchName, bool? isActive, int? minAge, int? maxAge)
        {
            _logger.LogInformation("FilterCustomersAsync started");

            try
            {
                _logger.LogInformation("Creating connection with parameters");
                using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand("sp_FilterCustomersWithSummary", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.Add(new SqlParameter("@SearchName", searchName ?? (object)DBNull.Value));
                command.Parameters.Add(new SqlParameter("@IsActive", isActive ?? (object)DBNull.Value));
                command.Parameters.Add(new SqlParameter("@MinAge", minAge ?? (object)DBNull.Value));
                command.Parameters.Add(new SqlParameter("@MaxAge", maxAge ?? (object)DBNull.Value));

                _logger.LogInformation("Opening connection");
                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();

                _logger.LogInformation("Reading filtered data");
                var customers = new List<CustomerEntity>();
                while (await reader.ReadAsync())
                {
                    customers.Add(new CustomerEntity
                    {
                        CustomerId = reader.GetInt32("CustomerId"),
                        FirstName = reader.GetString("FullName").Split(' ')[0],
                        LastName = reader.GetString("FullName").Split(' ').Last(),
                        Gender = (Kanini.RouteBuddy.Domain.Enums.Gender)reader.GetInt32("Gender"),
                        DateOfBirth = DateTime.Today.AddYears(-reader.GetInt32("Age")),
                        IsActive = reader.GetBoolean("IsActive")
                    });
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

        public async Task<CustomerEntity?> GetCustomerByIdAsync(int customerId)
        {
            _logger.LogInformation("GetCustomerByIdAsync started");

            try
            {
                _logger.LogInformation("Creating EF query for customer {CustomerId}", customerId);
                _logger.LogInformation("Including related entities");
                _logger.LogInformation("Executing query");
                var customer = await _context.Customers
                    .Include(c => c.Bookings)
                    .Include(c => c.Reviews)
                    .FirstOrDefaultAsync(c => c.CustomerId == customerId);

                _logger.LogInformation("Query execution completed");
                _logger.LogInformation("GetCustomerByIdAsync completed successfully");
                return customer;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetCustomerByIdAsync failed");
                return null;
            }
        }

        public async Task<bool> SoftDeleteCustomerAsync(int customerId)
        {
            _logger.LogInformation("SoftDeleteCustomerAsync started");

            try
            {
                _logger.LogInformation("Creating parameter for customer {CustomerId}", customerId);
                var parameter = new SqlParameter("@CustomerId", customerId);
                _logger.LogInformation("Preparing stored procedure call");
                _logger.LogInformation("Executing stored procedure");
                var result = await _context.Database.ExecuteSqlRawAsync("EXEC sp_SoftDeleteCustomer @CustomerId", parameter);
                _logger.LogInformation("Stored procedure execution completed");
                _logger.LogInformation("SoftDeleteCustomerAsync completed successfully");
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SoftDeleteCustomerAsync failed");
                return false;
            }
        }
    }
}