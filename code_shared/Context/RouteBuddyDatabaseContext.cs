using Kanini.RouteBuddy.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kanini.RouteBuddy.Data.DatabaseContext;

public class RouteBuddyDatabaseContext(DbContextOptions<RouteBuddyDatabaseContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Vendor> Vendors { get; set; }
    public DbSet<Bus> Buses { get; set; }
    public DbSet<Route> Routes { get; set; }
    public DbSet<Stop> Stops { get; set; }
    public DbSet<BusSchedule> BusSchedules { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<BusPhoto> BusPhotos { get; set; }
    public DbSet<BookedSeat> BookedSeats { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<BookingSegment> BookingSegments { get; set; }
    public DbSet<Refund> Refunds { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Cancellation> Cancellations { get; set; }
    public DbSet<Fare> Fares { get; set; }

    public DbSet<Driver> Drivers { get; set; }
    public DbSet<DriverAssignment> DriverAssignments { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RouteBuddyDatabaseContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
