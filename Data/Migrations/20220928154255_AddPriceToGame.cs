using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    public partial class AddPriceToGame : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Games",
                type: "decimal(6,2)",
                precision: 6,
                scale: 2,
                nullable: false);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Game_Price",
                table: "Games",
                sql: "[Price] BETWEEN 0 AND 1000");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Game_Price",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Games");
        }
    }
}
