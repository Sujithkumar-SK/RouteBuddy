using Microsoft.EntityFrameworkCore;
using Kanini.RouteBuddy.Data.DatabaseContext;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Kanini.RouteBuddy.Common.Utility;
using Kanini.RouteBuddy.Domain.Entities;
using Kanini.RouteBuddy.Common.Services;
using Kanini.RouteBuddy.Common.Errors;
using Kanini.RouteBuddy.Common;
using Kanini.RouteBuddy.Data.Models;

namespace Kanini.RouteBuddy.Data.Repositories.Route;

public class RouteRepository : IRouteRepository
{
    private readonly RouteBuddyDatabaseContext _context;
    private readonly string _connectionString;
    private readonly ILogger<RouteRepository> _logger;

    public RouteRepository(RouteBuddyDatabaseContext context, IConfiguration configuration, ILogger<RouteRepository> logger)
    {
        _context = context;
        _connectionString = configuration.GetConnectionString("DatabaseConnectionString")!;
        _logger = logger;
    }

    public async Task<Result<Domain.Entities.Route>> CreateAsync(Domain.Entities.Route route)
    {
        try
        {
            RouteFileLogger.LogInfo("Creating route: {0} to {1}", route.Source, route.Destination);
            
            _context.Routes.Add(route);
            await _context.SaveChangesAsync();
            
            RouteFileLogger.LogInfo("Route created successfully with ID: {0}", route.RouteId);
            return Result.Success(route);
        }
        catch (Exception ex)
        {
            RouteFileLogger.LogError("Error creating route", ex);
            return Result.Failure<Domain.Entities.Route>(
                Error.Failure("Route.CreationFailed", RouteMessages.ErrorMessages.DatabaseError)
            );
        }
    }

    public async Task<Result<Domain.Entities.Route>> GetByIdAsync(int routeId)
    {
        try
        {
            RouteFileLogger.LogInfo("Getting route by ID: {0}", routeId);
            
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(MagicStrings.StoredProcedures.GetRouteById, connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@RouteId", routeId);
            
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            
            // Read first result set (route data)
            if (await reader.ReadAsync())
            {
                // Check if it's an error response
                if (reader.FieldCount == 2 && reader.GetName(0) == "Result")
                {
                    RouteFileLogger.LogWarning("Route not found: {0}", routeId);
                    return Result.Failure<Domain.Entities.Route>(
                        Error.NotFound("Route.NotFound", RouteMessages.ErrorMessages.RouteNotFound)
                    );
                }
                
                var route = new Domain.Entities.Route
                {
                    RouteId = reader.GetInt32("RouteId"),
                    Source = reader.GetString("Source"),
                    Destination = reader.GetString("Destination"),
                    Distance = reader.GetDecimal("Distance"),
                    Duration = reader.IsDBNull(4) ? TimeSpan.Zero : reader.GetTimeSpan(4),
                    BasePrice = reader.GetDecimal("BasePrice"),
                    IsActive = reader.GetBoolean("IsActive")
                };
                
                RouteFileLogger.LogInfo("Route retrieved successfully: {0}", routeId);
                return Result.Success(route);
            }
            
            RouteFileLogger.LogWarning("Route not found: {0}", routeId);
            return Result.Failure<Domain.Entities.Route>(
                Error.NotFound("Route.NotFound", RouteMessages.ErrorMessages.RouteNotFound)
            );
        }
        catch (Exception ex)
        {
            RouteFileLogger.LogError("Error retrieving route", ex);
            return Result.Failure<Domain.Entities.Route>(
                Error.Failure("Route.RetrievalFailed", RouteMessages.ErrorMessages.DatabaseError)
            );
        }
    }

    public async Task<Result<List<Domain.Entities.Route>>> GetAllAsync(int pageNumber, int pageSize)
    {
        try
        {
            RouteFileLogger.LogInfo("Getting all routes, page: {0}, size: {1}", pageNumber, pageSize);
            
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(MagicStrings.StoredProcedures.GetAllRoutes, connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@PageNumber", pageNumber);
            command.Parameters.AddWithValue("@PageSize", pageSize);
            
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            
            var routes = new List<Domain.Entities.Route>();
            while (await reader.ReadAsync())
            {
                routes.Add(new Domain.Entities.Route
                {
                    RouteId = reader.GetInt32("RouteId"),
                    Source = reader.GetString("Source"),
                    Destination = reader.GetString("Destination"),
                    Distance = reader.GetDecimal("Distance"),
                    Duration = reader.IsDBNull(4) ? TimeSpan.Zero : reader.GetTimeSpan(4),
                    BasePrice = reader.GetDecimal("BasePrice"),
                    IsActive = reader.GetBoolean("IsActive")
                });
            }
            
            RouteFileLogger.LogInfo("Retrieved {0} routes", routes.Count);
            return Result.Success(routes);
        }
        catch (Exception ex)
        {
            RouteFileLogger.LogError("Error getting all routes", ex);
            return Result.Failure<List<Domain.Entities.Route>>(
                Error.Failure("Route.ListFailed", RouteMessages.ErrorMessages.DatabaseError)
            );
        }
    }

    public async Task<Result<int>> GetTotalCountAsync()
    {
        try
        {
            var count = await _context.Routes.CountAsync(r => r.IsActive);
            return Result.Success(count);
        }
        catch (Exception ex)
        {
            RouteFileLogger.LogError("Error getting route count", ex);
            return Result.Failure<int>(
                Error.Failure("Route.CountFailed", RouteMessages.ErrorMessages.DatabaseError)
            );
        }
    }

    public async Task<Result<Domain.Entities.Route>> UpdateAsync(Domain.Entities.Route route)
    {
        try
        {
            RouteFileLogger.LogInfo("Updating route: {0}", route.RouteId);
            
            route.UpdatedOn = DateTime.UtcNow;
            route.UpdatedBy = "System";
            
            _context.Routes.Update(route);
            _context.Entry(route).Property(x => x.UpdatedBy).IsModified = true;
            _context.Entry(route).Property(x => x.UpdatedOn).IsModified = true;
            
            await _context.SaveChangesAsync();
            
            RouteFileLogger.LogInfo("Route updated successfully: {0}", route.RouteId);
            return Result.Success(route);
        }
        catch (Exception ex)
        {
            RouteFileLogger.LogError("Error updating route", ex);
            return Result.Failure<Domain.Entities.Route>(
                Error.Failure("Route.UpdateFailed", RouteMessages.ErrorMessages.DatabaseError)
            );
        }
    }

    public async Task<Result<bool>> DeleteAsync(int routeId)
    {
        try
        {
            RouteFileLogger.LogInfo("Deleting route: {0}", routeId);
            
            var route = await _context.Routes.FindAsync(routeId);
            if (route == null)
            {
                RouteFileLogger.LogWarning("Route not found for deletion: {0}", routeId);
                return Result.Failure<bool>(
                    Error.NotFound("Route.NotFound", RouteMessages.ErrorMessages.RouteNotFound)
                );
            }

            _context.Routes.Remove(route);
            await _context.SaveChangesAsync();
            
            RouteFileLogger.LogInfo("Route deleted successfully: {0}", routeId);
            return Result.Success(true);
        }
        catch (Exception ex)
        {
            RouteFileLogger.LogError("Error deleting route", ex);
            return Result.Failure<bool>(
                Error.Failure("Route.DeleteFailed", RouteMessages.ErrorMessages.DatabaseError)
            );
        }
    }

    public async Task<Result<bool>> ExistsBySourceDestinationAsync(string source, string destination)
    {
        try
        {
            var exists = await _context.Routes
                .AnyAsync(r => r.Source == source && r.Destination == destination && r.IsActive);
            return Result.Success(exists);
        }
        catch (Exception ex)
        {
            RouteFileLogger.LogError("Error checking route exists", ex);
            return Result.Failure<bool>(
                Error.Failure("Route.ExistsFailed", RouteMessages.ErrorMessages.DatabaseError)
            );
        }
    }

    public async Task<Result<List<Domain.Entities.RouteStop>>> GetRouteStopsAsync(int routeId)
    {
        try
        {
            RouteFileLogger.LogInfo("Getting route stops for route: {0}", routeId);
            
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_GetRouteStops", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@RouteId", routeId);
            
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            
            var routeStops = new List<Domain.Entities.RouteStop>();
            while (await reader.ReadAsync())
            {
                routeStops.Add(new Domain.Entities.RouteStop
                {
                    RouteStopId = reader.GetInt32("RouteStopId"),
                    StopId = reader.GetInt32("StopId"),
                    OrderNumber = reader.GetInt32("OrderNumber"),
                    ArrivalTime = reader.IsDBNull(reader.GetOrdinal("ArrivalTime")) ? null : reader.GetTimeSpan(reader.GetOrdinal("ArrivalTime")),
                    DepartureTime = reader.IsDBNull(reader.GetOrdinal("DepartureTime")) ? null : reader.GetTimeSpan(reader.GetOrdinal("DepartureTime")),
                    Stop = new Domain.Entities.Stop
                    {
                        StopId = reader.GetInt32("StopId"),
                        Name = reader.GetString("StopName"),
                        Landmark = reader.IsDBNull("Landmark") ? null : reader.GetString("Landmark")
                    }
                });
            }
            
            RouteFileLogger.LogInfo("Retrieved {0} route stops", routeStops.Count);
            return Result.Success(routeStops);
        }
        catch (Exception ex)
        {
            RouteFileLogger.LogError("Error getting route stops", ex);
            return Result.Failure<List<Domain.Entities.RouteStop>>(
                Error.Failure("RouteStops.Failed", RouteMessages.ErrorMessages.DatabaseError)
            );
        }
    }

    public async Task<Result<List<RouteSearchResult>>> SearchRoutesAsync(string? source, string? destination, int limit)
    {
        try
        {
            RouteFileLogger.LogInfo("Searching routes - Source: {0}, Destination: {1}, Limit: {2}", source ?? "null", destination ?? "null", limit);
            
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_SearchRoutes", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@Source", (object?)source ?? DBNull.Value);
            command.Parameters.AddWithValue("@Destination", (object?)destination ?? DBNull.Value);
            command.Parameters.AddWithValue("@Limit", limit);
            
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            
            var routes = new List<RouteSearchResult>();
            while (await reader.ReadAsync())
            {
                routes.Add(new RouteSearchResult
                {
                    RouteId = reader.GetInt32("RouteId"),
                    Source = reader.GetString("Source"),
                    Destination = reader.GetString("Destination"),
                    Distance = reader.GetDecimal("Distance"),
                    Duration = reader.GetTimeSpan(reader.GetOrdinal("Duration")),
                    BasePrice = reader.GetDecimal("BasePrice"),
                    TotalStops = reader.GetInt32("TotalStops"),
                    IsActive = reader.GetBoolean("IsActive")
                });
            }
            
            RouteFileLogger.LogInfo("Route search completed: {0} routes found", routes.Count);
            return Result.Success(routes);
        }
        catch (Exception ex)
        {
            RouteFileLogger.LogError("Error searching routes", ex);
            return Result.Failure<List<RouteSearchResult>>(
                Error.Failure("RouteSearch.Failed", "Route search failed")
            );
        }
    }

    public async Task<Result<List<RouteSearchResult>>> GetAllActiveRoutesAsync()
    {
        try
        {
            RouteFileLogger.LogInfo("Getting all active routes for schedule creation");
            
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(MagicStrings.StoredProcedures.GetAllActiveRoutes, connection);
            command.CommandType = CommandType.StoredProcedure;
            
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            
            var routes = new List<RouteSearchResult>();
            while (await reader.ReadAsync())
            {
                routes.Add(new RouteSearchResult
                {
                    RouteId = reader.GetInt32("RouteId"),
                    Source = reader.GetString("Source"),
                    Destination = reader.GetString("Destination"),
                    Distance = reader.GetDecimal("Distance"),
                    Duration = reader.GetTimeSpan(reader.GetOrdinal("Duration")),
                    BasePrice = reader.GetDecimal("BasePrice"),
                    TotalStops = reader.GetInt32("TotalStops"),
                    IsActive = reader.GetBoolean("IsActive")
                });
            }
            
            RouteFileLogger.LogInfo("Retrieved {0} active routes", routes.Count);
            return Result.Success(routes);
        }
        catch (Exception ex)
        {
            RouteFileLogger.LogError("Error getting all active routes", ex);
            return Result.Failure<List<RouteSearchResult>>(
                Error.Failure("AllActiveRoutes.Failed", "Failed to retrieve all active routes")
            );
        }
    }

    public async Task<Result<List<RouteStopDetailResult>>> GetRouteStopsWithDetailsAsync(int routeId)
    {
        try
        {
            RouteFileLogger.LogInfo("Getting detailed route stops for route: {0}", routeId);
            
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_GetRouteStopsWithDetails", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@RouteId", routeId);
            
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            
            var routeStops = new List<RouteStopDetailResult>();
            while (await reader.ReadAsync())
            {
                routeStops.Add(new RouteStopDetailResult
                {
                    RouteStopId = reader.GetInt32("RouteStopId"),
                    StopId = reader.GetInt32("StopId"),
                    StopName = reader.GetString("StopName"),
                    Landmark = reader.IsDBNull("Landmark") ? null : reader.GetString("Landmark"),
                    OrderNumber = reader.GetInt32("OrderNumber"),
                    ArrivalTime = reader.IsDBNull(reader.GetOrdinal("ArrivalTime")) ? null : reader.GetTimeSpan(reader.GetOrdinal("ArrivalTime")),
                    DepartureTime = reader.IsDBNull(reader.GetOrdinal("DepartureTime")) ? null : reader.GetTimeSpan(reader.GetOrdinal("DepartureTime"))
                });
            }
            
            RouteFileLogger.LogInfo("Retrieved {0} detailed route stops", routeStops.Count);
            return Result.Success(routeStops);
        }
        catch (Exception ex)
        {
            RouteFileLogger.LogError("Error getting detailed route stops", ex);
            return Result.Failure<List<RouteStopDetailResult>>(
                Error.Failure("RouteStopsDetail.Failed", "Route stops details retrieval failed")
            );
        }
    }

    public async Task<Result<List<Domain.Entities.Stop>>> GetAllActiveStopsAsync()
    {
        try
        {
            RouteFileLogger.LogInfo("Getting all active stops");
            
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(MagicStrings.StoredProcedures.GetAllActiveStops, connection);
            command.CommandType = CommandType.StoredProcedure;
            
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            
            var stops = new List<Domain.Entities.Stop>();
            while (await reader.ReadAsync())
            {
                stops.Add(new Domain.Entities.Stop
                {
                    StopId = reader.GetInt32("StopId"),
                    Name = reader.GetString("Name"),
                    Landmark = reader.IsDBNull("Landmark") ? null : reader.GetString("Landmark"),
                    IsActive = reader.GetBoolean("IsActive")
                });
            }
            
            RouteFileLogger.LogInfo("Retrieved {0} active stops", stops.Count);
            return Result.Success(stops);
        }
        catch (Exception ex)
        {
            RouteFileLogger.LogError("Error getting all active stops", ex);
            return Result.Failure<List<Domain.Entities.Stop>>(
                Error.Failure("AllActiveStops.Failed", "Failed to retrieve all active stops")
            );
        }
    }

    public async Task<Result<bool>> CreateRouteStopsAsync(int routeId, List<RouteStopCreationModel> stops, int vendorId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            RouteFileLogger.LogInfo("Creating route-level template stops for route: {0}", routeId);
            
            // Check if route template stops already exist
            var existingStops = await _context.RouteStops
                .Where(rs => rs.RouteId == routeId && rs.ScheduleId == null)
                .ToListAsync();
            
            if (existingStops.Any())
            {
                RouteFileLogger.LogInfo("Route template stops already exist for route: {0}", routeId);
                return Result.Success(true); // Don't overwrite existing template stops
            }
            
            // Create new route-level template stops
            foreach (var stop in stops)
            {
                var routeStop = new Domain.Entities.RouteStop
                {
                    RouteId = routeId,
                    StopId = stop.StopId,
                    OrderNumber = stop.OrderNumber,
                    ArrivalTime = null, // No timing at route level
                    DepartureTime = null, // No timing at route level
                    ScheduleId = null, // Route-level template
                    CreatedBy = $"Vendor-{vendorId}",
                    CreatedOn = DateTime.UtcNow
                };
                
                _context.RouteStops.Add(routeStop);
                RouteFileLogger.LogInfo("Created route template stop: StopId {0}", stop.StopId);
            }
            
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            
            RouteFileLogger.LogInfo("Created {0} route template stops for route: {1}", stops.Count, routeId);
            return Result.Success(true);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            RouteFileLogger.LogError("Error creating route template stops", ex);
            return Result.Failure<bool>(
                Error.Failure("CreateRouteStops.Failed", "Failed to create route template stops")
            );
        }
    }

    public async Task<Result<bool>> CreateScheduleRouteStopsAsync(int scheduleId, List<RouteStopCreationModel> stops)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            RouteFileLogger.LogInfo("Creating schedule-specific stops for schedule: {0}", scheduleId);
            
            // Get schedule to find RouteId
            var schedule = await _context.BusSchedules.FindAsync(scheduleId);
            if (schedule == null)
            {
                return Result.Failure<bool>(Error.NotFound("Schedule.NotFound", "Schedule not found"));
            }
            
            // Check if schedule stops already exist
            var existingScheduleStops = await _context.RouteStops
                .Where(rs => rs.ScheduleId == scheduleId)
                .ToListAsync();
            
            if (existingScheduleStops.Any())
            {
                RouteFileLogger.LogInfo("Schedule stops already exist for schedule: {0}", scheduleId);
                return Result.Success(true); // Don't overwrite existing stops
            }
            
            // Create new schedule-specific stops with timings
            foreach (var stop in stops)
            {
                var routeStop = new Domain.Entities.RouteStop
                {
                    RouteId = schedule.RouteId,
                    StopId = stop.StopId,
                    OrderNumber = stop.OrderNumber,
                    ArrivalTime = stop.ArrivalTime,
                    DepartureTime = stop.DepartureTime,
                    ScheduleId = scheduleId, // Schedule-specific
                    CreatedBy = "Schedule-System",
                    CreatedOn = DateTime.UtcNow
                };
                
                _context.RouteStops.Add(routeStop);
                RouteFileLogger.LogInfo("Created schedule stop: StopId {0} for Schedule {1}", stop.StopId, scheduleId);
            }
            
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            
            RouteFileLogger.LogInfo("Created {0} schedule stops for schedule: {1}", stops.Count, scheduleId);
            return Result.Success(true);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            RouteFileLogger.LogError("Error creating schedule stops", ex);
            return Result.Failure<bool>(
                Error.Failure("CreateScheduleStops.Failed", "Failed to create schedule stops")
            );
        }
    }
}