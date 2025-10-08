using Kanini.RouteBuddy.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kanini.RouteBuddy.Data.Configurations;

public class VendorConfiguration : IEntityTypeConfiguration<Vendor>
{
  public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Vendor> builder)
  {
    builder.HasQueryFilter(v => !v.IsDeleted);

    builder.HasMany(v => v.Buses)
        .WithOne(b => b.Vendor)
        .HasForeignKey(b => b.VendorId)
        .OnDelete(DeleteBehavior.Restrict);
  }
}
