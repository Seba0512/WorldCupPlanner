using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorldCupPlanner.AccesoADatos.Migraciones
{
    /// <inheritdoc />
    public partial class AgregarConfiguracionTorneo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConfiguracionesTorneo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    SemillaFixture = table.Column<int>(type: "int", nullable: false),
                    SemillaCompletar = table.Column<int>(type: "int", nullable: false),
                    SemillaCrucesFase = table.Column<int>(type: "int", nullable: false),
                    SemillaSimulation = table.Column<int>(type: "int", nullable: false),
                    FechaInicioTorneo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaxPartidosPorDia = table.Column<int>(type: "int", nullable: false),
                    SeparacionEntreFechas = table.Column<int>(type: "int", nullable: false),
                    HoraInicioPartidos = table.Column<long>(type: "bigint", nullable: false),
                    SeparacionHorasEntrePartidos = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracionesTorneo", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfiguracionesTorneo");
        }
    }
}
