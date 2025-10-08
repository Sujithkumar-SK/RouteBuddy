using Kanini.RouteBuddy.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kanini.RouteBuddy.Data.Configurations;

public class FareConfiguration : IEntityTypeConfiguration<Fare>
{
  public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Fare> builder)
  {
    builder.HasOne(f => f.Schedule)
          .WithMany(s => s.Fares)
          .HasForeignKey(f => f.ScheduleId)
          .OnDelete(DeleteBehavior.Cascade);
  }
}
