using Kanini.RouteBuddy.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kanini.RouteBuddy.Data.Configurations;

public class RouteConfiguration : IEntityTypeConfiguration<Route>
{
  public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Route> builder)
  {
    builder.HasQueryFilter(r => !r.IsDeleted);

    builder.HasMany(r => r.Stops)
        .WithOne(s => s.Route)
        .HasForeignKey(s => s.RouteId);

    builder.HasMany(r => r.Schedules)
        .WithOne(s => s.Route)
        .HasForeignKey(s => s.RouteId);
  }
}
