using Kanini.RouteBuddy.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kanini.RouteBuddy.Data.Configurations;

public class StopConfiguration : IEntityTypeConfiguration<Stop>
{
    public void Configure(
        Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Stop> builder
    )
    {
        builder.HasQueryFilter(s => s.IsActive);

        builder
            .HasMany(s => s.RouteStops)
            .WithOne(rs => rs.Stop)
            .HasForeignKey(rs => rs.StopId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
