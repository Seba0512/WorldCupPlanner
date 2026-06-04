using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorldCupPlanner.AccesoADatos.Migraciones
{
    /// <inheritdoc />
    public partial class EliminarFkPartidoEnIncidencia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Incidencia_Partidos_PartidoId",
                table: "Incidencia");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_Incidencia_Partidos_PartidoId",
                table: "Incidencia",
                column: "PartidoId",
                principalTable: "Partidos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
