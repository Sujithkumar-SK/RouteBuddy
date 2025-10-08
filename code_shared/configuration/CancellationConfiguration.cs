using Kanini.RouteBuddy.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kanini.RouteBuddy.Data.Configurations;

public class CancellationConfiguration : IEntityTypeConfiguration<Cancellation>
{
  public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Cancellation> builder)
  {
    builder.HasOne(c => c.Booking)
          .WithOne(b => b.Cancellation)
          .HasForeignKey<Cancellation>(c => c.BookingId);
  }
}
