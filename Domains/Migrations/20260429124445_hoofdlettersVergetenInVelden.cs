using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Domains.Migrations
{
    /// <inheritdoc />
    public partial class hoofdlettersVergetenInVelden : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "seatsPerRow",
                table: "Vak",
                newName: "SeatsPerRow");

            migrationBuilder.RenameColumn(
                name: "rows",
                table: "Vak",
                newName: "Rows");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SeatsPerRow",
                table: "Vak",
                newName: "seatsPerRow");

            migrationBuilder.RenameColumn(
                name: "Rows",
                table: "Vak",
                newName: "rows");
        }
    }
}
