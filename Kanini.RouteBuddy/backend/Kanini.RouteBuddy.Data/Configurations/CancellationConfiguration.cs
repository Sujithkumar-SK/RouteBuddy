using Kanini.RouteBuddy.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kanini.RouteBuddy.Data.Configurations;

public class CancellationConfiguration : IEntityTypeConfiguration<Cancellation>
{
    public void Configure(
        Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Cancellation> builder
    )
    {
        builder.HasQueryFilter(c => c.IsActive);
        builder
            .HasOne(c => c.Booking)
            .WithOne(b => b.Cancellation)
            .HasForeignKey<Cancellation>(c => c.BookingId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
