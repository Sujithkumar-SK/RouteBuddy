using Microsoft.EntityFrameworkCore;
using Kanini.RouteBuddy.Data.DatabaseContext;
using Kanini.RouteBuddy.Domain.Entities;

namespace Kanini.RouteBuddy.Data.Repositories.RouteStop;

public class RouteStopRepository : IRouteStopRepository
{
    private readonly RouteBuddyDatabaseContext _context;

    public RouteStopRepository(RouteBuddyDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Domain.Entities.RouteStop> CreateAsync(Domain.Entities.RouteStop routeStop)
    {
        _context.RouteStops.Add(routeStop);
        await _context.SaveChangesAsync();
        return routeStop;
    }

    public async Task<Domain.Entities.RouteStop?> GetByIdAsync(int routeStopId)
    {
        return await _context.RouteStops
            .Include(rs => rs.Stop)
            .Include(rs => rs.Route)
            .FirstOrDefaultAsync(rs => rs.RouteStopId == routeStopId);
    }

    public async Task<IEnumerable<Domain.Entities.RouteStop>> GetByRouteIdAsync(int routeId)
    {
        // Get only route-level template stops (ScheduleId = NULL)
        return await _context.RouteStops
            .Include(rs => rs.Stop)
            .Where(rs => rs.RouteId == routeId && rs.ScheduleId == null)
            .OrderBy(rs => rs.OrderNumber)
            .ToListAsync();
    }

    public async Task<Domain.Entities.RouteStop> UpdateAsync(Domain.Entities.RouteStop routeStop)
    {
        routeStop.UpdatedOn = DateTime.UtcNow;
        routeStop.UpdatedBy = "System"; // In real app, get from JWT token
        _context.RouteStops.Update(routeStop);
        await _context.SaveChangesAsync();
        return routeStop;
    }

    public async Task<bool> DeleteAsync(int routeStopId)
    {
        var routeStop = await _context.RouteStops.FindAsync(routeStopId);
        if (routeStop == null) return false;

        _context.RouteStops.Remove(routeStop);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsByRouteAndOrderAsync(int routeId, int orderNumber)
    {
        // Check only route-level template stops (ScheduleId = NULL)
        return await _context.RouteStops
            .AnyAsync(rs => rs.RouteId == routeId && rs.OrderNumber == orderNumber && rs.ScheduleId == null);
    }

    public async Task<IEnumerable<Domain.Entities.RouteStop>> GetByScheduleIdAsync(int scheduleId)
    {
        // Get schedule-specific stops with timings
        return await _context.RouteStops
            .Include(rs => rs.Stop)
            .Where(rs => rs.ScheduleId == scheduleId)
            .OrderBy(rs => rs.OrderNumber)
            .ToListAsync();
    }
}