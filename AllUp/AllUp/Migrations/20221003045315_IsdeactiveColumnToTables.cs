using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AllUp.Migrations
{
    public partial class IsdeactiveColumnToTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeactive",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeactive",
                table: "Categories",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeactive",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IsDeactive",
                table: "Categories");
        }
    }
}
