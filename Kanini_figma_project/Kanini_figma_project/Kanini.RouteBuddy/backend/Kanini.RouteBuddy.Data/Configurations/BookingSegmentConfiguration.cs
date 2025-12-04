using Kanini.RouteBuddy.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kanini.RouteBuddy.Data.Configurations;

public class BookingSegmentConfiguration : IEntityTypeConfiguration<BookingSegment>
{
    public void Configure(
        Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<BookingSegment> builder
    )
    {
        builder
            .HasOne(bs => bs.Booking)
            .WithMany(b => b.Segments)
            .HasForeignKey(bs => bs.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(bs => bs.Schedule)
            .WithMany(s => s.Segments)
            .HasForeignKey(bs => bs.ScheduleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasMany(bs => bs.BookedSeats)
            .WithOne(bks => bks.BookingSegment)
            .HasForeignKey(bks => bks.BookingSegmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
