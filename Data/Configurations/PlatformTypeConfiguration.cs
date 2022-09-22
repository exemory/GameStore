using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations;

public class PlatformTypeConfiguration : IEntityTypeConfiguration<PlatformType>
{
    public void Configure(EntityTypeBuilder<PlatformType> builder)
    {
        builder.Property(pt => pt.Type)
            .HasMaxLength(50);

        builder.HasIndex(pt => pt.Type)
            .IsUnique();

        SeedData(builder);
    }

    private static void SeedData(EntityTypeBuilder<PlatformType> builder)
    {
        var platformTypes = new List<PlatformType>
        {
            new() {Id = new Guid("54a66d4a-bd5d-4bf0-ad27-a5eb6584c512"), Type = "Mobile"},
            new() {Id = new Guid("fddef9fc-211c-403f-ab7e-a10be5f180c4"), Type = "Browser"},
            new() {Id = new Guid("397f9580-8b09-43ea-a9de-07e9b78422f9"), Type = "Desktop"},
            new() {Id = new Guid("ce56abee-2346-46e6-a683-7697dcbeef2b"), Type = "Console"}
        };

        builder.HasData(platformTypes);
    }
}