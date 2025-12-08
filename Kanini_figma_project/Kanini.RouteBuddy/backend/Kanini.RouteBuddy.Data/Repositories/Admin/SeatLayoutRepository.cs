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

namespace Kanini.RouteBuddy.Data.Repositories.Admin;

public class SeatLayoutRepository : ISeatLayoutRepository
{
    private readonly string _connectionString;
    private readonly RouteBuddyDatabaseContext _context;
    private readonly ILogger<SeatLayoutRepository> _logger;

    public SeatLayoutRepository(
        IConfiguration configuration,
        RouteBuddyDatabaseContext context,
        ILogger<SeatLayoutRepository> logger
    )
    {
        _connectionString = configuration.GetConnectionString("DatabaseConnectionString")!;
        _context = context;
        _logger = logger;
    }

    // READ operations - ADO.NET with stored procedures
    public async Task<Result<List<SeatLayoutTemplate>>> GetAllTemplatesAsync()
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.SeatLayoutTemplateGetAllStarted);

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                MagicStrings.StoredProcedures.GetAllSeatLayoutTemplates,
                connection
            )
            {
                CommandType = CommandType.StoredProcedure,
            };

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var templates = new List<SeatLayoutTemplate>();
            while (await reader.ReadAsync())
            {
                var template = new SeatLayoutTemplate
                {
                    SeatLayoutTemplateId = reader.GetInt32("SeatLayoutTemplateId"),
                    TemplateName = reader.GetString("TemplateName"),
                    TotalSeats = reader.GetInt32("TotalSeats"),
                    BusType = (BusType)reader.GetInt32("BusType"),
                    Description = reader.IsDBNull("Description")
                        ? null
                        : reader.GetString("Description"),
                    IsActive = reader.GetBoolean("IsActive"),
                    CreatedBy = reader.GetString("CreatedBy"),
                    CreatedOn = reader.GetDateTime("CreatedOn"),
                    UpdatedBy = reader.IsDBNull("UpdatedBy") ? null : reader.GetString("UpdatedBy"),
                    UpdatedOn = reader.IsDBNull("UpdatedOn")
                        ? null
                        : reader.GetDateTime("UpdatedOn"),
                };
                templates.Add(template);
            }

            _logger.LogInformation(
                MagicStrings.LogMessages.SeatLayoutTemplateGetAllCompleted,
                templates.Count
            );
            return Result.Success(templates);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                MagicStrings.LogMessages.SeatLayoutTemplateGetAllFailed,
                ex.Message
            );
            return Result.Failure<List<SeatLayoutTemplate>>(
                Error.Failure(
                    "SeatLayoutTemplate.GetAllFailed",
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result<SeatLayoutTemplate>> GetTemplateByIdAsync(int templateId)
    {
        try
        {
            _logger.LogInformation(
                MagicStrings.LogMessages.SeatLayoutTemplateGetByIdStarted,
                templateId
            );

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                MagicStrings.StoredProcedures.GetSeatLayoutTemplateById,
                connection
            )
            {
                CommandType = CommandType.StoredProcedure,
            };

            command.Parameters.AddWithValue("@SeatLayoutTemplateId", templateId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            SeatLayoutTemplate? template = null;

            // Read template basic info
            if (await reader.ReadAsync())
            {
                template = new SeatLayoutTemplate
                {
                    SeatLayoutTemplateId = reader.GetInt32("SeatLayoutTemplateId"),
                    TemplateName = reader.GetString("TemplateName"),
                    TotalSeats = reader.GetInt32("TotalSeats"),
                    BusType = (BusType)reader.GetInt32("BusType"),
                    Description = reader.IsDBNull("Description")
                        ? null
                        : reader.GetString("Description"),
                    IsActive = reader.GetBoolean("IsActive"),
                    CreatedBy = reader.GetString("CreatedBy"),
                    CreatedOn = reader.GetDateTime("CreatedOn"),
                    UpdatedBy = reader.IsDBNull("UpdatedBy") ? null : reader.GetString("UpdatedBy"),
                    UpdatedOn = reader.IsDBNull("UpdatedOn")
                        ? null
                        : reader.GetDateTime("UpdatedOn"),
                };
            }

            if (template == null)
            {
                _logger.LogWarning(
                    MagicStrings.LogMessages.SeatLayoutTemplateGetByIdFailed,
                    MagicStrings.ErrorMessages.SeatLayoutTemplateNotFound
                );
                return Result.Failure<SeatLayoutTemplate>(
                    Error.NotFound(
                        "SeatLayoutTemplate.NotFound",
                        MagicStrings.ErrorMessages.SeatLayoutTemplateNotFound
                    )
                );
            }

            _logger.LogInformation(
                MagicStrings.LogMessages.SeatLayoutTemplateGetByIdCompleted,
                templateId
            );
            return Result.Success(template);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                MagicStrings.LogMessages.SeatLayoutTemplateGetByIdFailed,
                ex.Message
            );
            return Result.Failure<SeatLayoutTemplate>(
                Error.Failure(
                    "SeatLayoutTemplate.GetByIdFailed",
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result<SeatLayoutTemplate>> GetTemplateWithDetailsAsync(int templateId)
    {
        try
        {
            _logger.LogInformation(
                MagicStrings.LogMessages.SeatLayoutTemplateGetByIdStarted,
                templateId
            );

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                MagicStrings.StoredProcedures.GetSeatLayoutTemplateById,
                connection
            )
            {
                CommandType = CommandType.StoredProcedure,
            };

            command.Parameters.AddWithValue("@SeatLayoutTemplateId", templateId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            SeatLayoutTemplate? template = null;

            // Read template basic info
            if (await reader.ReadAsync())
            {
                template = new SeatLayoutTemplate
                {
                    SeatLayoutTemplateId = reader.GetInt32("SeatLayoutTemplateId"),
                    TemplateName = reader.GetString("TemplateName"),
                    TotalSeats = reader.GetInt32("TotalSeats"),
                    BusType = (BusType)reader.GetInt32("BusType"),
                    Description = reader.IsDBNull("Description")
                        ? null
                        : reader.GetString("Description"),
                    IsActive = reader.GetBoolean("IsActive"),
                    CreatedBy = reader.GetString("CreatedBy"),
                    CreatedOn = reader.GetDateTime("CreatedOn"),
                    UpdatedBy = reader.IsDBNull("UpdatedBy") ? null : reader.GetString("UpdatedBy"),
                    UpdatedOn = reader.IsDBNull("UpdatedOn")
                        ? null
                        : reader.GetDateTime("UpdatedOn"),
                    SeatLayoutDetails = new List<SeatLayoutDetail>(),
                };
            }

            if (template == null)
            {
                _logger.LogWarning(
                    MagicStrings.LogMessages.SeatLayoutTemplateGetByIdFailed,
                    MagicStrings.ErrorMessages.SeatLayoutTemplateNotFound
                );
                return Result.Failure<SeatLayoutTemplate>(
                    Error.NotFound(
                        "SeatLayoutTemplate.NotFound",
                        MagicStrings.ErrorMessages.SeatLayoutTemplateNotFound
                    )
                );
            }

            // Read seat layout details
            if (await reader.NextResultAsync())
            {
                while (await reader.ReadAsync())
                {
                    var seatDetail = new SeatLayoutDetail
                    {
                        SeatLayoutDetailId = reader.GetInt32("SeatLayoutDetailId"),
                        SeatLayoutTemplateId = reader.GetInt32("SeatLayoutTemplateId"),
                        SeatNumber = reader.GetString("SeatNumber"),
                        SeatType = (SeatType)reader.GetInt32("SeatType"),
                        SeatPosition = (SeatPosition)reader.GetInt32("SeatPosition"),
                        RowNumber = reader.GetInt32("RowNumber"),
                        ColumnNumber = reader.GetInt32("ColumnNumber"),
                        PriceTier = (PriceTier)reader.GetInt32("PriceTier"),
                        CreatedBy = reader.GetString("CreatedBy"),
                        CreatedOn = reader.GetDateTime("CreatedOn"),
                        UpdatedBy = reader.IsDBNull("UpdatedBy")
                            ? null
                            : reader.GetString("UpdatedBy"),
                        UpdatedOn = reader.IsDBNull("UpdatedOn")
                            ? null
                            : reader.GetDateTime("UpdatedOn"),
                    };
                    template.SeatLayoutDetails.Add(seatDetail);
                }
            }

            _logger.LogInformation(
                MagicStrings.LogMessages.SeatLayoutTemplateGetByIdCompleted,
                templateId
            );
            return Result.Success(template);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                MagicStrings.LogMessages.SeatLayoutTemplateGetByIdFailed,
                ex.Message
            );
            return Result.Failure<SeatLayoutTemplate>(
                Error.Failure(
                    "SeatLayoutTemplate.GetWithDetailsFailed",
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    // WRITE operations - EF Core
    public async Task<Result<SeatLayoutTemplate>> CreateTemplateAsync(
        SeatLayoutTemplate template,
        List<SeatLayoutDetail> details
    )
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            _logger.LogInformation(
                MagicStrings.LogMessages.SeatLayoutTemplateCreationStarted,
                template.TemplateName
            );

            // Check if template name already exists
            var nameExistsResult = await CheckTemplateNameExistsAsync(template.TemplateName);
            if (nameExistsResult.IsFailure)
            {
                await transaction.RollbackAsync();
                return Result.Failure<SeatLayoutTemplate>(nameExistsResult.Error);
            }

            if (nameExistsResult.Value)
            {
                await transaction.RollbackAsync();
                return Result.Failure<SeatLayoutTemplate>(
                    Error.Failure(
                        "SeatLayoutTemplate.NameExists",
                        MagicStrings.ErrorMessages.SeatLayoutTemplateNameExists
                    )
                );
            }

            // Create template
            _context.SeatLayoutTemplates.Add(template);
            await _context.SaveChangesAsync();

            // Create seat details
            foreach (var detail in details)
            {
                detail.SeatLayoutTemplateId = template.SeatLayoutTemplateId;
                _context.SeatLayoutDetails.Add(detail);
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation(
                MagicStrings.LogMessages.SeatLayoutTemplateCreationCompleted,
                template.SeatLayoutTemplateId
            );
            return Result.Success(template);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(
                ex,
                MagicStrings.LogMessages.SeatLayoutTemplateCreationFailed,
                ex.Message
            );
            return Result.Failure<SeatLayoutTemplate>(
                Error.Failure(
                    "SeatLayoutTemplate.CreateFailed",
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result<SeatLayoutTemplate>> UpdateTemplateAsync(
        SeatLayoutTemplate template,
        List<SeatLayoutDetail> details
    )
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            _logger.LogInformation(
                MagicStrings.LogMessages.SeatLayoutTemplateUpdateStarted,
                template.SeatLayoutTemplateId
            );

            // Check if template name already exists (excluding current template)
            var nameExistsResult = await CheckTemplateNameExistsAsync(
                template.TemplateName,
                template.SeatLayoutTemplateId
            );
            if (nameExistsResult.IsFailure)
            {
                await transaction.RollbackAsync();
                return Result.Failure<SeatLayoutTemplate>(nameExistsResult.Error);
            }

            if (nameExistsResult.Value)
            {
                await transaction.RollbackAsync();
                return Result.Failure<SeatLayoutTemplate>(
                    Error.Failure(
                        "SeatLayoutTemplate.NameExists",
                        MagicStrings.ErrorMessages.SeatLayoutTemplateNameExists
                    )
                );
            }

            // Update template
            _context.SeatLayoutTemplates.Update(template);

            // Remove existing seat details
            var existingDetails = await _context
                .SeatLayoutDetails.Where(d =>
                    d.SeatLayoutTemplateId == template.SeatLayoutTemplateId
                )
                .ToListAsync();
            _context.SeatLayoutDetails.RemoveRange(existingDetails);

            // Add new seat details
            foreach (var detail in details)
            {
                detail.SeatLayoutTemplateId = template.SeatLayoutTemplateId;
                _context.SeatLayoutDetails.Add(detail);
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation(
                MagicStrings.LogMessages.SeatLayoutTemplateUpdateCompleted,
                template.SeatLayoutTemplateId
            );
            return Result.Success(template);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(
                ex,
                MagicStrings.LogMessages.SeatLayoutTemplateUpdateFailed,
                ex.Message
            );
            return Result.Failure<SeatLayoutTemplate>(
                Error.Failure(
                    "SeatLayoutTemplate.UpdateFailed",
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result<bool>> DeactivateTemplateAsync(int templateId)
    {
        try
        {
            _logger.LogInformation(
                MagicStrings.LogMessages.SeatLayoutTemplateDeactivationStarted,
                templateId
            );

            var template = await _context.SeatLayoutTemplates.FindAsync(templateId);
            if (template == null)
            {
                return Result.Failure<bool>(
                    Error.NotFound(
                        "SeatLayoutTemplate.NotFound",
                        MagicStrings.ErrorMessages.SeatLayoutTemplateNotFound
                    )
                );
            }

            template.IsActive = false;
            template.UpdatedBy = "Admin";
            template.UpdatedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                MagicStrings.LogMessages.SeatLayoutTemplateDeactivationCompleted,
                templateId
            );
            return Result.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                MagicStrings.LogMessages.SeatLayoutTemplateDeactivationFailed,
                ex.Message
            );
            return Result.Failure<bool>(
                Error.Failure(
                    "SeatLayoutTemplate.DeactivationFailed",
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result<bool>> CheckTemplateNameExistsAsync(
        string templateName,
        int? excludeTemplateId = null
    )
    {
        try
        {
            var query = _context.SeatLayoutTemplates.Where(t =>
                t.TemplateName == templateName && t.IsActive
            );

            if (excludeTemplateId.HasValue)
            {
                query = query.Where(t => t.SeatLayoutTemplateId != excludeTemplateId.Value);
            }

            var exists = await query.AnyAsync();
            return Result.Success(exists);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check template name exists: {Message}", ex.Message);
            return Result.Failure<bool>(
                Error.Failure(
                    "SeatLayoutTemplate.NameCheckFailed",
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result<List<SeatLayoutTemplate>>> GetTemplatesByBusTypeAsync(int busType)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.SeatLayoutTemplatesByBusTypeStarted, busType);

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                MagicStrings.StoredProcedures.GetSeatLayoutTemplatesByBusType,
                connection
            )
            {
                CommandType = CommandType.StoredProcedure,
            };

            command.Parameters.AddWithValue("@BusType", busType);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var templates = new List<SeatLayoutTemplate>();
            while (await reader.ReadAsync())
            {
                var template = new SeatLayoutTemplate
                {
                    SeatLayoutTemplateId = reader.GetInt32("SeatLayoutTemplateId"),
                    TemplateName = reader.GetString("TemplateName"),
                    TotalSeats = reader.GetInt32("TotalSeats"),
                    BusType = (BusType)reader.GetInt32("BusType"),
                    Description = reader.IsDBNull("Description")
                        ? null
                        : reader.GetString("Description"),
                    IsActive = reader.GetBoolean("IsActive"),
                    CreatedBy = reader.GetString("CreatedBy"),
                    CreatedOn = reader.GetDateTime("CreatedOn"),
                    UpdatedBy = reader.IsDBNull("UpdatedBy") ? null : reader.GetString("UpdatedBy"),
                    UpdatedOn = reader.IsDBNull("UpdatedOn")
                        ? null
                        : reader.GetDateTime("UpdatedOn"),
                };
                templates.Add(template);
            }

            _logger.LogInformation(
                MagicStrings.LogMessages.SeatLayoutTemplatesByBusTypeCompleted,
                templates.Count
            );
            return Result.Success(templates);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                MagicStrings.LogMessages.SeatLayoutTemplatesByBusTypeFailed,
                ex.Message
            );
            return Result.Failure<List<SeatLayoutTemplate>>(
                Error.Failure(
                    "SeatLayoutTemplate.GetByBusTypeFailed",
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }
}
