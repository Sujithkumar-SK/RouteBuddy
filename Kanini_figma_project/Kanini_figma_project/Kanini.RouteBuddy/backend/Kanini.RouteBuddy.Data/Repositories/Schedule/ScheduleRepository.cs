using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Kanini.RouteBuddy.Common.Errors;
using Kanini.RouteBuddy.Common.Utility;
using Kanini.RouteBuddy.Common.Services;
using Kanini.RouteBuddy.Data.DatabaseContext;
using Kanini.RouteBuddy.Data.Repositories.Schedule;
using Kanini.RouteBuddy.Domain.Entities;
using BusSchedule = Kanini.RouteBuddy.Domain.Entities.BusSchedule;

namespace Kanini.RouteBuddy.Data.Repositories.Schedule;

public class ScheduleRepository : IScheduleRepository
{
    private readonly RouteBuddyDatabaseContext _context;
    private readonly string _connectionString;
    private readonly ILogger<ScheduleRepository> _logger;

    public ScheduleRepository(RouteBuddyDatabaseContext context, IConfiguration configuration, ILogger<ScheduleRepository> logger)
    {
        _context = context;
        _connectionString = configuration.GetConnectionString("DatabaseConnectionString")!;
        _logger = logger;
    }

    public async Task<Result<BusSchedule>> CreateAsync(BusSchedule schedule)
    {
        try
        {
            ScheduleFileLogger.LogInfo(ScheduleMessages.LogMessages.CreatingSchedule, schedule.BusId, schedule.RouteId, schedule.TravelDate);
            
            _context.BusSchedules.Add(schedule);
            await _context.SaveChangesAsync();
            
            ScheduleFileLogger.LogInfo(ScheduleMessages.LogMessages.ScheduleCreatedSuccessfully, schedule.ScheduleId);
            return Result.Success(schedule);
        }
        catch (Exception ex)
        {
            ScheduleFileLogger.LogError("Error creating schedule", ex);
            return Result.Failure<BusSchedule>(
                Error.Failure(ScheduleMessages.ErrorCodes.CreationFailed, ScheduleMessages.ErrorMessages.DatabaseError)
            );
        }
    }

    public async Task<Result<BusSchedule>> GetByIdAsync(int scheduleId)
    {
        try
        {
            ScheduleFileLogger.LogInfo(ScheduleMessages.LogMessages.GettingScheduleById, scheduleId);
            
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_GetScheduleById", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@ScheduleId", scheduleId);
            
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            
            if (await reader.ReadAsync())
            {
                var schedule = new BusSchedule
                {
                    ScheduleId = reader.GetInt32(0),
                    BusId = reader.GetInt32(1),
                    RouteId = reader.GetInt32(2),
                    TravelDate = reader.GetDateTime(3),
                    DepartureTime = reader.GetTimeSpan(4),
                    ArrivalTime = reader.GetTimeSpan(5),
                    AvailableSeats = reader.GetInt32(6),
                    IsActive = reader.GetBoolean(7)
                };
                
                ScheduleFileLogger.LogInfo(ScheduleMessages.LogMessages.ScheduleRetrievedSuccessfully, scheduleId);
                return Result.Success(schedule);
            }
            
            ScheduleFileLogger.LogWarning(ScheduleMessages.LogMessages.ScheduleNotFound, scheduleId);
            return Result.Failure<BusSchedule>(
                Error.NotFound(ScheduleMessages.ErrorCodes.ScheduleNotFound, ScheduleMessages.ErrorMessages.ScheduleNotFound)
            );
        }
        catch (Exception ex)
        {
            ScheduleFileLogger.LogError("Error retrieving schedule", ex);
            return Result.Failure<BusSchedule>(
                Error.Failure(ScheduleMessages.ErrorCodes.DatabaseError, ScheduleMessages.ErrorMessages.DatabaseError)
            );
        }
    }

    public async Task<Result<List<BusSchedule>>> GetByVendorIdAsync(int vendorId, int pageNumber, int pageSize)
    {
        try
        {
            ScheduleFileLogger.LogInfo(ScheduleMessages.LogMessages.GettingSchedulesByVendor, vendorId, pageNumber, pageSize);
            
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_GetSchedulesByVendor", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@VendorId", vendorId);
            command.Parameters.AddWithValue("@PageNumber", pageNumber);
            command.Parameters.AddWithValue("@PageSize", pageSize);
            
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            
            var schedules = new List<BusSchedule>();
            while (await reader.ReadAsync())
            {
                schedules.Add(new BusSchedule
                {
                    ScheduleId = reader.GetInt32(0),
                    BusId = reader.GetInt32(1),
                    RouteId = reader.GetInt32(2),
                    TravelDate = reader.GetDateTime(3),
                    DepartureTime = reader.GetTimeSpan(4),
                    ArrivalTime = reader.GetTimeSpan(5),
                    AvailableSeats = reader.GetInt32(6),
                    IsActive = reader.GetBoolean(7)
                });
            }
            
            ScheduleFileLogger.LogInfo(ScheduleMessages.LogMessages.SchedulesByVendorRetrievedSuccessfully, schedules.Count, vendorId);
            return Result.Success(schedules);
        }
        catch (Exception ex)
        {
            ScheduleFileLogger.LogError("Error getting schedules by vendor", ex);
            return Result.Failure<List<BusSchedule>>(
                Error.Failure(ScheduleMessages.ErrorCodes.DatabaseError, ScheduleMessages.ErrorMessages.DatabaseError)
            );
        }
    }

    public async Task<Result<int>> GetCountByVendorIdAsync(int vendorId)
    {
        try
        {
            ScheduleFileLogger.LogInfo(ScheduleMessages.LogMessages.GettingSchedulesByVendor, vendorId, 0, 0);
            
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_GetScheduleCountByVendor", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@VendorId", vendorId);
            
            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            var count = result != null ? (int)result : 0;
            
            ScheduleFileLogger.LogInfo(ScheduleMessages.LogMessages.SchedulesByVendorRetrievedSuccessfully, count, vendorId);
            return Result.Success(count);
        }
        catch (Exception ex)
        {
            ScheduleFileLogger.LogError("Error getting schedule count", ex);
            return Result.Failure<int>(
                Error.Failure(ScheduleMessages.ErrorCodes.DatabaseError, ScheduleMessages.ErrorMessages.DatabaseError)
            );
        }
    }

    public async Task<Result<BusSchedule>> UpdateAsync(BusSchedule schedule)
    {
        try
        {
            ScheduleFileLogger.LogInfo(ScheduleMessages.LogMessages.UpdatingSchedule, schedule.ScheduleId);
            
            schedule.UpdatedOn = DateTime.UtcNow;
            schedule.UpdatedBy = "System";
            
            _context.BusSchedules.Update(schedule);
            _context.Entry(schedule).Property(x => x.UpdatedBy).IsModified = true;
            _context.Entry(schedule).Property(x => x.UpdatedOn).IsModified = true;
            
            await _context.SaveChangesAsync();
            
            ScheduleFileLogger.LogInfo(ScheduleMessages.LogMessages.ScheduleUpdatedSuccessfully, schedule.ScheduleId);
            return Result.Success(schedule);
        }
        catch (Exception ex)
        {
            ScheduleFileLogger.LogError("Error updating schedule", ex);
            return Result.Failure<BusSchedule>(
                Error.Failure(ScheduleMessages.ErrorCodes.UpdateFailed, ScheduleMessages.ErrorMessages.DatabaseError)
            );
        }
    }

    public async Task<Result<bool>> DeleteAsync(int scheduleId)
    {
        try
        {
            ScheduleFileLogger.LogInfo(ScheduleMessages.LogMessages.DeletingSchedule, scheduleId);
            
            var schedule = await _context.BusSchedules.FindAsync(scheduleId);
            if (schedule == null)
            {
                ScheduleFileLogger.LogWarning(ScheduleMessages.LogMessages.ScheduleNotFound, scheduleId);
                return Result.Failure<bool>(
                    Error.NotFound(ScheduleMessages.ErrorCodes.ScheduleNotFound, ScheduleMessages.ErrorMessages.ScheduleNotFound)
                );
            }

            _context.BusSchedules.Remove(schedule);
            await _context.SaveChangesAsync();
            
            ScheduleFileLogger.LogInfo(ScheduleMessages.LogMessages.ScheduleDeletedSuccessfully, scheduleId);
            return Result.Success(true);
        }
        catch (Exception ex)
        {
            ScheduleFileLogger.LogError("Error deleting schedule", ex);
            return Result.Failure<bool>(
                Error.Failure(ScheduleMessages.ErrorCodes.DeletionFailed, ScheduleMessages.ErrorMessages.DatabaseError)
            );
        }
    }

    public async Task<Result<bool>> ExistsByBusRouteAndDateAsync(int busId, int routeId, DateTime travelDate)
    {
        try
        {
            ScheduleFileLogger.LogInfo(ScheduleMessages.LogMessages.CheckingScheduleExistence, busId, routeId, travelDate);
            
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_CheckScheduleExists", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@BusId", busId);
            command.Parameters.AddWithValue("@RouteId", routeId);
            command.Parameters.AddWithValue("@TravelDate", travelDate.Date);
            
            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            var exists = result != null && (bool)result;
            
            ScheduleFileLogger.LogInfo(ScheduleMessages.LogMessages.ScheduleExistenceChecked, exists, busId, routeId, travelDate);
            return Result.Success(exists);
        }
        catch (Exception ex)
        {
            ScheduleFileLogger.LogError("Error checking schedule existence", ex);
            return Result.Failure<bool>(
                Error.Failure(ScheduleMessages.ErrorCodes.DatabaseError, ScheduleMessages.ErrorMessages.DatabaseError)
            );
        }
    }
}