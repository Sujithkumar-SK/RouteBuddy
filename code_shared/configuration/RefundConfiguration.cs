using Kanini.RouteBuddy.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kanini.RouteBuddy.Data.Configurations;

public class RefundConfiguration : IEntityTypeConfiguration<Refund>
{
  public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Refund> builder)
  {
    builder.HasOne(r => r.Payment)
          .WithMany(p => p.Refunds)
          .HasForeignKey(r => r.PaymentId);
  }
}
