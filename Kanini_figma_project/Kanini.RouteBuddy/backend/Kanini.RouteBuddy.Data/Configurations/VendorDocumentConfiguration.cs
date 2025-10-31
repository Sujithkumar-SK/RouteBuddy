using Kanini.RouteBuddy.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kanini.RouteBuddy.Data.Configurations;

public class VendorDocumentConfiguration : IEntityTypeConfiguration<VendorDocument>
{
    public void Configure(
        Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<VendorDocument> builder
    )
    {
        builder
            .HasOne(vd => vd.Vendor)
            .WithMany(v => v.Documents)
            .HasForeignKey(vd => vd.VendorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
