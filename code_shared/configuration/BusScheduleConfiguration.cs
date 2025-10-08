using Kanini.RouteBuddy.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kanini.RouteBuddy.Data.Configurations;

public class BusScheduleConfiguration : IEntityTypeConfiguration<BusSchedule>
{
  public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<BusSchedule> builder)
  {
    builder.HasOne(s => s.Bus)
          .WithMany(b => b.Schedules)
          .HasForeignKey(s => s.BusId);

    builder.HasOne(s => s.Route)
        .WithMany(r => r.Schedules)
        .HasForeignKey(s => s.RouteId);

    builder.HasMany(s => s.Segments)
        .WithOne(bs => bs.Schedule)
        .HasForeignKey(bs => bs.ScheduleId)
        .OnDelete(DeleteBehavior.Restrict);

    builder.HasMany(s => s.DriverAssignments)
        .WithOne(da => da.Schedule)
        .HasForeignKey(da => da.ScheduleId);

    builder.HasMany(s => s.Fares)
        .WithOne(f => f.Schedule)
        .HasForeignKey(f => f.ScheduleId)
        .OnDelete(DeleteBehavior.Cascade);

    builder.HasMany(s => s.Bookings)
        .WithOne(b => b.Schedule)
        .HasForeignKey(b => b.ScheduleId)
        .OnDelete(DeleteBehavior.Restrict);
  }
}
