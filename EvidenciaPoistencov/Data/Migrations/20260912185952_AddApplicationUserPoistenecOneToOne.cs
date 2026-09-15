using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EvidenciaPoistencov.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddApplicationUserPoistenecOneToOne : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Poistenci_PoistenecId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_PoistenecId",
                table: "AspNetUsers");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_PoistenecId",
                table: "AspNetUsers",
                column: "PoistenecId",
                unique: true,
                filter: "[PoistenecId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Poistenci_PoistenecId",
                table: "AspNetUsers",
                column: "PoistenecId",
                principalTable: "Poistenci",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Poistenci_PoistenecId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_PoistenecId",
                table: "AspNetUsers");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_PoistenecId",
                table: "AspNetUsers",
                column: "PoistenecId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Poistenci_PoistenecId",
                table: "AspNetUsers",
                column: "PoistenecId",
                principalTable: "Poistenci",
                principalColumn: "Id");
        }
    }
}
