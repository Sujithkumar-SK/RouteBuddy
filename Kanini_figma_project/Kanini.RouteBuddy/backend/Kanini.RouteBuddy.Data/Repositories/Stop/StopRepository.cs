using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Kanini.RouteBuddy.Common.Errors;
using Kanini.RouteBuddy.Common.Utility;
using Kanini.RouteBuddy.Common.Services;
using Kanini.RouteBuddy.Data.DatabaseContext;
using Kanini.RouteBuddy.Data.Repositories.Stop;
using Kanini.RouteBuddy.Domain.Entities;
using StopEntity = Kanini.RouteBuddy.Domain.Entities.Stop;

namespace Kanini.RouteBuddy.Data.Repositories.Stop;

public class StopRepository : IStopRepository
{
    private readonly RouteBuddyDatabaseContext _context;
    private readonly string _connectionString;
    private readonly ILogger<StopRepository> _logger;

    public StopRepository(RouteBuddyDatabaseContext context, IConfiguration configuration, ILogger<StopRepository> logger)
    {
        _context = context;
        _connectionString = configuration.GetConnectionString("DatabaseConnectionString")!;
        _logger = logger;
    }

    public async Task<Result<StopEntity>> CreateAsync(StopEntity stop)
    {
        try
        {
            StopFileLogger.LogInfo(StopMessages.LogMessages.CreatingStop, stop.Name);
            
            _context.Stops.Add(stop);
            await _context.SaveChangesAsync();
            
            StopFileLogger.LogInfo(StopMessages.LogMessages.StopCreatedSuccessfully, stop.StopId);
            return Result.Success(stop);
        }
        catch (Exception ex)
        {
            StopFileLogger.LogError("Error creating stop", ex);
            return Result.Failure<StopEntity>(
                Error.Failure(StopMessages.ErrorCodes.CreationFailed, StopMessages.ErrorMessages.DatabaseError)
            );
        }
    }

    public async Task<Result<StopEntity>> GetByIdAsync(int stopId)
    {
        try
        {
            StopFileLogger.LogInfo(StopMessages.LogMessages.GettingStopById, stopId);
            
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_GetStopById", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@StopId", stopId);
            
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            
            if (await reader.ReadAsync())
            {
                var stop = new StopEntity
                {
                    StopId = reader.GetInt32("StopId"),
                    Name = reader.GetString("Name"),
                    Landmark = reader.IsDBNull("Landmark") ? null : reader.GetString("Landmark"),
                    IsActive = reader.GetBoolean("IsActive"),
                    CreatedBy = reader.GetString("CreatedBy"),
                    CreatedOn = reader.GetDateTime("CreatedOn")
                };
                
                StopFileLogger.LogInfo(StopMessages.LogMessages.StopRetrievedSuccessfully, stopId);
                return Result.Success(stop);
            }
            
            StopFileLogger.LogWarning(StopMessages.LogMessages.StopNotFound, stopId);
            return Result.Failure<StopEntity>(
                Error.NotFound(StopMessages.ErrorCodes.StopNotFound, StopMessages.ErrorMessages.StopNotFound)
            );
        }
        catch (Exception ex)
        {
            StopFileLogger.LogError("Error retrieving stop", ex);
            return Result.Failure<StopEntity>(
                Error.Failure(StopMessages.ErrorCodes.DatabaseError, StopMessages.ErrorMessages.DatabaseError)
            );
        }
    }

    public async Task<Result<List<StopEntity>>> GetAllAsync(int pageNumber, int pageSize)
    {
        try
        {
            StopFileLogger.LogInfo(StopMessages.LogMessages.GettingAllStops, pageNumber, pageSize);
            
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_GetAllStops", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@PageNumber", pageNumber);
            command.Parameters.AddWithValue("@PageSize", pageSize);
            
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            
            var stops = new List<StopEntity>();
            while (await reader.ReadAsync())
            {
                stops.Add(new StopEntity
                {
                    StopId = reader.GetInt32("StopId"),
                    Name = reader.GetString("Name"),
                    Landmark = reader.IsDBNull("Landmark") ? null : reader.GetString("Landmark"),
                    IsActive = reader.GetBoolean("IsActive"),
                    CreatedBy = reader.GetString("CreatedBy"),
                    CreatedOn = reader.GetDateTime("CreatedOn")
                });
            }
            
            StopFileLogger.LogInfo(StopMessages.LogMessages.StopsRetrievedSuccessfully, stops.Count);
            return Result.Success(stops);
        }
        catch (Exception ex)
        {
            StopFileLogger.LogError("Error getting stops", ex);
            return Result.Failure<List<StopEntity>>(
                Error.Failure(StopMessages.ErrorCodes.DatabaseError, StopMessages.ErrorMessages.DatabaseError)
            );
        }
    }

    public async Task<Result<int>> GetCountAsync()
    {
        try
        {
            var count = await _context.Stops.Where(s => s.IsActive).CountAsync();
            return Result.Success(count);
        }
        catch (Exception ex)
        {
            StopFileLogger.LogError("Error getting stop count", ex);
            return Result.Failure<int>(
                Error.Failure(StopMessages.ErrorCodes.DatabaseError, StopMessages.ErrorMessages.DatabaseError)
            );
        }
    }

    public async Task<Result<StopEntity>> UpdateAsync(StopEntity stop)
    {
        try
        {
            StopFileLogger.LogInfo(StopMessages.LogMessages.UpdatingStop, stop.StopId);
            
            stop.UpdatedOn = DateTime.UtcNow;
            stop.UpdatedBy = "System";
            
            _context.Stops.Update(stop);
            await _context.SaveChangesAsync();
            
            StopFileLogger.LogInfo(StopMessages.LogMessages.StopUpdatedSuccessfully, stop.StopId);
            return Result.Success(stop);
        }
        catch (Exception ex)
        {
            StopFileLogger.LogError("Error updating stop", ex);
            return Result.Failure<StopEntity>(
                Error.Failure(StopMessages.ErrorCodes.UpdateFailed, StopMessages.ErrorMessages.DatabaseError)
            );
        }
    }

    public async Task<Result<bool>> DeleteAsync(int stopId)
    {
        try
        {
            StopFileLogger.LogInfo(StopMessages.LogMessages.DeletingStop, stopId);
            
            var stop = await _context.Stops.FindAsync(stopId);
            if (stop == null)
            {
                StopFileLogger.LogWarning(StopMessages.LogMessages.StopNotFound, stopId);
                return Result.Failure<bool>(
                    Error.NotFound(StopMessages.ErrorCodes.StopNotFound, StopMessages.ErrorMessages.StopNotFound)
                );
            }

            _context.Stops.Remove(stop);
            await _context.SaveChangesAsync();
            
            StopFileLogger.LogInfo(StopMessages.LogMessages.StopDeletedSuccessfully, stopId);
            return Result.Success(true);
        }
        catch (Exception ex)
        {
            StopFileLogger.LogError("Error deleting stop", ex);
            return Result.Failure<bool>(
                Error.Failure(StopMessages.ErrorCodes.DeletionFailed, StopMessages.ErrorMessages.DatabaseError)
            );
        }
    }

    public async Task<Result<bool>> ExistsByNameAsync(string name)
    {
        try
        {
            var exists = await _context.Stops.AnyAsync(s => s.Name == name);
            return Result.Success(exists);
        }
        catch (Exception ex)
        {
            StopFileLogger.LogError("Error checking stop exists", ex);
            return Result.Failure<bool>(
                Error.Failure(StopMessages.ErrorCodes.DatabaseError, StopMessages.ErrorMessages.DatabaseError)
            );
        }
    }
}