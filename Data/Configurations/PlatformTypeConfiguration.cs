using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations;

public class PlatformTypeConfiguration : IEntityTypeConfiguration<PlatformType>
{
    public void Configure(EntityTypeBuilder<PlatformType> builder)
    {
        builder.HasIndex(pt => pt.Type)
            .IsUnique();
    }
}