using Kanini.RouteBuddy.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kanini.RouteBuddy.Data.Configurations;

public class VendorConfiguration : IEntityTypeConfiguration<Vendor>
{
    public void Configure(
        Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Vendor> builder
    )
    {
        builder.HasQueryFilter(v => v.IsActive);

        builder.HasIndex(v => v.BusinessLicenseNumber).IsUnique();
        builder.HasIndex(v => v.TaxRegistrationNumber).IsUnique();

        builder
            .HasMany(v => v.Buses)
            .WithOne(b => b.Vendor)
            .HasForeignKey(b => b.VendorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasMany(v => v.Documents)
            .WithOne(d => d.Vendor)
            .HasForeignKey(d => d.VendorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
