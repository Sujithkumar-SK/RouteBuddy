using Kanini.RouteBuddy.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kanini.RouteBuddy.Data.Configurations;

public class BusScheduleConfiguration : IEntityTypeConfiguration<BusSchedule>
{
    public void Configure(
        Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<BusSchedule> builder
    )
    {
        builder.HasQueryFilter(s => s.IsActive);

        builder
            .HasOne(s => s.Bus)
            .WithMany(b => b.Schedules)
            .HasForeignKey(s => s.BusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(s => s.Route)
            .WithMany(r => r.Schedules)
            .HasForeignKey(s => s.RouteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasMany(s => s.Segments)
            .WithOne(bs => bs.Schedule)
            .HasForeignKey(bs => bs.ScheduleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
