using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RENAPER.Dominio.Migrations
{
    /// <inheritdoc />
    public partial class Datos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApiKeys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Mail = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SolicitudesSemanales = table.Column<int>(type: "int", nullable: false),
                    SolicitudesUsadas = table.Column<int>(type: "int", nullable: false),
                    FechaInicioSemana = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Activa = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiKeys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Personas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Apellido = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DNI = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CUIL = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Mail = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personas", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_Key",
                table: "ApiKeys",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_Mail",
                table: "ApiKeys",
                column: "Mail",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Personas_CUIL",
                table: "Personas",
                column: "CUIL",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Personas_DNI",
                table: "Personas",
                column: "DNI",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiKeys");

            migrationBuilder.DropTable(
                name: "Personas");
        }
    }
}
