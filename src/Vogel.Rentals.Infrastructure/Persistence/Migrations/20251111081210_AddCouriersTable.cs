using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vogel.Rentals.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCouriersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "couriers",
                columns: table => new
                {
                    identifier = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    cnpj = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: false),
                    birth_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    cnh_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    cnh_type = table.Column<int>(type: "integer", nullable: false),
                    cnh_image = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_couriers", x => x.identifier);
                });

            migrationBuilder.CreateIndex(
                name: "IX_couriers_cnh_number",
                table: "couriers",
                column: "cnh_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_couriers_cnpj",
                table: "couriers",
                column: "cnpj",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "couriers");
        }
    }
}
