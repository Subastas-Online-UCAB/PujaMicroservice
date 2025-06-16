using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PujaService.Infraestructura.Migrations
{
    /// <inheritdoc />
    public partial class AddPujasAutomaticasTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PujasAutomaticas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SubastaId = table.Column<string>(type: "text", nullable: false),
                    UsuarioId = table.Column<string>(type: "text", nullable: false),
                    MontoMaximo = table.Column<decimal>(type: "numeric", nullable: false),
                    Incremento = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PujasAutomaticas", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PujasAutomaticas");
        }
    }
}
