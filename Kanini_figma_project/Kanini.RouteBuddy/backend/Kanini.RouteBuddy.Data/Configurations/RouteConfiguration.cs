using Kanini.RouteBuddy.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kanini.RouteBuddy.Data.Configurations;

public class RouteConfiguration : IEntityTypeConfiguration<Route>
{
    public void Configure(
        Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Route> builder
    )
    {
        builder.HasQueryFilter(r => r.IsActive);

        builder.Property(r => r.Source).HasColumnType("NVARCHAR(100)");

        builder.Property(r => r.Destination).HasColumnType("NVARCHAR(100)");

        builder.Property(r => r.Distance).HasColumnType("DECIMAL(10,2)");

        builder.Property(r => r.Duration).HasColumnType("TIME");

        builder.Property(r => r.BasePrice).HasColumnType("DECIMAL(18,2)");

        builder
            .HasMany(r => r.RouteStops)
            .WithOne(rs => rs.Route)
            .HasForeignKey(rs => rs.RouteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(r => r.Schedules)
            .WithOne(s => s.Route)
            .HasForeignKey(s => s.RouteId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
