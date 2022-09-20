using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    public partial class AddDataSeeding : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Genres",
                columns: new[] { "Id", "Name", "ParentId" },
                values: new object[,]
                {
                    { new Guid("2a1dc5b7-373e-4da3-bd8c-caa3158888f7"), "Action", null },
                    { new Guid("3db87e8a-9714-4e27-bba7-bbafa6bae2b9"), "Sports", null },
                    { new Guid("47caa6af-167e-46b5-adbe-ee93534885b0"), "Strategy", null },
                    { new Guid("8df84e6c-bbf3-4b77-bf8d-04878eeddc15"), "Puzzle & Skill", null },
                    { new Guid("98047c28-741b-4add-be6d-e46d81e9bb45"), "Races", null },
                    { new Guid("9e8cb492-4345-43ba-9050-dbdf34156713"), "Misc.", null },
                    { new Guid("a13f2dfc-d2c3-4122-ade7-9a4c44915ada"), "Adventure", null },
                    { new Guid("db7653d5-b4be-40e1-9985-6a4a61a674ed"), "RPG", null }
                });

            migrationBuilder.InsertData(
                table: "PlatformTypes",
                columns: new[] { "Id", "Type" },
                values: new object[,]
                {
                    { new Guid("397f9580-8b09-43ea-a9de-07e9b78422f9"), "Desktop" },
                    { new Guid("54a66d4a-bd5d-4bf0-ad27-a5eb6584c512"), "Mobile" },
                    { new Guid("ce56abee-2346-46e6-a683-7697dcbeef2b"), "Console" },
                    { new Guid("fddef9fc-211c-403f-ab7e-a10be5f180c4"), "Browser" }
                });

            migrationBuilder.InsertData(
                table: "Genres",
                columns: new[] { "Id", "Name", "ParentId" },
                values: new object[,]
                {
                    { new Guid("00c08f06-efcd-41d3-9621-5b1e90d2da6c"), "RTS", new Guid("47caa6af-167e-46b5-adbe-ee93534885b0") },
                    { new Guid("0bd3d31b-65b6-47cd-adad-a8a90618c2ac"), "Off-road", new Guid("98047c28-741b-4add-be6d-e46d81e9bb45") },
                    { new Guid("30d7e62c-13ce-4d3f-9590-339c9d9ecf06"), "Formula", new Guid("98047c28-741b-4add-be6d-e46d81e9bb45") },
                    { new Guid("54f35383-8f67-4481-a559-1c01e697be33"), "TBS", new Guid("47caa6af-167e-46b5-adbe-ee93534885b0") },
                    { new Guid("54f3b304-1535-418e-bbf0-2e8a4028c371"), "TPS", new Guid("2a1dc5b7-373e-4da3-bd8c-caa3158888f7") },
                    { new Guid("6519ec12-d1fd-49d5-81b2-8f5545390b9d"), "Arcade", new Guid("98047c28-741b-4add-be6d-e46d81e9bb45") },
                    { new Guid("7607b9a4-d7ce-46ef-a67c-884ec3b3ac1f"), "FPS", new Guid("2a1dc5b7-373e-4da3-bd8c-caa3158888f7") },
                    { new Guid("7707e09b-6f55-4eaa-bcab-ae1491bbb0db"), "Misc.", new Guid("2a1dc5b7-373e-4da3-bd8c-caa3158888f7") },
                    { new Guid("f6de1593-a8ee-45d5-b220-57a1da6f566e"), "Rally", new Guid("98047c28-741b-4add-be6d-e46d81e9bb45") }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: new Guid("00c08f06-efcd-41d3-9621-5b1e90d2da6c"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: new Guid("0bd3d31b-65b6-47cd-adad-a8a90618c2ac"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: new Guid("30d7e62c-13ce-4d3f-9590-339c9d9ecf06"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: new Guid("3db87e8a-9714-4e27-bba7-bbafa6bae2b9"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: new Guid("54f35383-8f67-4481-a559-1c01e697be33"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: new Guid("54f3b304-1535-418e-bbf0-2e8a4028c371"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: new Guid("6519ec12-d1fd-49d5-81b2-8f5545390b9d"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: new Guid("7607b9a4-d7ce-46ef-a67c-884ec3b3ac1f"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: new Guid("7707e09b-6f55-4eaa-bcab-ae1491bbb0db"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: new Guid("8df84e6c-bbf3-4b77-bf8d-04878eeddc15"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: new Guid("9e8cb492-4345-43ba-9050-dbdf34156713"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: new Guid("a13f2dfc-d2c3-4122-ade7-9a4c44915ada"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: new Guid("db7653d5-b4be-40e1-9985-6a4a61a674ed"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: new Guid("f6de1593-a8ee-45d5-b220-57a1da6f566e"));

            migrationBuilder.DeleteData(
                table: "PlatformTypes",
                keyColumn: "Id",
                keyValue: new Guid("397f9580-8b09-43ea-a9de-07e9b78422f9"));

            migrationBuilder.DeleteData(
                table: "PlatformTypes",
                keyColumn: "Id",
                keyValue: new Guid("54a66d4a-bd5d-4bf0-ad27-a5eb6584c512"));

            migrationBuilder.DeleteData(
                table: "PlatformTypes",
                keyColumn: "Id",
                keyValue: new Guid("ce56abee-2346-46e6-a683-7697dcbeef2b"));

            migrationBuilder.DeleteData(
                table: "PlatformTypes",
                keyColumn: "Id",
                keyValue: new Guid("fddef9fc-211c-403f-ab7e-a10be5f180c4"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: new Guid("2a1dc5b7-373e-4da3-bd8c-caa3158888f7"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: new Guid("47caa6af-167e-46b5-adbe-ee93534885b0"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: new Guid("98047c28-741b-4add-be6d-e46d81e9bb45"));
        }
    }
}
