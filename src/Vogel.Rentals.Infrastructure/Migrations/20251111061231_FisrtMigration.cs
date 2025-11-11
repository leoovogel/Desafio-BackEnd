using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vogel.Rentals.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FisrtMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "motorcycles",
                columns: table => new
                {
                    identifier = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    year = table.Column<int>(type: "integer", nullable: false),
                    model = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    plate = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_motorcycles", x => x.identifier);
                });

            migrationBuilder.CreateIndex(
                name: "IX_motorcycles_plate",
                table: "motorcycles",
                column: "plate",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "motorcycles");
        }
    }
}
