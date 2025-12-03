using Microsoft.Data.SqlClient;
using System.Data;
using Kanini.RouteBuddy.Common.Utility;
using Kanini.RouteBuddy.Common.Errors;
using Kanini.RouteBuddy.Domain.Entities;
using Kanini.RouteBuddy.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Kanini.RouteBuddy.Data.Repositories.Vendor;

public class VendorSeatLayoutRepository : IVendorSeatLayoutRepository
{
    private readonly string _connectionString;
    private readonly ILogger<VendorSeatLayoutRepository> _logger;

    public VendorSeatLayoutRepository(IConfiguration configuration, ILogger<VendorSeatLayoutRepository> logger)
    {
        _connectionString = configuration.GetConnectionString("DatabaseConnectionString")!;
        _logger = logger;
    }

    public async Task<Result<List<SeatLayoutTemplate>>> GetTemplatesByBusTypeAsync(int busType)
    {
        try
        {
            _logger.LogInformation("Getting seat layout templates for bus type: {BusType}", busType);
            
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_GetSeatLayoutTemplatesByBusType", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@BusType", busType);
            
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            
            var templates = new List<SeatLayoutTemplate>();
            while (await reader.ReadAsync())
            {
                templates.Add(new SeatLayoutTemplate
                {
                    SeatLayoutTemplateId = reader.GetInt32("SeatLayoutTemplateId"),
                    TemplateName = reader.GetString("TemplateName"),
                    TotalSeats = reader.GetInt32("TotalSeats"),
                    BusType = (BusType)reader.GetInt32("BusType"),
                    Description = reader.IsDBNull("Description") ? null : reader.GetString("Description"),
                    IsActive = reader.GetBoolean("IsActive"),
                    CreatedOn = reader.GetDateTime("CreatedOn")
                });
            }
            
            _logger.LogInformation("Retrieved {Count} templates for bus type {BusType}", templates.Count, busType);
            return Result.Success(templates);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting templates by bus type: {Message}", ex.Message);
            return Result.Failure<List<SeatLayoutTemplate>>(
                Error.Failure("SeatLayout.GetByTypeFailed", "Failed to retrieve templates by bus type"));
        }
    }

    public async Task<Result<SeatLayoutTemplate>> GetTemplateByIdAsync(int templateId)
    {
        try
        {
            _logger.LogInformation("Getting seat layout template by ID: {TemplateId}", templateId);
            
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_GetSeatLayoutTemplateById", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@SeatLayoutTemplateId", templateId);
            
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            
            if (await reader.ReadAsync())
            {
                var template = new SeatLayoutTemplate
                {
                    SeatLayoutTemplateId = reader.GetInt32("SeatLayoutTemplateId"),
                    TemplateName = reader.GetString("TemplateName"),
                    TotalSeats = reader.GetInt32("TotalSeats"),
                    BusType = (BusType)reader.GetInt32("BusType"),
                    Description = reader.IsDBNull("Description") ? null : reader.GetString("Description"),
                    IsActive = reader.GetBoolean("IsActive"),
                    CreatedOn = reader.GetDateTime("CreatedOn")
                };
                
                _logger.LogInformation("Template retrieved successfully: {TemplateId}", templateId);
                return Result.Success(template);
            }
            
            _logger.LogWarning("Template not found: {TemplateId}", templateId);
            return Result.Failure<SeatLayoutTemplate>(
                Error.NotFound("SeatLayout.TemplateNotFound", "Seat layout template not found"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting template by ID: {Message}", ex.Message);
            return Result.Failure<SeatLayoutTemplate>(
                Error.Failure("SeatLayout.GetByIdFailed", "Failed to retrieve seat layout template"));
        }
    }

    public async Task<Result<List<SeatLayoutDetail>>> GetBusSeatLayoutAsync(int busId)
    {
        try
        {
            _logger.LogInformation("Getting seat layout for bus: {BusId}", busId);
            
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_GetBusSeatLayout", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@BusId", busId);
            
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            
            var seatDetails = new List<SeatLayoutDetail>();
            while (await reader.ReadAsync())
            {
                seatDetails.Add(new SeatLayoutDetail
                {
                    SeatLayoutDetailId = reader.GetInt32("SeatLayoutDetailId"),
                    SeatLayoutTemplateId = reader.GetInt32("SeatLayoutTemplateId"),
                    SeatNumber = reader.GetString("SeatNumber"),
                    SeatType = (SeatType)reader.GetInt32("SeatType"),
                    SeatPosition = (SeatPosition)reader.GetInt32("SeatPosition"),
                    RowNumber = reader.GetInt32("RowNumber"),
                    ColumnNumber = reader.GetInt32("ColumnNumber"),
                    PriceTier = (PriceTier)reader.GetInt32("PriceTier")
                });
            }
            
            _logger.LogInformation("Retrieved {Count} seat details for bus {BusId}", seatDetails.Count, busId);
            return Result.Success(seatDetails);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting bus seat layout: {Message}", ex.Message);
            return Result.Failure<List<SeatLayoutDetail>>(
                Error.Failure("SeatLayout.GetBusLayoutFailed", "Failed to retrieve bus seat layout"));
        }
    }

    public async Task<Result<bool>> ApplyTemplateToLayoutAsync(int busId, int templateId)
    {
        try
        {
            _logger.LogInformation("Applying template {TemplateId} to bus {BusId}", templateId, busId);
            
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_ApplyTemplateToLayout", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@BusId", busId);
            command.Parameters.AddWithValue("@TemplateId", templateId);
            
            await connection.OpenAsync();
            var result = await command.ExecuteNonQueryAsync();
            
            _logger.LogInformation("Template applied successfully to bus {BusId}", busId);
            return Result.Success(result > 0);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error applying template to layout: {Message}", ex.Message);
            return Result.Failure<bool>(
                Error.Failure("SeatLayout.ApplyTemplateFailed", "Failed to apply template to bus layout"));
        }
    }
}