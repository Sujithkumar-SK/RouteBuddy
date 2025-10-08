using Kanini.RouteBuddy.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kanini.RouteBuddy.Data.Configurations;

public class DriverConfiguration : IEntityTypeConfiguration<Driver>
{
  public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Driver> builder)
  {
    builder.HasQueryFilter(d => !d.IsDeleted);

    builder.HasIndex(d => d.LicenseNumber).IsUnique();

    builder.HasMany(d => d.Assignments)
        .WithOne(da => da.Driver)
        .HasForeignKey(da => da.DriverId);
  }
}
