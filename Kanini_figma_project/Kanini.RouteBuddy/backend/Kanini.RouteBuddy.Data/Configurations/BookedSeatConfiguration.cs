using Kanini.RouteBuddy.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kanini.RouteBuddy.Data.Configurations;

public class BookedSeatConfiguration : IEntityTypeConfiguration<BookedSeat>
{
    public void Configure(
        Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<BookedSeat> builder
    )
    {
        builder
            .HasOne(bs => bs.BookingSegment)
            .WithMany(seg => seg.BookedSeats)
            .HasForeignKey(bs => bs.BookingSegmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(bs => bs.Booking)
            .WithMany(b => b.BookedSeats)
            .HasForeignKey(bs => bs.BookingId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
