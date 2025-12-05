using System.Data;
using Kanini.RouteBuddy.Common;
using Kanini.RouteBuddy.Common.Errors;
using Kanini.RouteBuddy.Common.Services;
using Kanini.RouteBuddy.Common.Utility;
using Kanini.RouteBuddy.Data.DatabaseContext;
using Kanini.RouteBuddy.Domain.Enums;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using BusEntity = Kanini.RouteBuddy.Domain.Entities.Bus;

namespace Kanini.RouteBuddy.Data.Repositories.Buses;

public class BusRepository : IBusRepository
{
    private readonly RouteBuddyDatabaseContext _context;
    private readonly string _connectionString;
    private readonly ILogger<BusRepository> _logger;

    public BusRepository(
        RouteBuddyDatabaseContext context,
        IConfiguration configuration,
        ILogger<BusRepository> logger
    )
    {
        _context = context;
        _connectionString = configuration.GetConnectionString("DatabaseConnectionString")!;
        _logger = logger;
    }

    public async Task<Result<BusEntity>> CreateAsync(BusEntity bus)
    {
        try
        {
            BusFileLogger.LogInfo("Creating bus: {0}", bus.BusName);

            _context.Buses.Add(bus);
            await _context.SaveChangesAsync();

            BusFileLogger.LogInfo("Bus created successfully with ID: {0}", bus.BusId);
            return Result.Success(bus);
        }
        catch (Exception ex)
        {
            BusFileLogger.LogError("Error creating bus", ex);
            return Result.Failure<BusEntity>(
                Error.Failure("Bus.CreationFailed", BusMessages.ErrorMessages.DatabaseError)
            );
        }
    }

    public async Task<Result<BusEntity>> GetByIdAsync(int busId)
    {
        try
        {
            BusFileLogger.LogInfo("Getting bus by ID: {0}", busId);

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                "SELECT BusId, VendorId, BusName, RegistrationNo, BusType, TotalSeats, Amenities, Status, IsActive, DriverName, DriverContact, RegistrationPath, SeatLayoutTemplateId, CreatedBy, UpdatedBy, CreatedOn FROM Buses WHERE BusId = @BusId",
                connection
            );
            command.Parameters.AddWithValue("@BusId", busId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                var bus = new BusEntity
                {
                    BusId = reader.GetInt32("BusId"),
                    VendorId = reader.GetInt32("VendorId"),
                    BusName = reader.GetString("BusName"),
                    RegistrationNo = reader.GetString("RegistrationNo"),
                    BusType = (BusType)reader.GetInt32("BusType"),
                    TotalSeats = reader.GetInt32("TotalSeats"),
                    Amenities = (BusAmenities)reader.GetInt32("Amenities"),
                    Status = (BusStatus)reader.GetInt32("Status"),
                    IsActive = reader.GetBoolean("IsActive"),
                    DriverName = reader.IsDBNull("DriverName")
                        ? null!
                        : reader.GetString("DriverName"),
                    DriverContact = reader.IsDBNull("DriverContact")
                        ? null!
                        : reader.GetString("DriverContact"),
                    RegistrationPath = reader.IsDBNull("RegistrationPath")
                        ? string.Empty
                        : reader.GetString("RegistrationPath"),
                    SeatLayoutTemplateId = reader.IsDBNull("SeatLayoutTemplateId")
                        ? null
                        : reader.GetInt32("SeatLayoutTemplateId"),
                    CreatedBy = reader.GetString("CreatedBy"),
                    UpdatedBy = reader.IsDBNull("UpdatedBy") ? null : reader.GetString("UpdatedBy"),
                    CreatedOn = reader.GetDateTime("CreatedOn"),
                };

                BusFileLogger.LogInfo("Bus retrieved successfully: {0}", busId);
                return Result.Success(bus);
            }

            BusFileLogger.LogWarning("Bus not found: {0}", busId);
            return Result.Failure<BusEntity>(
                Error.NotFound("Bus.NotFound", BusMessages.ErrorMessages.BusNotFound)
            );
        }
        catch (Exception ex)
        {
            BusFileLogger.LogError("Error retrieving bus", ex);
            return Result.Failure<BusEntity>(
                Error.Failure("Bus.RetrievalFailed", BusMessages.ErrorMessages.DatabaseError)
            );
        }
    }

    public async Task<Result<List<BusEntity>>> GetByVendorIdAsync(
        int vendorId,
        int pageNumber,
        int pageSize,
        BusStatus? status = null,
        BusType? busType = null,
        string? search = null
    )
    {
        try
        {
            BusFileLogger.LogInfo(
                "Getting buses by vendor: {0}, page: {1}, size: {2}, search: {3}",
                vendorId,
                pageNumber,
                pageSize,
                search ?? "none"
            );

            var whereClause = "WHERE VendorId = @VendorId";

            if (status.HasValue)
                whereClause += " AND Status = @Status";
            if (busType.HasValue)
                whereClause += " AND BusType = @BusType";
            if (!string.IsNullOrWhiteSpace(search))
                whereClause +=
                    " AND (BusName LIKE @Search OR RegistrationNo LIKE @Search OR DriverName LIKE @Search)";

            var sql =
                $"SELECT BusId, VendorId, BusName, RegistrationNo, BusType, TotalSeats, Amenities, Status, IsActive, DriverName, DriverContact, SeatLayoutTemplateId, CreatedBy, UpdatedBy, CreatedOn FROM Buses {whereClause} ORDER BY CreatedOn DESC OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@VendorId", vendorId);
            command.Parameters.AddWithValue("@Offset", (pageNumber - 1) * pageSize);
            command.Parameters.AddWithValue("@PageSize", pageSize);

            if (status.HasValue)
                command.Parameters.AddWithValue("@Status", (int)status.Value);
            if (busType.HasValue)
                command.Parameters.AddWithValue("@BusType", (int)busType.Value);
            if (!string.IsNullOrWhiteSpace(search))
                command.Parameters.AddWithValue("@Search", $"%{search}%");

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var buses = new List<BusEntity>();
            while (await reader.ReadAsync())
            {
                buses.Add(
                    new BusEntity
                    {
                        BusId = reader.GetInt32("BusId"),
                        VendorId = reader.GetInt32("VendorId"),
                        BusName = reader.GetString("BusName"),
                        RegistrationNo = reader.GetString("RegistrationNo"),
                        BusType = (BusType)reader.GetInt32("BusType"),
                        TotalSeats = reader.GetInt32("TotalSeats"),
                        Amenities = (BusAmenities)reader.GetInt32("Amenities"),
                        Status = (BusStatus)reader.GetInt32("Status"),
                        IsActive = reader.GetBoolean("IsActive"),
                        DriverName = reader.IsDBNull("DriverName")
                            ? null!
                            : reader.GetString("DriverName"),
                        DriverContact = reader.IsDBNull("DriverContact")
                            ? null!
                            : reader.GetString("DriverContact"),
                        SeatLayoutTemplateId = reader.IsDBNull("SeatLayoutTemplateId")
                            ? null
                            : reader.GetInt32("SeatLayoutTemplateId"),
                        CreatedBy = reader.GetString("CreatedBy"),
                        UpdatedBy = reader.IsDBNull("UpdatedBy")
                            ? null
                            : reader.GetString("UpdatedBy"),
                        CreatedOn = reader.GetDateTime("CreatedOn"),
                    }
                );
            }

            BusFileLogger.LogInfo("Retrieved {0} buses", buses.Count);
            return Result.Success(buses);
        }
        catch (Exception ex)
        {
            BusFileLogger.LogError("Error getting buses by vendor", ex);
            return Result.Failure<List<BusEntity>>(
                Error.Failure("Bus.ListFailed", BusMessages.ErrorMessages.DatabaseError)
            );
        }
    }

    public async Task<Result<int>> GetCountByVendorIdAsync(
        int vendorId,
        BusStatus? status = null,
        BusType? busType = null,
        string? search = null
    )
    {
        try
        {
            var whereClause = "WHERE VendorId = @VendorId";

            if (status.HasValue)
                whereClause += " AND Status = @Status";
            if (busType.HasValue)
                whereClause += " AND BusType = @BusType";
            if (!string.IsNullOrWhiteSpace(search))
                whereClause +=
                    " AND (BusName LIKE @Search OR RegistrationNo LIKE @Search OR DriverName LIKE @Search)";

            var sql = $"SELECT COUNT(*) FROM Buses {whereClause}";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@VendorId", vendorId);

            if (status.HasValue)
                command.Parameters.AddWithValue("@Status", (int)status.Value);
            if (busType.HasValue)
                command.Parameters.AddWithValue("@BusType", (int)busType.Value);
            if (!string.IsNullOrWhiteSpace(search))
                command.Parameters.AddWithValue("@Search", $"%{search}%");

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            var count = result != null ? (int)result : 0;
            return Result.Success(count);
        }
        catch (Exception ex)
        {
            BusFileLogger.LogError("Error getting bus count", ex);
            return Result.Failure<int>(
                Error.Failure("Bus.CountFailed", BusMessages.ErrorMessages.DatabaseError)
            );
        }
    }

    public async Task<Result<BusEntity>> UpdateAsync(BusEntity bus)
    {
        try
        {
            BusFileLogger.LogInfo("Updating bus: {0}", bus.BusId);

            bus.UpdatedOn = DateTime.UtcNow;
            bus.UpdatedBy = "System";

            _context.Buses.Update(bus);
            _context.Entry(bus).Property(x => x.UpdatedBy).IsModified = true;
            _context.Entry(bus).Property(x => x.UpdatedOn).IsModified = true;

            await _context.SaveChangesAsync();

            BusFileLogger.LogInfo("Bus updated successfully: {0}", bus.BusId);
            return Result.Success(bus);
        }
        catch (Exception ex)
        {
            BusFileLogger.LogError("Error updating bus", ex);
            return Result.Failure<BusEntity>(
                Error.Failure("Bus.UpdateFailed", BusMessages.ErrorMessages.DatabaseError)
            );
        }
    }

    public async Task<Result<bool>> DeleteAsync(int busId)
    {
        try
        {
            BusFileLogger.LogInfo("Deleting bus: {0}", busId);

            var bus = await _context.Buses.FindAsync(busId);
            if (bus == null)
            {
                BusFileLogger.LogWarning("Bus not found for deletion: {0}", busId);
                return Result.Failure<bool>(
                    Error.NotFound("Bus.NotFound", BusMessages.ErrorMessages.BusNotFound)
                );
            }

            _context.Buses.Remove(bus);
            await _context.SaveChangesAsync();

            BusFileLogger.LogInfo("Bus deleted successfully: {0}", busId);
            return Result.Success(true);
        }
        catch (Exception ex)
        {
            BusFileLogger.LogError("Error deleting bus", ex);
            return Result.Failure<bool>(
                Error.Failure("Bus.DeleteFailed", BusMessages.ErrorMessages.DatabaseError)
            );
        }
    }

    public async Task<Result<bool>> ExistsByRegistrationNoAsync(string registrationNo)
    {
        try
        {
            var exists = await _context.Buses.AnyAsync(b =>
                b.RegistrationNo == registrationNo && b.IsActive
            );
            return Result.Success(exists);
        }
        catch (Exception ex)
        {
            BusFileLogger.LogError("Error checking bus exists by registration", ex);
            return Result.Failure<bool>(
                Error.Failure("Bus.ExistsFailed", BusMessages.ErrorMessages.DatabaseError)
            );
        }
    }

    public async Task<Result<bool>> ExistsByIdAndVendorAsync(int busId, int vendorId)
    {
        try
        {
            var exists = await _context.Buses.AnyAsync(b =>
                b.BusId == busId && b.VendorId == vendorId
            );
            return Result.Success(exists);
        }
        catch (Exception ex)
        {
            BusFileLogger.LogError("Error checking bus exists by ID and vendor", ex);
            return Result.Failure<bool>(
                Error.Failure("Bus.ExistsFailed", BusMessages.ErrorMessages.DatabaseError)
            );
        }
    }

    public async Task<Result<List<BusEntity>>> GetAwaitingConfirmationByVendorAsync(int vendorId)
    {
        try
        {
            var buses = await _context
                .Buses.Include(b => b.Vendor)
                .ThenInclude(v => v.User)
                .Where(b => b.VendorId == vendorId && b.Status == BusStatus.PendingApproval)
                .ToListAsync();
            return Result.Success(buses);
        }
        catch (Exception ex)
        {
            BusFileLogger.LogError("Error getting awaiting confirmation buses", ex);
            return Result.Failure<List<BusEntity>>(
                Error.Failure(
                    "Bus.AwaitingConfirmationFailed",
                    BusMessages.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result<bool>> ExistsByNameAndVendorAsync(string busName, int vendorId)
    {
        try
        {
            var exists = await _context.Buses.AnyAsync(b =>
                b.BusName == busName && b.VendorId == vendorId && b.IsActive
            );
            return Result.Success(exists);
        }
        catch (Exception ex)
        {
            BusFileLogger.LogError("Error checking bus exists by name and vendor", ex);
            return Result.Failure<bool>(
                Error.Failure("Bus.ExistsFailed", BusMessages.ErrorMessages.DatabaseError)
            );
        }
    }

    public async Task<Result<bool>> ExistsAsync(int busId)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                "SELECT COUNT(1) FROM Buses WHERE BusId = @BusId",
                connection
            );
            command.Parameters.AddWithValue("@BusId", busId);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            var count = result != null ? (int)result : 0;
            return Result.Success(count > 0);
        }
        catch (Exception ex)
        {
            BusFileLogger.LogError("Error checking bus exists by ID", ex);
            return Result.Failure<bool>(
                Error.Failure("Bus.ExistsFailed", BusMessages.ErrorMessages.DatabaseError)
            );
        }
    }

    // Admin methods using ADO.NET for read operations
    public async Task<List<BusEntity>> GetAllBusesForAdminAsync()
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_GetAllBusesForAdmin", connection);
            command.CommandType = CommandType.StoredProcedure;

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var buses = new List<BusEntity>();
            while (await reader.ReadAsync())
            {
                buses.Add(MapBusFromReader(reader));
            }
            return buses;
        }
        catch (Exception ex)
        {
            BusFileLogger.LogError("Error getting all buses for admin", ex);
            return new List<BusEntity>();
        }
    }

    public async Task<List<BusEntity>> GetBusesByStatusAsync(BusStatus status)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_GetBusesByStatus", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@Status", (int)status);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var buses = new List<BusEntity>();
            while (await reader.ReadAsync())
            {
                buses.Add(MapBusFromReader(reader));
            }
            return buses;
        }
        catch (Exception ex)
        {
            BusFileLogger.LogError("Error getting buses by status", ex);
            return new List<BusEntity>();
        }
    }

    public async Task<List<BusEntity>> FilterBusesForAdminAsync(
        string? searchName,
        int? status,
        bool? isActive
    )
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_FilterBusesForAdmin", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@SearchName", (object?)searchName ?? DBNull.Value);
            command.Parameters.AddWithValue("@Status", (object?)status ?? DBNull.Value);
            command.Parameters.AddWithValue("@IsActive", (object?)isActive ?? DBNull.Value);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var buses = new List<BusEntity>();
            while (await reader.ReadAsync())
            {
                buses.Add(MapBusFromReader(reader));
            }
            return buses;
        }
        catch (Exception ex)
        {
            BusFileLogger.LogError("Error filtering buses for admin", ex);
            return new List<BusEntity>();
        }
    }

    public async Task<BusEntity?> GetBusDetailsForAdminAsync(int busId)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_GetBusDetailsForAdmin", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@BusId", busId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapBusFromReader(reader);
            }
            return null;
        }
        catch (Exception ex)
        {
            BusFileLogger.LogError("Error getting bus details for admin", ex);
            return null;
        }
    }

    private static BusEntity MapBusFromReader(SqlDataReader reader)
    {
        var bus = new BusEntity
        {
            BusId = reader.GetInt32("BusId"),
            BusName = reader.GetString("BusName"),
            BusType = (BusType)reader.GetInt32("BusType"),
            TotalSeats = reader.GetInt32("TotalSeats"),
            RegistrationNo = reader.GetString("RegistrationNo"),
            Status = (BusStatus)reader.GetInt32("Status"),
            Amenities = (BusAmenities)reader.GetInt32("Amenities"),
            DriverName = reader.IsDBNull("DriverName") ? null! : reader.GetString("DriverName"),
            DriverContact = reader.IsDBNull("DriverContact")
                ? null!
                : reader.GetString("DriverContact"),
            IsActive = reader.GetBoolean("IsActive"),
            CreatedOn = reader.GetDateTime("CreatedOn"),
            VendorId = reader.GetInt32("VendorId"),
            CreatedBy = reader.IsDBNull("CreatedBy") ? string.Empty : reader.GetString("CreatedBy"),
            UpdatedBy = reader.IsDBNull("UpdatedBy") ? null : reader.GetString("UpdatedBy"),
            SeatLayoutTemplateId = reader.IsDBNull("SeatLayoutTemplateId") ? null : reader.GetInt32("SeatLayoutTemplateId")
        };

        // Create vendor with agency name if available
        if (!reader.IsDBNull("VendorName"))
        {
            bus.Vendor = new Domain.Entities.Vendor
            {
                VendorId = reader.GetInt32("VendorId"),
                AgencyName = reader.GetString("VendorName")
            };
        }

        return bus;
    }

    public async Task<Result<bool>> ApplyTemplateAsync(int busId, int templateId)
    {
        try
        {
            _logger.LogInformation(
                MagicStrings.LogMessages.BusTemplateApplicationStarted,
                busId,
                templateId
            );

            // Use EF Core for write operation (following rule 2)
            var bus = await _context.Buses.FindAsync(busId);
            if (bus == null)
            {
                _logger.LogWarning("Bus not found for template application: {BusId}", busId);
                return Result.Failure<bool>(Error.NotFound("Bus.NotFound", "Bus not found"));
            }

            bus.SeatLayoutTemplateId = templateId;
            bus.UpdatedOn = DateTime.UtcNow;
            bus.UpdatedBy = "System";

            await _context.SaveChangesAsync();

            _logger.LogInformation(MagicStrings.LogMessages.BusTemplateApplicationCompleted, busId);
            return Result.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.BusTemplateApplicationFailed, ex.Message);
            return Result.Failure<bool>(
                Error.Failure("Template.ApplyFailed", "Template application failed")
            );
        }
    }
}
