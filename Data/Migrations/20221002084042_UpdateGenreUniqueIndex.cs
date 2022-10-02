using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    public partial class UpdateGenreUniqueIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Genres_Name_ParentId",
                table: "Genres");

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: new Guid("7707e09b-6f55-4eaa-bcab-ae1491bbb0db"));

            migrationBuilder.CreateIndex(
                name: "IX_Genres_Name",
                table: "Genres",
                column: "Name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Genres_Name",
                table: "Genres");

            migrationBuilder.InsertData(
                table: "Genres",
                columns: new[] { "Id", "Name", "ParentId" },
                values: new object[] { new Guid("7707e09b-6f55-4eaa-bcab-ae1491bbb0db"), "Misc.", new Guid("2a1dc5b7-373e-4da3-bd8c-caa3158888f7") });

            migrationBuilder.CreateIndex(
                name: "IX_Genres_Name_ParentId",
                table: "Genres",
                columns: new[] { "Name", "ParentId" },
                unique: true);
        }
    }
}
