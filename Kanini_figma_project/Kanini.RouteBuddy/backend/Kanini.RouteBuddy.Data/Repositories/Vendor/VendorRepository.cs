using Microsoft.EntityFrameworkCore;
using Kanini.RouteBuddy.Data.DatabaseContext;
using Kanini.RouteBuddy.Domain.Enums;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;
using Kanini.RouteBuddy.Common.Services;

namespace Kanini.RouteBuddy.Data.Repositories.Vendor;

public class VendorRepository : IVendorRepository
{
    private readonly RouteBuddyDatabaseContext _context;
    private readonly string _connectionString;

    public VendorRepository(RouteBuddyDatabaseContext context, IConfiguration configuration)
    {
        _context = context;
        _connectionString = configuration.GetConnectionString("DatabaseConnectionString") ?? throw new ArgumentNullException(nameof(configuration), "Database connection string not found");
    }

    public async Task<Domain.Entities.Vendor> CreateAsync(Domain.Entities.Vendor vendor)
    {
        try
        {
            VendorFileLogger.LogInfo("Creating vendor: {0}", vendor.AgencyName);
            _context.Vendors.Add(vendor);
            await _context.SaveChangesAsync();
            VendorFileLogger.LogInfo("Vendor created successfully with ID: {0}", vendor.VendorId);
            return vendor;
        }
        catch (SqlException ex)
        {
            VendorFileLogger.LogError("SQL error creating vendor: {0}", ex, vendor.AgencyName);
            return null!;
        }
        catch (DbUpdateException ex)
        {
            VendorFileLogger.LogError("Database update error creating vendor: {0}", ex, vendor.AgencyName);
            return null!;
        }
    }

    public async Task<Domain.Entities.Vendor> CreateVendorAsync(Domain.Entities.Vendor vendor)
    {
        return await CreateAsync(vendor);
    }

    public async Task<Domain.Entities.Vendor?> GetByIdAsync(int vendorId)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_GetVendorById", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@VendorId", vendorId);
            
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            
            if (await reader.ReadAsync())
            {
                return new Domain.Entities.Vendor
                {
                    VendorId = reader.GetInt32("VendorId"),
                    UserId = reader.GetInt32("UserId"),
                    AgencyName = reader.GetString("AgencyName"),
                    OwnerName = reader.GetString("OwnerName"),
                    BusinessLicenseNumber = reader.GetString("BusinessLicenseNumber"),
                    OfficeAddress = reader.GetString("OfficeAddress"),
                    FleetSize = reader.GetInt32("FleetSize"),
                    TaxRegistrationNumber = reader.IsDBNull("TaxRegistrationNumber") ? null : reader.GetString("TaxRegistrationNumber"),
                    Status = (VendorStatus)reader.GetInt32("Status"),
                    IsActive = reader.GetBoolean("IsActive"),
                    User = new Domain.Entities.User
                    {
                        UserId = reader.GetInt32("UserId"),
                        Email = reader.GetString("Email"),
                        Phone = reader.GetString("Phone")
                    }
                };
            }
            return null;
        }
        catch (SqlException ex)
        {
            VendorFileLogger.LogError("SQL error fetching vendor by ID: {0}", ex, vendorId);
            return null;
        }
    }

    public async Task<Domain.Entities.Vendor?> GetByUserIdAsync(int userId)
    {
        return await _context.Vendors
            .Include(v => v.User)
            .OrderByDescending(v => v.CreatedOn)
            .FirstOrDefaultAsync(v => v.UserId == userId);
    }

    public async Task<Domain.Entities.Vendor?> GetByEmailAsync(string email)
    {
        return await _context.Vendors
            .Include(v => v.User)
            .OrderByDescending(v => v.CreatedOn)
            .FirstOrDefaultAsync(v => v.User.Email == email);
    }

    public async Task<IEnumerable<Domain.Entities.Vendor>> GetAllAsync(int pageNumber, int pageSize)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_GetAllVendors", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@Offset", (pageNumber - 1) * pageSize);
            command.Parameters.AddWithValue("@PageSize", pageSize);
            
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            
            var vendors = new List<Domain.Entities.Vendor>();
            while (await reader.ReadAsync())
            {
                vendors.Add(new Domain.Entities.Vendor
                {
                    VendorId = reader.GetInt32("VendorId"),
                    UserId = reader.GetInt32("UserId"),
                    AgencyName = reader.GetString("AgencyName"),
                    OwnerName = reader.GetString("OwnerName"),
                    BusinessLicenseNumber = reader.GetString("BusinessLicenseNumber"),
                    OfficeAddress = reader.GetString("OfficeAddress"),
                    FleetSize = reader.GetInt32("FleetSize"),
                    TaxRegistrationNumber = reader.IsDBNull("TaxRegistrationNumber") ? null : reader.GetString("TaxRegistrationNumber"),
                    IsActive = reader.GetBoolean("IsActive"),
                    Status = (VendorStatus)reader.GetInt32("Status"),
                    CreatedOn = reader.GetDateTime("CreatedOn"),
                    User = new Domain.Entities.User
                    {
                        UserId = reader.GetInt32("UserId"),
                        Email = reader.GetString("Email"),
                        Phone = reader.GetString("Phone")
                    }
                });
            }
            return vendors;
        }
        catch (SqlException ex)
        {
            VendorFileLogger.LogError("SQL error getting all vendors", ex);
            return new List<Domain.Entities.Vendor>();
        }
    }

    public async Task<int> GetTotalCountAsync()
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_GetVendorTotalCount", connection);
            command.CommandType = CommandType.StoredProcedure;
            
            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return result != null ? (int)result : 0;
        }
        catch (SqlException ex)
        {
            VendorFileLogger.LogError("SQL error getting total count", ex);
            return 0;
        }
    }

    public async Task<Domain.Entities.Vendor> UpdateAsync(Domain.Entities.Vendor vendor)
    {
        try
        {
            vendor.UpdatedOn = DateTime.UtcNow;
            vendor.UpdatedBy = "System"; // In real app, get from JWT token
            _context.Vendors.Update(vendor);
            await _context.SaveChangesAsync();
            return vendor;
        }
        catch (SqlException ex)
        {
            VendorFileLogger.LogError("SQL error updating vendor", ex);
            return vendor;
        }
        catch (DbUpdateException ex)
        {
            VendorFileLogger.LogError("Database update error updating vendor", ex);
            return vendor;
        }
    }

    public async Task<bool> DeleteAsync(int vendorId)
    {
        try
        {
            var vendor = await _context.Vendors.FindAsync(vendorId);
            if (vendor == null) return false;

            _context.Vendors.Remove(vendor);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (SqlException ex)
        {
            VendorFileLogger.LogError("SQL error deleting vendor", ex);
            return false;
        }
        catch (DbUpdateException ex)
        {
            VendorFileLogger.LogError("Database update error deleting vendor", ex);
            return false;
        }
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_CheckVendorExistsByEmail", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@Email", email);
            
            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return result != null && (int)result > 0;
        }
        catch (SqlException ex)
        {
            VendorFileLogger.LogError("SQL error checking email exists", ex);
            return false;
        }
    }

    public async Task<bool> ExistsByLicenseNumberAsync(string licenseNumber)
    {
        return await _context.Vendors
            .OrderByDescending(v => v.CreatedOn)
            .AnyAsync(v => v.BusinessLicenseNumber == licenseNumber);
    }

    public async Task<bool> ExistsByIdAsync(int vendorId)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_CheckVendorExistsById", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@VendorId", vendorId);
            
            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return result != null && (int)result > 0;
        }
        catch (SqlException ex)
        {
            VendorFileLogger.LogError("SQL error checking vendor exists: {0}", ex, vendorId);
            return false;
        }
    }

    public async Task<IEnumerable<Domain.Entities.Vendor>> GetPendingVendorsAsync(int pageNumber, int pageSize)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_GetPendingVendors", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@Offset", (pageNumber - 1) * pageSize);
            command.Parameters.AddWithValue("@PageSize", pageSize);
            
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            
            var vendors = new List<Domain.Entities.Vendor>();
            while (await reader.ReadAsync())
            {
                vendors.Add(new Domain.Entities.Vendor
                {
                    VendorId = reader.GetInt32("VendorId"),
                    UserId = reader.GetInt32("UserId"),
                    AgencyName = reader.GetString("AgencyName"),
                    OwnerName = reader.GetString("OwnerName"),
                    BusinessLicenseNumber = reader.GetString("BusinessLicenseNumber"),
                    OfficeAddress = reader.GetString("OfficeAddress"),
                    FleetSize = reader.GetInt32("FleetSize"),
                    TaxRegistrationNumber = reader.IsDBNull("TaxRegistrationNumber") ? null : reader.GetString("TaxRegistrationNumber"),
                    Status = (VendorStatus)reader.GetInt32("Status"),
                    IsActive = reader.GetBoolean("IsActive"),
                    CreatedOn = reader.GetDateTime("CreatedOn"),
                    User = new Domain.Entities.User
                    {
                        UserId = reader.GetInt32("UserId"),
                        Email = reader.GetString("Email"),
                        Phone = reader.GetString("Phone")
                    }
                });
            }
            return vendors;
        }
        catch (SqlException ex)
        {
            VendorFileLogger.LogError("SQL error getting pending vendors", ex);
            return new List<Domain.Entities.Vendor>();
        }
    }

    public async Task<int> GetPendingVendorsCountAsync()
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_GetPendingVendorsCount", connection);
            command.CommandType = CommandType.StoredProcedure;
            
            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return result != null ? (int)result : 0;
        }
        catch (SqlException ex)
        {
            VendorFileLogger.LogError("SQL error getting pending vendors count", ex);
            return 0;
        }
    }

    public async Task<(int TotalBuses, int ActiveBuses, int PendingBuses, int TotalRoutes, int TotalSchedules, int UpcomingSchedules, string VendorStatus)> GetDashboardSummaryAsync(int vendorId)
    {
        try
        {
            VendorFileLogger.LogInfo("Getting dashboard summary for vendor: {0}", vendorId);
            
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_GetVendorDashboardSummary", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@VendorId", vendorId);
            
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            
            if (await reader.ReadAsync())
            {
                var result = (
                    reader.GetInt32("TotalBuses"),
                    reader.GetInt32("ActiveBuses"),
                    reader.GetInt32("PendingBuses"),
                    reader.GetInt32("TotalRoutes"),
                    reader.GetInt32("TotalSchedules"),
                    reader.GetInt32("UpcomingSchedules"),
                    reader.GetString("VendorStatus")
                );
                
                VendorFileLogger.LogInfo("Dashboard summary retrieved - Buses: {0}, Routes: {1}, Status: {2}", result.Item1, result.Item4, result.Item7);
                return result;
            }
            
            VendorFileLogger.LogWarning("No dashboard data found for vendor: {0}", vendorId);
            return (0, 0, 0, 0, 0, 0, "Unknown");
        }
        catch (SqlException ex)
        {
            VendorFileLogger.LogError("SQL error getting dashboard summary for vendor: {0}", ex, vendorId);
            throw;
        }
        catch (Exception ex)
        {
            VendorFileLogger.LogError("Unexpected error getting dashboard summary for vendor: {0}", ex, vendorId);
            throw;
        }
    }

    public async Task<IEnumerable<Domain.Entities.Vendor>> FilterVendorsAsync(string? searchName, bool? isActive, int? status)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_FilterVendorsForAdmin", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@SearchName", (object?)searchName ?? DBNull.Value);
            command.Parameters.AddWithValue("@IsActive", (object?)isActive ?? DBNull.Value);
            command.Parameters.AddWithValue("@Status", (object?)status ?? DBNull.Value);
            
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            
            var vendors = new List<Domain.Entities.Vendor>();
            while (await reader.ReadAsync())
            {
                vendors.Add(new Domain.Entities.Vendor
                {
                    VendorId = reader.GetInt32("VendorId"),
                    UserId = reader.GetInt32("UserId"),
                    AgencyName = reader.GetString("AgencyName"),
                    OwnerName = reader.GetString("OwnerName"),
                    BusinessLicenseNumber = reader.GetString("BusinessLicenseNumber"),
                    OfficeAddress = reader.GetString("OfficeAddress"),
                    FleetSize = reader.GetInt32("FleetSize"),
                    IsActive = reader.GetBoolean("IsActive"),
                    Status = (VendorStatus)reader.GetInt32("Status"),
                    CreatedOn = reader.GetDateTime("CreatedOn"),
                    User = new Domain.Entities.User
                    {
                        UserId = reader.GetInt32("UserId"),
                        Email = reader.GetString("Email"),
                        Phone = reader.GetString("Phone")
                    }
                });
            }
            return vendors.OrderByDescending(v => v.CreatedOn);
        }
        catch (SqlException ex)
        {
            VendorFileLogger.LogError("SQL error filtering vendors", ex);
            return new List<Domain.Entities.Vendor>();
        }
    }

    public async Task<VendorApprovalData?> GetVendorForApprovalAsync(int vendorId)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_GetVendorForApproval", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@VendorId", vendorId);
            
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            
            VendorApprovalData? result = null;
            
            // Read vendor details
            if (await reader.ReadAsync())
            {
                result = new VendorApprovalData
                {
                    Vendor = new Domain.Entities.Vendor
                    {
                        VendorId = reader.GetInt32("VendorId"),
                        UserId = reader.GetInt32("UserId"),
                        AgencyName = reader.GetString("AgencyName"),
                        OwnerName = reader.GetString("OwnerName"),
                        BusinessLicenseNumber = reader.GetString("BusinessLicenseNumber"),
                        OfficeAddress = reader.GetString("OfficeAddress"),
                        FleetSize = reader.GetInt32("FleetSize"),
                        TaxRegistrationNumber = reader.IsDBNull("TaxRegistrationNumber") ? null : reader.GetString("TaxRegistrationNumber"),
                        Status = (VendorStatus)reader.GetInt32("Status"),
                        IsActive = reader.GetBoolean("IsActive"),
                        CreatedOn = reader.GetDateTime("CreatedOn"),
                        User = new Domain.Entities.User
                        {
                            UserId = reader.GetInt32("UserId"),
                            Email = reader.GetString("Email"),
                            Phone = reader.GetString("Phone")
                        }
                    }
                };
            }
            
            // Read documents
            if (result != null && await reader.NextResultAsync())
            {
                while (await reader.ReadAsync())
                {
                    result.Documents.Add(new Domain.Entities.VendorDocument
                    {
                        DocumentId = reader.GetInt32("DocumentId"),
                        VendorId = reader.GetInt32("VendorId"),
                        DocumentFile = (DocumentCategory)reader.GetInt32("DocumentFile"),
                        DocumentPath = reader.GetString("DocumentPath"),
                        IssueDate = reader.IsDBNull("IssueDate") ? null : reader.GetDateTime("IssueDate"),
                        ExpiryDate = reader.IsDBNull("ExpiryDate") ? null : reader.GetDateTime("ExpiryDate"),
                        UploadedAt = reader.GetDateTime("UploadedAt"),
                        IsVerified = reader.GetBoolean("IsVerified"),
                        VerifiedAt = reader.IsDBNull("VerifiedAt") ? null : reader.GetDateTime("VerifiedAt"),
                        VerifiedBy = reader.IsDBNull("VerifiedBy") ? null : reader.GetString("VerifiedBy"),
                        RejectedReason = reader.IsDBNull("RejectedReason") ? null : reader.GetString("RejectedReason"),
                        Status = (DocumentStatus)reader.GetInt32("Status")
                    });
                }
            }
            
            return result;
        }
        catch (SqlException ex)
        {
            VendorFileLogger.LogError("SQL error getting vendor for approval: {0}", ex, vendorId);
            return null;
        }
    }

    public async Task<Domain.Entities.Vendor> UpdateVendorOnlyAsync(Domain.Entities.Vendor vendor)
    {
        try
        {
            vendor.UpdatedOn = DateTime.UtcNow;
            vendor.UpdatedBy = "Admin";
            
            // Only update vendor entity, not related entities
            _context.Entry(vendor).State = EntityState.Modified;
            _context.Entry(vendor).Property(v => v.UserId).IsModified = false;
            
            await _context.SaveChangesAsync();
            return vendor;
        }
        catch (SqlException ex)
        {
            VendorFileLogger.LogError("SQL error updating vendor only", ex);
            return vendor;
        }
        catch (DbUpdateException ex)
        {
            VendorFileLogger.LogError("Database update error updating vendor only", ex);
            return vendor;
        }
    }

    public async Task UpdateVendorDocumentsStatusAsync(int vendorId, DocumentStatus status, string verifiedBy)
    {
        try
        {
            var utcNow = DateTime.UtcNow;
            await _context.Database.ExecuteSqlInterpolatedAsync(
                $"UPDATE VendorDocuments SET Status = {(int)status}, IsVerified = {status == DocumentStatus.Verified}, VerifiedBy = {verifiedBy}, VerifiedAt = {utcNow}, UpdatedBy = {verifiedBy}, UpdatedOn = {utcNow} WHERE VendorId = {vendorId}");
            
            VendorFileLogger.LogInfo("Updated documents status for vendor: {0}", vendorId);
        }
        catch (SqlException ex)
        {
            VendorFileLogger.LogError("SQL error updating vendor documents status: {0}", ex, vendorId);
            throw;
        }
    }
}