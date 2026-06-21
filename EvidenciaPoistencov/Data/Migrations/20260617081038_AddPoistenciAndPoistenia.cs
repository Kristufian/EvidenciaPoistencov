using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EvidenciaPoistencov.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPoistenciAndPoistenia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PoistenecId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Poistenci",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Meno = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Priezvisko = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Telefon = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Ulica = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Mesto = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PSC = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Poistenci", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Poistenia",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nazov = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PredmetPoistenia = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Suma = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PlatnostOd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PlatnostDo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PoistenecId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Poistenia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Poistenia_Poistenci_PoistenecId",
                        column: x => x.PoistenecId,
                        principalTable: "Poistenci",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_PoistenecId",
                table: "AspNetUsers",
                column: "PoistenecId");

            migrationBuilder.CreateIndex(
                name: "IX_Poistenia_PoistenecId",
                table: "Poistenia",
                column: "PoistenecId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Poistenci_PoistenecId",
                table: "AspNetUsers",
                column: "PoistenecId",
                principalTable: "Poistenci",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Poistenci_PoistenecId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Poistenia");

            migrationBuilder.DropTable(
                name: "Poistenci");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_PoistenecId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PoistenecId",
                table: "AspNetUsers");
        }
    }
}
