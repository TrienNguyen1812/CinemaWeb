using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CinemaWeb.Migrations
{
    public partial class UpdateCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "Movies",
                type: "nvarchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "Movies",
                type: "nvarchar(30)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)");
        }
    }
}
