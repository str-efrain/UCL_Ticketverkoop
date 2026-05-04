using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Domains.Migrations
{
    /// <inheritdoc />
    public partial class AddedSeasonAndAddedSubscriptionPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "SubscriptionPrice",
                table: "Vak",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Seizoen",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "SeizoenId",
                table: "Match",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_Match_SeizoenId",
                table: "Match",
                column: "SeizoenId");

            migrationBuilder.AddForeignKey(
                name: "FK_Match_Seizoen_SeizoenId",
                table: "Match",
                column: "SeizoenId",
                principalTable: "Seizoen",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Match_Seizoen_SeizoenId",
                table: "Match");

            migrationBuilder.DropIndex(
                name: "IX_Match_SeizoenId",
                table: "Match");

            migrationBuilder.DropColumn(
                name: "SubscriptionPrice",
                table: "Vak");

            migrationBuilder.DropColumn(
                name: "SeizoenId",
                table: "Match");

            migrationBuilder.AlterColumn<int>(
                name: "Name",
                table: "Seizoen",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
