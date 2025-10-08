using Kanini.RouteBuddy.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kanini.RouteBuddy.Data.Configurations;

public class BusConfiguration : IEntityTypeConfiguration<Bus>
{
  public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Bus> builder)
  {
    builder.HasIndex(b => b.RegistrationNo).IsUnique();

    builder.HasMany(b => b.Photos)
        .WithOne(p => p.Bus)
        .HasForeignKey(p => p.BusId);

    builder.HasMany(b => b.Bookings)
        .WithOne(bk => bk.Bus)
        .HasForeignKey(bk => bk.BusId)
        .OnDelete(DeleteBehavior.Restrict);

    builder.HasMany(b => b.BookedSeats)
        .WithOne(bs => bs.Bus)
        .HasForeignKey(bs => bs.BusId)
        .OnDelete(DeleteBehavior.Restrict);

    builder.HasMany(b => b.Reviews)
        .WithOne(r => r.Bus)
        .HasForeignKey(r => r.BusId);

    builder.HasMany(b => b.Schedules)
        .WithOne(s => s.Bus)
        .HasForeignKey(s => s.BusId);

    builder.HasOne(b => b.Vendor)
        .WithMany(v => v.Buses)
        .HasForeignKey(b => b.VendorId)
        .OnDelete(DeleteBehavior.Restrict);
  }
}
