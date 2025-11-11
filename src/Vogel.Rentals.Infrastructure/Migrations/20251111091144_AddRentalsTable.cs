using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vogel.Rentals.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRentalsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "rentals",
                columns: table => new
                {
                    identifier = table.Column<Guid>(type: "uuid", nullable: false),
                    courier_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    motorcycle_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    end_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    expected_end_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    return_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    plan = table.Column<int>(type: "integer", nullable: false),
                    daily_rate = table.Column<decimal>(type: "numeric(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rentals", x => x.identifier);
                    table.ForeignKey(
                        name: "FK_rentals_couriers_courier_id",
                        column: x => x.courier_id,
                        principalTable: "couriers",
                        principalColumn: "identifier",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_rentals_motorcycles_motorcycle_id",
                        column: x => x.motorcycle_id,
                        principalTable: "motorcycles",
                        principalColumn: "identifier",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_rentals_courier_id",
                table: "rentals",
                column: "courier_id");

            migrationBuilder.CreateIndex(
                name: "IX_rentals_motorcycle_id",
                table: "rentals",
                column: "motorcycle_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "rentals");
        }
    }
}
