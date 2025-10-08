using Kanini.RouteBuddy.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kanini.RouteBuddy.Data.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
  public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Review> builder)
  {
    builder.HasOne(r => r.User)
          .WithMany(u => u.Reviews)
          .HasForeignKey(r => r.UserId);

    builder.HasOne(r => r.Bus)
        .WithMany(b => b.Reviews)
        .HasForeignKey(r => r.BusId);
  }
}
