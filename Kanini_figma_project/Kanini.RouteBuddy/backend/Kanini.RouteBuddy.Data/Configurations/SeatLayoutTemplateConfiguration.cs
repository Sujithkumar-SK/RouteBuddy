using Kanini.RouteBuddy.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kanini.RouteBuddy.Data.Configurations;

public class SeatLayoutTemplateConfiguration : IEntityTypeConfiguration<SeatLayoutTemplate>
{
    public void Configure(EntityTypeBuilder<SeatLayoutTemplate> builder)
    {
        builder.HasQueryFilter(slt => slt.IsActive);

        builder
            .HasMany(slt => slt.SeatLayoutDetails)
            .WithOne(sld => sld.SeatLayoutTemplate)
            .HasForeignKey(sld => sld.SeatLayoutTemplateId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(slt => slt.Buses)
            .WithOne(b => b.SeatLayoutTemplate)
            .HasForeignKey(b => b.SeatLayoutTemplateId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
