using Kanini.RouteBuddy.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kanini.RouteBuddy.Data.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(
        Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Booking> builder
    )
    {
        builder.HasQueryFilter(b => b.IsActive);

        builder.HasIndex(b => b.PNRNo).IsUnique();

        builder
            .HasOne(b => b.Customer)
            .WithMany(c => c.Bookings)
            .HasForeignKey(b => b.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasMany(b => b.Segments)
            .WithOne(s => s.Booking)
            .HasForeignKey(s => s.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(b => b.BookedSeats)
            .WithOne(bs => bs.Booking)
            .HasForeignKey(bs => bs.BookingId)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasOne(b => b.Payment)
            .WithOne(p => p.Booking)
            .HasForeignKey<Payment>(p => p.BookingId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(b => b.Cancellation)
            .WithOne(c => c.Booking)
            .HasForeignKey<Cancellation>(c => c.BookingId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
