using Kanini.RouteBuddy.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kanini.RouteBuddy.Data.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
  public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Booking> builder)
  {
    builder.HasOne(b => b.User)
            .WithMany(u => u.Bookings)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Restrict);

    builder.HasOne(b => b.Bus)
        .WithMany(bu => bu.Bookings)
        .HasForeignKey(b => b.BusId)
        .OnDelete(DeleteBehavior.Restrict);

    builder.HasOne(b => b.Schedule)
        .WithMany(s => s.Bookings)
        .HasForeignKey(b => b.ScheduleId)
        .OnDelete(DeleteBehavior.Restrict);

    builder.HasMany(b => b.BookedSeats)
        .WithOne(bs => bs.Booking)
        .HasForeignKey(bs => bs.BookingId)
        .OnDelete(DeleteBehavior.Restrict);

    builder.HasOne(b => b.Payment)
        .WithOne(p => p.Booking)
        .HasForeignKey<Payment>(p => p.BookingId);

    builder.HasOne(b => b.Cancellation)
        .WithOne(c => c.Booking)
        .HasForeignKey<Cancellation>(c => c.BookingId);
  }
}
