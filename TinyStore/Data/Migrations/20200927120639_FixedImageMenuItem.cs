using Microsoft.EntityFrameworkCore.Migrations;

namespace TinyStore.Data.Migrations
{
    public partial class FixedImageMenuItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ïmage",
                table: "MenuItem");

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "MenuItem",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "MenuItem");

            migrationBuilder.AddColumn<string>(
                name: "Ïmage",
                table: "MenuItem",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
