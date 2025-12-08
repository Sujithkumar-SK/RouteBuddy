using Microsoft.Data.SqlClient;
using Kanini.RouteBuddy.Common.Errors;
using Microsoft.Extensions.Configuration;
using Kanini.RouteBuddy.Common.Utility;
using Microsoft.Extensions.Logging;

namespace Kanini.RouteBuddy.Data.Repositories.VendorAnalytics;

public class VendorAnalyticsRepository : IVendorAnalyticsRepository
{
    private readonly string _connectionString;
    private readonly ILogger<VendorAnalyticsRepository> _logger;

    public VendorAnalyticsRepository(IConfiguration configuration, ILogger<VendorAnalyticsRepository> logger)
    {
        _connectionString = configuration.GetConnectionString("DatabaseConnectionString")!;
        _logger = logger;
    }

    public async Task<Result<object>> GetRevenueAnalyticsAsync(int vendorId)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            
            using var command = new SqlCommand("sp_GetVendorRevenueAnalytics", connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@VendorId", vendorId);
            
            using var reader = await command.ExecuteReaderAsync();
            
            if (await reader.ReadAsync())
            {
                var result = new {
                    TotalRevenue = reader.IsDBNull(reader.GetOrdinal("TotalRevenue")) ? 0m : reader.GetDecimal(reader.GetOrdinal("TotalRevenue")),
                    MonthlyRevenue = reader.IsDBNull(reader.GetOrdinal("MonthlyRevenue")) ? 0m : reader.GetDecimal(reader.GetOrdinal("MonthlyRevenue")),
                    WeeklyRevenue = reader.IsDBNull(reader.GetOrdinal("WeeklyRevenue")) ? 0m : reader.GetDecimal(reader.GetOrdinal("WeeklyRevenue"))
                };
                
                _logger.LogInformation(VendorAnalyticsMessages.LogMessages.RevenueAnalyticsRetrieved, vendorId);
                return Result.Success<object>(result);
            }
            
            var emptyResult = new { TotalRevenue = 0m, MonthlyRevenue = 0m, WeeklyRevenue = 0m };
            _logger.LogInformation(VendorAnalyticsMessages.LogMessages.RevenueAnalyticsRetrieved, vendorId);
            return Result.Success<object>(emptyResult);
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "Database error occurred while getting revenue analytics for vendor {VendorId}", vendorId);
            return Result.Failure<object>(Error.Failure(VendorAnalyticsMessages.ErrorCodes.DatabaseError, VendorAnalyticsMessages.ErrorMessages.DatabaseError));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while getting revenue analytics for vendor {VendorId}", vendorId);
            return Result.Failure<object>(Error.Failure(VendorAnalyticsMessages.ErrorCodes.UnexpectedError, VendorAnalyticsMessages.ErrorMessages.UnexpectedError));
        }
    }

    public async Task<Result<object>> GetPerformanceMetricsAsync(int vendorId)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            
            using var command = new SqlCommand("sp_GetVendorPerformanceMetrics", connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@VendorId", vendorId);
            
            using var reader = await command.ExecuteReaderAsync();
            
            if (await reader.ReadAsync())
            {
                var result = new {
                    MonthlyBookings = reader.IsDBNull(reader.GetOrdinal("MonthlyBookings")) ? 0 : reader.GetInt32(reader.GetOrdinal("MonthlyBookings")),
                    OnTimePerformance = reader.IsDBNull(reader.GetOrdinal("OnTimePerformance")) ? 0.0 : Convert.ToDouble(reader.GetDecimal(reader.GetOrdinal("OnTimePerformance")))
                };
                
                _logger.LogInformation(VendorAnalyticsMessages.LogMessages.PerformanceMetricsRetrieved, vendorId);
                return Result.Success<object>(result);
            }
            
            var emptyResult = new { MonthlyBookings = 0, OnTimePerformance = 0.0 };
            _logger.LogInformation(VendorAnalyticsMessages.LogMessages.PerformanceMetricsRetrieved, vendorId);
            return Result.Success<object>(emptyResult);
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "Database error occurred while getting performance metrics for vendor {VendorId}", vendorId);
            return Result.Failure<object>(Error.Failure(VendorAnalyticsMessages.ErrorCodes.DatabaseError, VendorAnalyticsMessages.ErrorMessages.DatabaseError));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while getting performance metrics for vendor {VendorId}", vendorId);
            return Result.Failure<object>(Error.Failure(VendorAnalyticsMessages.ErrorCodes.UnexpectedError, VendorAnalyticsMessages.ErrorMessages.UnexpectedError));
        }
    }

    public async Task<Result<object>> GetFleetStatusAsync(int vendorId)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            
            using var command = new SqlCommand("sp_GetVendorFleetStatus", connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@VendorId", vendorId);
            
            using var reader = await command.ExecuteReaderAsync();
            
            if (await reader.ReadAsync())
            {
                var totalBuses = reader.IsDBNull(reader.GetOrdinal("TotalBuses")) ? 0 : reader.GetInt32(reader.GetOrdinal("TotalBuses"));
                var activeBuses = reader.IsDBNull(reader.GetOrdinal("ActiveBuses")) ? 0 : reader.GetInt32(reader.GetOrdinal("ActiveBuses"));
                var maintenanceBuses = reader.IsDBNull(reader.GetOrdinal("MaintenanceBuses")) ? 0 : reader.GetInt32(reader.GetOrdinal("MaintenanceBuses"));
                
                var result = new {
                    TotalBuses = totalBuses,
                    ActiveBuses = activeBuses,
                    MaintenanceBuses = maintenanceBuses,
                    IdleBuses = totalBuses - activeBuses - maintenanceBuses
                };
                
                _logger.LogInformation(VendorAnalyticsMessages.LogMessages.FleetStatusRetrieved, vendorId);
                return Result.Success<object>(result);
            }
            
            var emptyResult = new { TotalBuses = 0, ActiveBuses = 0, MaintenanceBuses = 0, IdleBuses = 0 };
            _logger.LogInformation(VendorAnalyticsMessages.LogMessages.FleetStatusRetrieved, vendorId);
            return Result.Success<object>(emptyResult);
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "Database error occurred while getting fleet status for vendor {VendorId}", vendorId);
            return Result.Failure<object>(Error.Failure(VendorAnalyticsMessages.ErrorCodes.DatabaseError, VendorAnalyticsMessages.ErrorMessages.DatabaseError));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while getting fleet status for vendor {VendorId}", vendorId);
            return Result.Failure<object>(Error.Failure(VendorAnalyticsMessages.ErrorCodes.UnexpectedError, VendorAnalyticsMessages.ErrorMessages.UnexpectedError));
        }
    }

    public async Task<Result<List<object>>> GetNotificationsAsync(int vendorId)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            
            using var command = new SqlCommand("sp_GetVendorNotifications", connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@VendorId", vendorId);
            
            using var reader = await command.ExecuteReaderAsync();
            
            var notifications = new List<object>();
            while (await reader.ReadAsync())
            {
                notifications.Add(new {
                    Type = reader.IsDBNull(reader.GetOrdinal("Type")) ? "" : reader.GetString(reader.GetOrdinal("Type")),
                    Message = reader.IsDBNull(reader.GetOrdinal("Message")) ? "" : reader.GetString(reader.GetOrdinal("Message")),
                    Time = reader.IsDBNull(reader.GetOrdinal("Time")) ? DateTime.Now : reader.GetDateTime(reader.GetOrdinal("Time"))
                });
            }
            
            _logger.LogInformation(VendorAnalyticsMessages.LogMessages.NotificationsRetrieved, vendorId);
            return Result.Success(notifications);
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "Database error occurred while getting notifications for vendor {VendorId}", vendorId);
            return Result.Failure<List<object>>(Error.Failure(VendorAnalyticsMessages.ErrorCodes.DatabaseError, VendorAnalyticsMessages.ErrorMessages.DatabaseError));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while getting notifications for vendor {VendorId}", vendorId);
            return Result.Failure<List<object>>(Error.Failure(VendorAnalyticsMessages.ErrorCodes.UnexpectedError, VendorAnalyticsMessages.ErrorMessages.UnexpectedError));
        }
    }

    public async Task<Result<object>> GetMaintenanceScheduleAsync(int vendorId)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            
            using var command = new SqlCommand("sp_GetVendorMaintenanceSchedule", connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@VendorId", vendorId);
            
            using var reader = await command.ExecuteReaderAsync();
            
            var upcomingMaintenance = new List<object>();
            while (await reader.ReadAsync())
            {
                upcomingMaintenance.Add(new {
                    BusNumber = reader.IsDBNull(reader.GetOrdinal("BusNumber")) ? "" : reader.GetString(reader.GetOrdinal("BusNumber")),
                    Type = reader.IsDBNull(reader.GetOrdinal("Type")) ? "" : reader.GetString(reader.GetOrdinal("Type")),
                    DueDate = reader.IsDBNull(reader.GetOrdinal("DueDate")) ? DateTime.Now : reader.GetDateTime(reader.GetOrdinal("DueDate")),
                    Priority = reader.IsDBNull(reader.GetOrdinal("Priority")) ? "" : reader.GetString(reader.GetOrdinal("Priority"))
                });
            }
            
            var result = new {
                UpcomingMaintenance = upcomingMaintenance,
                MaintenanceHistory = new List<object>()
            };
            
            _logger.LogInformation(VendorAnalyticsMessages.LogMessages.MaintenanceScheduleRetrieved, vendorId);
            return Result.Success<object>(result);
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "Database error occurred while getting maintenance schedule for vendor {VendorId}", vendorId);
            return Result.Failure<object>(Error.Failure(VendorAnalyticsMessages.ErrorCodes.DatabaseError, VendorAnalyticsMessages.ErrorMessages.DatabaseError));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while getting maintenance schedule for vendor {VendorId}", vendorId);
            return Result.Failure<object>(Error.Failure(VendorAnalyticsMessages.ErrorCodes.UnexpectedError, VendorAnalyticsMessages.ErrorMessages.UnexpectedError));
        }
    }

    public async Task<Result<object>> GetRecentBookingsAsync(int vendorId)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            
            using var command = new SqlCommand("sp_GetVendorRecentBookings", connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@VendorId", vendorId);
            
            using var reader = await command.ExecuteReaderAsync();
            
            var bookings = new List<object>();
            while (await reader.ReadAsync())
            {
                bookings.Add(new {
                    BookingId = reader.IsDBNull(reader.GetOrdinal("BookingId")) ? 0 : reader.GetInt32(reader.GetOrdinal("BookingId")),
                    CustomerName = reader.IsDBNull(reader.GetOrdinal("CustomerName")) ? "" : reader.GetString(reader.GetOrdinal("CustomerName")),
                    Route = reader.IsDBNull(reader.GetOrdinal("Route")) ? "" : reader.GetString(reader.GetOrdinal("Route")),
                    BookingDate = reader.IsDBNull(reader.GetOrdinal("BookingDate")) ? DateTime.Now : reader.GetDateTime(reader.GetOrdinal("BookingDate")),
                    Status = reader.IsDBNull(reader.GetOrdinal("Status")) ? "" : reader.GetString(reader.GetOrdinal("Status"))
                });
            }
            
            _logger.LogInformation(VendorAnalyticsMessages.LogMessages.RecentBookingsRetrieved, vendorId);
            return Result.Success<object>(bookings);
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "Database error occurred while getting recent bookings for vendor {VendorId}", vendorId);
            return Result.Failure<object>(Error.Failure(VendorAnalyticsMessages.ErrorCodes.DatabaseError, VendorAnalyticsMessages.ErrorMessages.DatabaseError));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while getting recent bookings for vendor {VendorId}", vendorId);
            return Result.Failure<object>(Error.Failure(VendorAnalyticsMessages.ErrorCodes.UnexpectedError, VendorAnalyticsMessages.ErrorMessages.UnexpectedError));
        }
    }

    public async Task<Result<object>> GetQuickStatsAsync(int vendorId)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            
            using var command = new SqlCommand("sp_GetVendorQuickStats", connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@VendorId", vendorId);
            
            using var reader = await command.ExecuteReaderAsync();
            
            if (await reader.ReadAsync())
            {
                var result = new {
                    TotalBookings = reader.IsDBNull(reader.GetOrdinal("TotalBookings")) ? 0 : reader.GetInt32(reader.GetOrdinal("TotalBookings")),
                    ActiveRoutes = reader.IsDBNull(reader.GetOrdinal("ActiveRoutes")) ? 0 : reader.GetInt32(reader.GetOrdinal("ActiveRoutes")),
                    TotalRevenue = reader.IsDBNull(reader.GetOrdinal("TotalRevenue")) ? 0m : reader.GetDecimal(reader.GetOrdinal("TotalRevenue"))
                };
                
                _logger.LogInformation(VendorAnalyticsMessages.LogMessages.QuickStatsRetrieved, vendorId);
                return Result.Success<object>(result);
            }
            
            var emptyResult = new { TotalBookings = 0, ActiveRoutes = 0, TotalRevenue = 0m };
            _logger.LogInformation(VendorAnalyticsMessages.LogMessages.QuickStatsRetrieved, vendorId);
            return Result.Success<object>(emptyResult);
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "Database error occurred while getting quick stats for vendor {VendorId}", vendorId);
            return Result.Failure<object>(Error.Failure(VendorAnalyticsMessages.ErrorCodes.DatabaseError, VendorAnalyticsMessages.ErrorMessages.DatabaseError));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while getting quick stats for vendor {VendorId}", vendorId);
            return Result.Failure<object>(Error.Failure(VendorAnalyticsMessages.ErrorCodes.UnexpectedError, VendorAnalyticsMessages.ErrorMessages.UnexpectedError));
        }
    }

    public async Task<Result<object>> GetAlertsAsync(int vendorId)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            
            using var command = new SqlCommand("sp_GetVendorAlerts", connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@VendorId", vendorId);
            
            using var reader = await command.ExecuteReaderAsync();
            
            var alerts = new List<object>();
            while (await reader.ReadAsync())
            {
                alerts.Add(new {
                    AlertId = reader.IsDBNull(reader.GetOrdinal("AlertId")) ? 0 : Convert.ToInt32(reader.GetValue(reader.GetOrdinal("AlertId"))),
                    Type = reader.IsDBNull(reader.GetOrdinal("Type")) ? "" : reader.GetString(reader.GetOrdinal("Type")),
                    Message = reader.IsDBNull(reader.GetOrdinal("Message")) ? "" : reader.GetString(reader.GetOrdinal("Message")),
                    Severity = reader.IsDBNull(reader.GetOrdinal("Severity")) ? "" : reader.GetString(reader.GetOrdinal("Severity")),
                    CreatedAt = reader.IsDBNull(reader.GetOrdinal("CreatedAt")) ? DateTime.Now : reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
                });
            }
            
            _logger.LogInformation(VendorAnalyticsMessages.LogMessages.AlertsRetrieved, vendorId);
            return Result.Success<object>(alerts);
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "Database error occurred while getting alerts for vendor {VendorId}", vendorId);
            return Result.Failure<object>(Error.Failure(VendorAnalyticsMessages.ErrorCodes.DatabaseError, VendorAnalyticsMessages.ErrorMessages.DatabaseError));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while getting alerts for vendor {VendorId}", vendorId);
            return Result.Failure<object>(Error.Failure(VendorAnalyticsMessages.ErrorCodes.UnexpectedError, VendorAnalyticsMessages.ErrorMessages.UnexpectedError));
        }
    }


}