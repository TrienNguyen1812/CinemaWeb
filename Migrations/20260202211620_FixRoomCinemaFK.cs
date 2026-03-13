using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CinemaWeb.Migrations
{
    public partial class FixRoomCinemaFK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScreeningRooms_Cinemas_CinemaIdCinema",
                table: "ScreeningRooms");

            migrationBuilder.DropForeignKey(
                name: "FK_Seats_Cinemas_CinemaIdCinema",
                table: "Seats");

            migrationBuilder.AlterColumn<int>(
                name: "CinemaIdCinema",
                table: "Seats",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "CinemaIdCinema",
                table: "ScreeningRooms",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_ScreeningRooms_Cinemas_CinemaIdCinema",
                table: "ScreeningRooms",
                column: "CinemaIdCinema",
                principalTable: "Cinemas",
                principalColumn: "IdCinema");

            migrationBuilder.AddForeignKey(
                name: "FK_Seats_Cinemas_CinemaIdCinema",
                table: "Seats",
                column: "CinemaIdCinema",
                principalTable: "Cinemas",
                principalColumn: "IdCinema");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScreeningRooms_Cinemas_CinemaIdCinema",
                table: "ScreeningRooms");

            migrationBuilder.DropForeignKey(
                name: "FK_Seats_Cinemas_CinemaIdCinema",
                table: "Seats");

            migrationBuilder.AlterColumn<int>(
                name: "CinemaIdCinema",
                table: "Seats",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CinemaIdCinema",
                table: "ScreeningRooms",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ScreeningRooms_Cinemas_CinemaIdCinema",
                table: "ScreeningRooms",
                column: "CinemaIdCinema",
                principalTable: "Cinemas",
                principalColumn: "IdCinema",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Seats_Cinemas_CinemaIdCinema",
                table: "Seats",
                column: "CinemaIdCinema",
                principalTable: "Cinemas",
                principalColumn: "IdCinema",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
