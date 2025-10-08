using Kanini.RouteBuddy.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kanini.RouteBuddy.Data.Configurations;

public class DriverAssignmentConfiguration : IEntityTypeConfiguration<DriverAssignment>
{
  public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<DriverAssignment> builder)
  {
    builder.HasOne(da => da.Driver)
          .WithMany(d => d.Assignments)
          .HasForeignKey(da => da.DriverId);

    builder.HasOne(da => da.Schedule)
        .WithMany(s => s.DriverAssignments)
        .HasForeignKey(da => da.ScheduleId);
  }
}
