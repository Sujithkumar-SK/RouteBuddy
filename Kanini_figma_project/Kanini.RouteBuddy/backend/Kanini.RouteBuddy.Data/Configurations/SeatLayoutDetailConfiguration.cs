using Kanini.RouteBuddy.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kanini.RouteBuddy.Data.Configurations;

public class SeatLayoutDetailConfiguration : IEntityTypeConfiguration<SeatLayoutDetail>
{
    public void Configure(EntityTypeBuilder<SeatLayoutDetail> builder)
    {
        builder
            .HasOne(sld => sld.SeatLayoutTemplate)
            .WithMany(slt => slt.SeatLayoutDetails)
            .HasForeignKey(sld => sld.SeatLayoutTemplateId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
