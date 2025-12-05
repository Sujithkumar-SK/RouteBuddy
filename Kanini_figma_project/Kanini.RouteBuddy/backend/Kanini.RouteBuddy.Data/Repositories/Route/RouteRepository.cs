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
            using var command = new SqlCommand("sp_GetRouteById", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@RouteId", routeId);
            
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            
            if (await reader.ReadAsync())
            {
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
            using var command = new SqlCommand("sp_GetAllRoutes", connection);
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
}