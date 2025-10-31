using Kanini.RouteBuddy.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kanini.RouteBuddy.Data.Configurations;

public class BusPhotoConfiguration : IEntityTypeConfiguration<BusPhoto>
{
    public void Configure(
        Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<BusPhoto> builder
    )
    {
        builder.Property(bp => bp.ImagePath).HasColumnType("NVARCHAR(500)");

        builder
            .HasOne(bp => bp.Bus)
            .WithMany(b => b.Photos)
            .HasForeignKey(bp => bp.BusId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
