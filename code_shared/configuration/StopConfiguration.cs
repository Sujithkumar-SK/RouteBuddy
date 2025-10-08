using Kanini.RouteBuddy.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kanini.RouteBuddy.Data.Configurations;

public class StopConfiguration : IEntityTypeConfiguration<Stop>
{
  public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Stop> builder)
  {
    builder.HasQueryFilter(s => !s.IsDeleted);

    builder.HasOne(s => s.Route)
        .WithMany(r => r.Stops)
        .HasForeignKey(s => s.RouteId);
  }
}
