using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CinemaWeb.Migrations
{
    public partial class AddSearchHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderCombos_Combos_ComboIdCombo",
                table: "OrderCombos");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderCombos_Orders_OrderIdOrder",
                table: "OrderCombos");

            migrationBuilder.DropIndex(
                name: "IX_OrderCombos_ComboIdCombo",
                table: "OrderCombos");

            migrationBuilder.DropIndex(
                name: "IX_OrderCombos_OrderIdOrder",
                table: "OrderCombos");

            migrationBuilder.DropColumn(
                name: "ComboIdCombo",
                table: "OrderCombos");

            migrationBuilder.DropColumn(
                name: "OrderIdOrder",
                table: "OrderCombos");

            migrationBuilder.CreateTable(
                name: "SearchHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    IdMovie = table.Column<int>(type: "int", nullable: false),
                    SearchTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SearchHistories_Movies_IdMovie",
                        column: x => x.IdMovie,
                        principalTable: "Movies",
                        principalColumn: "IdMovie",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderCombos_IdCombo",
                table: "OrderCombos",
                column: "IdCombo");

            migrationBuilder.CreateIndex(
                name: "IX_OrderCombos_IdOrder",
                table: "OrderCombos",
                column: "IdOrder");

            migrationBuilder.CreateIndex(
                name: "IX_SearchHistories_IdMovie",
                table: "SearchHistories",
                column: "IdMovie");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderCombos_Combos_IdCombo",
                table: "OrderCombos",
                column: "IdCombo",
                principalTable: "Combos",
                principalColumn: "IdCombo",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderCombos_Orders_IdOrder",
                table: "OrderCombos",
                column: "IdOrder",
                principalTable: "Orders",
                principalColumn: "IdOrder",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderCombos_Combos_IdCombo",
                table: "OrderCombos");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderCombos_Orders_IdOrder",
                table: "OrderCombos");

            migrationBuilder.DropTable(
                name: "SearchHistories");

            migrationBuilder.DropIndex(
                name: "IX_OrderCombos_IdCombo",
                table: "OrderCombos");

            migrationBuilder.DropIndex(
                name: "IX_OrderCombos_IdOrder",
                table: "OrderCombos");

            migrationBuilder.AddColumn<int>(
                name: "ComboIdCombo",
                table: "OrderCombos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrderIdOrder",
                table: "OrderCombos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderCombos_ComboIdCombo",
                table: "OrderCombos",
                column: "ComboIdCombo");

            migrationBuilder.CreateIndex(
                name: "IX_OrderCombos_OrderIdOrder",
                table: "OrderCombos",
                column: "OrderIdOrder");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderCombos_Combos_ComboIdCombo",
                table: "OrderCombos",
                column: "ComboIdCombo",
                principalTable: "Combos",
                principalColumn: "IdCombo");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderCombos_Orders_OrderIdOrder",
                table: "OrderCombos",
                column: "OrderIdOrder",
                principalTable: "Orders",
                principalColumn: "IdOrder");
        }
    }
}
