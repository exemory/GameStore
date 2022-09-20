using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations;

public class GenreConfiguration : IEntityTypeConfiguration<Genre>
{
    public void Configure(EntityTypeBuilder<Genre> builder)
    {
        builder.HasIndex(g => new {g.Name, g.ParentId})
            .IsUnique()
            .HasFilter(null);

        builder.HasCheckConstraint($"CK_{nameof(Genre)}_{nameof(Genre.ParentId)}",
            $"[{nameof(Genre.ParentId)}] != [{nameof(Genre.Id)}]");

        SeedData(builder);
    }

    private void SeedData(EntityTypeBuilder<Genre> builder)
    {
        var genres = new List<Genre>
        {
            new() {Id = new Guid("47caa6af-167e-46b5-adbe-ee93534885b0"), Name = "Strategy"},
            new()
            {
                Id = new Guid("00c08f06-efcd-41d3-9621-5b1e90d2da6c"), Name = "RTS",
                ParentId = new Guid("47caa6af-167e-46b5-adbe-ee93534885b0")
            },
            new()
            {
                Id = new Guid("54f35383-8f67-4481-a559-1c01e697be33"), Name = "TBS",
                ParentId = new Guid("47caa6af-167e-46b5-adbe-ee93534885b0")
            },
            new() {Id = new Guid("db7653d5-b4be-40e1-9985-6a4a61a674ed"), Name = "RPG"},
            new() {Id = new Guid("3db87e8a-9714-4e27-bba7-bbafa6bae2b9"), Name = "Sports"},
            new() {Id = new Guid("98047c28-741b-4add-be6d-e46d81e9bb45"), Name = "Races"},
            new()
            {
                Id = new Guid("f6de1593-a8ee-45d5-b220-57a1da6f566e"), Name = "Rally",
                ParentId = new Guid("98047c28-741b-4add-be6d-e46d81e9bb45")
            },
            new()
            {
                Id = new Guid("6519ec12-d1fd-49d5-81b2-8f5545390b9d"), Name = "Arcade",
                ParentId = new Guid("98047c28-741b-4add-be6d-e46d81e9bb45")
            },
            new()
            {
                Id = new Guid("30d7e62c-13ce-4d3f-9590-339c9d9ecf06"), Name = "Formula",
                ParentId = new Guid("98047c28-741b-4add-be6d-e46d81e9bb45")
            },
            new()
            {
                Id = new Guid("0bd3d31b-65b6-47cd-adad-a8a90618c2ac"), Name = "Off-road",
                ParentId = new Guid("98047c28-741b-4add-be6d-e46d81e9bb45")
            },
            new() {Id = new Guid("2a1dc5b7-373e-4da3-bd8c-caa3158888f7"), Name = "Action"},
            new()
            {
                Id = new Guid("7607b9a4-d7ce-46ef-a67c-884ec3b3ac1f"), Name = "FPS",
                ParentId = new Guid("2a1dc5b7-373e-4da3-bd8c-caa3158888f7")
            },
            new()
            {
                Id = new Guid("54f3b304-1535-418e-bbf0-2e8a4028c371"), Name = "TPS",
                ParentId = new Guid("2a1dc5b7-373e-4da3-bd8c-caa3158888f7")
            },
            new()
            {
                Id = new Guid("7707e09b-6f55-4eaa-bcab-ae1491bbb0db"), Name = "Misc.",
                ParentId = new Guid("2a1dc5b7-373e-4da3-bd8c-caa3158888f7")
            },
            new() {Id = new Guid("a13f2dfc-d2c3-4122-ade7-9a4c44915ada"), Name = "Adventure"},
            new() {Id = new Guid("8df84e6c-bbf3-4b77-bf8d-04878eeddc15"), Name = "Puzzle & Skill"},
            new() {Id = new Guid("9e8cb492-4345-43ba-9050-dbdf34156713"), Name = "Misc."},
        };

        builder.HasData(genres);
    }
}