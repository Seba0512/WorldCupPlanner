using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorldCupPlanner.AccesoADatos.Migraciones
{
    /// <inheritdoc />
    public partial class AgregarPartidos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Partidos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EstadioNombre = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    EquipoLocalNombre = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    EquipoVisitanteNombre = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    Fase = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GolesLocal = table.Column<int>(type: "int", nullable: true),
                    GolesVisitante = table.Column<int>(type: "int", nullable: true),
                    GrupoEtiqueta = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    GanadorPorPenalesNombre = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Partidos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Partidos_Equipos_EquipoLocalNombre",
                        column: x => x.EquipoLocalNombre,
                        principalTable: "Equipos",
                        principalColumn: "Nombre");
                    table.ForeignKey(
                        name: "FK_Partidos_Equipos_EquipoVisitanteNombre",
                        column: x => x.EquipoVisitanteNombre,
                        principalTable: "Equipos",
                        principalColumn: "Nombre");
                    table.ForeignKey(
                        name: "FK_Partidos_Equipos_GanadorPorPenalesNombre",
                        column: x => x.GanadorPorPenalesNombre,
                        principalTable: "Equipos",
                        principalColumn: "Nombre");
                    table.ForeignKey(
                        name: "FK_Partidos_Estadios_EstadioNombre",
                        column: x => x.EstadioNombre,
                        principalTable: "Estadios",
                        principalColumn: "Nombre");
                    table.ForeignKey(
                        name: "FK_Partidos_Grupos_GrupoEtiqueta",
                        column: x => x.GrupoEtiqueta,
                        principalTable: "Grupos",
                        principalColumn: "Etiqueta");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Partidos_EquipoLocalNombre",
                table: "Partidos",
                column: "EquipoLocalNombre");

            migrationBuilder.CreateIndex(
                name: "IX_Partidos_EquipoVisitanteNombre",
                table: "Partidos",
                column: "EquipoVisitanteNombre");

            migrationBuilder.CreateIndex(
                name: "IX_Partidos_EstadioNombre",
                table: "Partidos",
                column: "EstadioNombre");

            migrationBuilder.CreateIndex(
                name: "IX_Partidos_GanadorPorPenalesNombre",
                table: "Partidos",
                column: "GanadorPorPenalesNombre");

            migrationBuilder.CreateIndex(
                name: "IX_Partidos_GrupoEtiqueta",
                table: "Partidos",
                column: "GrupoEtiqueta");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Partidos");
        }
    }
}
