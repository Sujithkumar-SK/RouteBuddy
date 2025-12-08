using Kanini.RouteBuddy.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kanini.RouteBuddy.Data.Configurations;

public class RouteStopConfiguration : IEntityTypeConfiguration<RouteStop>
{
    public void Configure(EntityTypeBuilder<RouteStop> builder)
    {
        // Unique constraint: (RouteId, OrderNumber) for route templates (ScheduleId = NULL)
        // OR (RouteId, OrderNumber, ScheduleId) for schedule-specific stops
        builder
            .HasIndex(rs => new { rs.RouteId, rs.OrderNumber, rs.ScheduleId })
            .IsUnique()
            .HasDatabaseName("IX_RouteStops_RouteId_OrderNumber_ScheduleId");

        builder
            .HasOne(rs => rs.Route)
            .WithMany(r => r.RouteStops)
            .HasForeignKey(rs => rs.RouteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(rs => rs.Stop)
            .WithMany(s => s.RouteStops)
            .HasForeignKey(rs => rs.StopId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
