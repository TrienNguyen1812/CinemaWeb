using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CinemaWeb.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cinemas",
                columns: table => new
                {
                    IdCinema = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CinemaName = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(255)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cinemas", x => x.IdCinema);
                });

            migrationBuilder.CreateTable(
                name: "Combos",
                columns: table => new
                {
                    IdCombo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ComboName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Combos", x => x.IdCombo);
                });

            migrationBuilder.CreateTable(
                name: "Movies",
                columns: table => new
                {
                    IdMovie = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MovieName = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(30)", nullable: false),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    ReleaseDate = table.Column<DateTime>(type: "date", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    Poster = table.Column<string>(type: "varchar(30)", nullable: false),
                    Trailer = table.Column<string>(type: "varchar(30)", nullable: false),
                    Age = table.Column<string>(type: "nvarchar(30)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movies", x => x.IdMovie);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    IdUser = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(30)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(30)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(11)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.IdUser);
                });

            migrationBuilder.CreateTable(
                name: "ScreeningRooms",
                columns: table => new
                {
                    IdRoom = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoomName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SeatQuantity = table.Column<int>(type: "int", nullable: false),
                    IdCinema = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScreeningRooms", x => x.IdRoom);
                    table.ForeignKey(
                        name: "FK_ScreeningRooms_Cinemas_IdCinema",
                        column: x => x.IdCinema,
                        principalTable: "Cinemas",
                        principalColumn: "IdCinema",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    IdOrder = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderTime = table.Column<DateTime>(type: "date", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    IdUser = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.IdOrder);
                    table.ForeignKey(
                        name: "FK_Orders_Users_IdUser",
                        column: x => x.IdUser,
                        principalTable: "Users",
                        principalColumn: "IdUser",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Seats",
                columns: table => new
                {
                    IdSeat = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SeatRow = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    SeatNumber = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    TypeSeat = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    IdRoom = table.Column<int>(type: "int", nullable: false),
                    CinemaIdCinema = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seats", x => x.IdSeat);
                    table.ForeignKey(
                        name: "FK_Seats_Cinemas_CinemaIdCinema",
                        column: x => x.CinemaIdCinema,
                        principalTable: "Cinemas",
                        principalColumn: "IdCinema");
                    table.ForeignKey(
                        name: "FK_Seats_ScreeningRooms_IdRoom",
                        column: x => x.IdRoom,
                        principalTable: "ScreeningRooms",
                        principalColumn: "IdRoom",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Showtimes",
                columns: table => new
                {
                    IdShowtime = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartFilm = table.Column<DateTime>(type: "date", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    Price = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    IdMovie = table.Column<int>(type: "int", nullable: false),
                    IdRoom = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Showtimes", x => x.IdShowtime);
                    table.ForeignKey(
                        name: "FK_Showtimes_Movies_IdMovie",
                        column: x => x.IdMovie,
                        principalTable: "Movies",
                        principalColumn: "IdMovie",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Showtimes_ScreeningRooms_IdRoom",
                        column: x => x.IdRoom,
                        principalTable: "ScreeningRooms",
                        principalColumn: "IdRoom",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderCombos",
                columns: table => new
                {
                    IdOrderCombo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    IdOrder = table.Column<int>(type: "int", nullable: false),
                    OrderIdOrder = table.Column<int>(type: "int", nullable: true),
                    IdCombo = table.Column<int>(type: "int", nullable: false),
                    ComboIdCombo = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderCombos", x => x.IdOrderCombo);
                    table.ForeignKey(
                        name: "FK_OrderCombos_Combos_ComboIdCombo",
                        column: x => x.ComboIdCombo,
                        principalTable: "Combos",
                        principalColumn: "IdCombo");
                    table.ForeignKey(
                        name: "FK_OrderCombos_Orders_OrderIdOrder",
                        column: x => x.OrderIdOrder,
                        principalTable: "Orders",
                        principalColumn: "IdOrder");
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    IdPayment = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentMethod = table.Column<string>(type: "nvarchar(30)", nullable: false),
                    Price = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    TransactionCode = table.Column<string>(type: "nvarchar(30)", nullable: false),
                    PaymentTime = table.Column<DateTime>(type: "date", nullable: false),
                    IdOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.IdPayment);
                    table.ForeignKey(
                        name: "FK_Payments_Orders_IdOrder",
                        column: x => x.IdOrder,
                        principalTable: "Orders",
                        principalColumn: "IdOrder",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    IdTicket = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OriginalPrice = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    FinalPrice = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    IdShowtime = table.Column<int>(type: "int", nullable: false),
                    IdSeat = table.Column<int>(type: "int", nullable: false),
                    IdOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.IdTicket);
                    table.ForeignKey(
                        name: "FK_Tickets_Orders_IdOrder",
                        column: x => x.IdOrder,
                        principalTable: "Orders",
                        principalColumn: "IdOrder",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tickets_Seats_IdSeat",
                        column: x => x.IdSeat,
                        principalTable: "Seats",
                        principalColumn: "IdSeat");
                    table.ForeignKey(
                        name: "FK_Tickets_Showtimes_IdShowtime",
                        column: x => x.IdShowtime,
                        principalTable: "Showtimes",
                        principalColumn: "IdShowtime");
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderCombos_ComboIdCombo",
                table: "OrderCombos",
                column: "ComboIdCombo");

            migrationBuilder.CreateIndex(
                name: "IX_OrderCombos_OrderIdOrder",
                table: "OrderCombos",
                column: "OrderIdOrder");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_IdUser",
                table: "Orders",
                column: "IdUser");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_IdOrder",
                table: "Payments",
                column: "IdOrder");

            migrationBuilder.CreateIndex(
                name: "IX_ScreeningRooms_IdCinema",
                table: "ScreeningRooms",
                column: "IdCinema");

            migrationBuilder.CreateIndex(
                name: "IX_Seats_CinemaIdCinema",
                table: "Seats",
                column: "CinemaIdCinema");

            migrationBuilder.CreateIndex(
                name: "IX_Seats_IdRoom",
                table: "Seats",
                column: "IdRoom");

            migrationBuilder.CreateIndex(
                name: "IX_Showtimes_IdMovie",
                table: "Showtimes",
                column: "IdMovie");

            migrationBuilder.CreateIndex(
                name: "IX_Showtimes_IdRoom",
                table: "Showtimes",
                column: "IdRoom");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_IdOrder",
                table: "Tickets",
                column: "IdOrder");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_IdSeat",
                table: "Tickets",
                column: "IdSeat");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_IdShowtime",
                table: "Tickets",
                column: "IdShowtime");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderCombos");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "Tickets");

            migrationBuilder.DropTable(
                name: "Combos");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Seats");

            migrationBuilder.DropTable(
                name: "Showtimes");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Movies");

            migrationBuilder.DropTable(
                name: "ScreeningRooms");

            migrationBuilder.DropTable(
                name: "Cinemas");
        }
    }
}
