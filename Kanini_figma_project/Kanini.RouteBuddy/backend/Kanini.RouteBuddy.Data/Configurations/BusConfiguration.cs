using Kanini.RouteBuddy.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kanini.RouteBuddy.Data.Configurations;

public class BusConfiguration : IEntityTypeConfiguration<Bus>
{
    public void Configure(
        Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Bus> builder
    )
    {
        builder.HasQueryFilter(b => b.IsActive);

        builder.HasIndex(b => b.RegistrationNo).IsUnique();

        builder
            .HasOne(b => b.Vendor)
            .WithMany(v => v.Buses)
            .HasForeignKey(b => b.VendorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasMany(b => b.Photos)
            .WithOne(p => p.Bus)
            .HasForeignKey(p => p.BusId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(b => b.Reviews)
            .WithOne(r => r.Bus)
            .HasForeignKey(r => r.BusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasMany(b => b.Schedules)
            .WithOne(s => s.Bus)
            .HasForeignKey(s => s.BusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(b => b.SeatLayoutTemplate)
            .WithMany(slt => slt.Buses)
            .HasForeignKey(b => b.SeatLayoutTemplateId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
