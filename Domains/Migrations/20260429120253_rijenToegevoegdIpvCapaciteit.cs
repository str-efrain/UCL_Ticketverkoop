using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Domains.Migrations
{
    /// <inheritdoc />
    public partial class rijenToegevoegdIpvCapaciteit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.RenameColumn(
                name: "Capacity",
                table: "Vak",
                newName: "seatsPerRow");

            migrationBuilder.AddColumn<int>(
                name: "rows",
                table: "Vak",
                type: "int",
                nullable: false,
                defaultValue: 0);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.DropColumn(
                name: "rows",
                table: "Vak");

            
        }
    }
}
