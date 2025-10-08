using Kanini.RouteBuddy.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kanini.RouteBuddy.Data.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
  public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Payment> builder)
  {
    builder.HasOne(p => p.Booking)
          .WithOne(b => b.Payment)
          .HasForeignKey<Payment>(p => p.BookingId);

    builder.HasMany(p => p.Refunds)
        .WithOne(r => r.Payment)
        .HasForeignKey(r => r.PaymentId);
  }
}
