using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Domains.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUniqueConstraintFromAbonnementOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Abonnement_OrderId",
                table: "Abonnement");

            migrationBuilder.CreateIndex(
                name: "IX_Abonnement_OrderId",
                table: "Abonnement",
                column: "OrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Abonnement_OrderId",
                table: "Abonnement");

            migrationBuilder.CreateIndex(
                name: "IX_Abonnement_OrderId",
                table: "Abonnement",
                column: "OrderId",
                unique: true);
        }
    }
}
