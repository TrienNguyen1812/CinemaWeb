using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CinemaWeb.Migrations
{
    public partial class ChangeStartTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<TimeSpan>(
                name: "StartTime",
                table: "Showtimes",
                type: "time",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "StartTime",
                table: "Showtimes",
                type: "datetime",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "time");
        }
    }
}
