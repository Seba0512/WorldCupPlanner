using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorldCupPlanner.AccesoADatos.Migraciones
{
    /// <inheritdoc />
    public partial class PartidoIdDeterministico : Migration
    {
        // SQL Server no permite quitar IDENTITY via ALTER COLUMN.
        // Solución: renombrar la tabla, crear la nueva sin IDENTITY y copiar los datos.

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex("IX_Partidos_EquipoLocalId", "Partidos");
            migrationBuilder.DropIndex("IX_Partidos_EquipoVisitanteId", "Partidos");
            migrationBuilder.DropIndex("IX_Partidos_EstadioId", "Partidos");
            migrationBuilder.DropIndex("IX_Partidos_GanadorPorPenalesId", "Partidos");
            migrationBuilder.DropIndex("IX_Partidos_GrupoId", "Partidos");

            migrationBuilder.RenameTable(name: "Partidos", newName: "Partidos_Temp");

            migrationBuilder.CreateTable(
                name: "Partidos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EstadioId = table.Column<int>(type: "int", nullable: true),
                    EquipoLocalId = table.Column<int>(type: "int", nullable: true),
                    EquipoVisitanteId = table.Column<int>(type: "int", nullable: true),
                    Fase = table.Column<int>(type: "int", nullable: false),
                    GolesLocal = table.Column<int>(type: "int", nullable: true),
                    GolesVisitante = table.Column<int>(type: "int", nullable: true),
                    GrupoId = table.Column<int>(type: "int", nullable: true),
                    GanadorPorPenalesId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Partidos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Partidos_Equipos_EquipoLocalId",
                        column: x => x.EquipoLocalId,
                        principalTable: "Equipos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Partidos_Equipos_EquipoVisitanteId",
                        column: x => x.EquipoVisitanteId,
                        principalTable: "Equipos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Partidos_Equipos_GanadorPorPenalesId",
                        column: x => x.GanadorPorPenalesId,
                        principalTable: "Equipos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Partidos_Estadios_EstadioId",
                        column: x => x.EstadioId,
                        principalTable: "Estadios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Partidos_Grupos_GrupoId",
                        column: x => x.GrupoId,
                        principalTable: "Grupos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.Sql(
                "INSERT INTO [Partidos] (Id, Fecha, EstadioId, EquipoLocalId, EquipoVisitanteId, Fase, GolesLocal, GolesVisitante, GrupoId, GanadorPorPenalesId) " +
                "SELECT Id, Fecha, EstadioId, EquipoLocalId, EquipoVisitanteId, Fase, GolesLocal, GolesVisitante, GrupoId, GanadorPorPenalesId " +
                "FROM [Partidos_Temp]");

            migrationBuilder.DropTable("Partidos_Temp");

            migrationBuilder.CreateIndex("IX_Partidos_EquipoLocalId", "Partidos", "EquipoLocalId");
            migrationBuilder.CreateIndex("IX_Partidos_EquipoVisitanteId", "Partidos", "EquipoVisitanteId");
            migrationBuilder.CreateIndex("IX_Partidos_EstadioId", "Partidos", "EstadioId");
            migrationBuilder.CreateIndex("IX_Partidos_GanadorPorPenalesId", "Partidos", "GanadorPorPenalesId");
            migrationBuilder.CreateIndex("IX_Partidos_GrupoId", "Partidos", "GrupoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex("IX_Partidos_EquipoLocalId", "Partidos");
            migrationBuilder.DropIndex("IX_Partidos_EquipoVisitanteId", "Partidos");
            migrationBuilder.DropIndex("IX_Partidos_EstadioId", "Partidos");
            migrationBuilder.DropIndex("IX_Partidos_GanadorPorPenalesId", "Partidos");
            migrationBuilder.DropIndex("IX_Partidos_GrupoId", "Partidos");

            migrationBuilder.RenameTable(name: "Partidos", newName: "Partidos_Temp");

            migrationBuilder.CreateTable(
                name: "Partidos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EstadioId = table.Column<int>(type: "int", nullable: true),
                    EquipoLocalId = table.Column<int>(type: "int", nullable: true),
                    EquipoVisitanteId = table.Column<int>(type: "int", nullable: true),
                    Fase = table.Column<int>(type: "int", nullable: false),
                    GolesLocal = table.Column<int>(type: "int", nullable: true),
                    GolesVisitante = table.Column<int>(type: "int", nullable: true),
                    GrupoId = table.Column<int>(type: "int", nullable: true),
                    GanadorPorPenalesId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Partidos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Partidos_Equipos_EquipoLocalId",
                        column: x => x.EquipoLocalId,
                        principalTable: "Equipos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Partidos_Equipos_EquipoVisitanteId",
                        column: x => x.EquipoVisitanteId,
                        principalTable: "Equipos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Partidos_Equipos_GanadorPorPenalesId",
                        column: x => x.GanadorPorPenalesId,
                        principalTable: "Equipos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Partidos_Estadios_EstadioId",
                        column: x => x.EstadioId,
                        principalTable: "Estadios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Partidos_Grupos_GrupoId",
                        column: x => x.GrupoId,
                        principalTable: "Grupos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.Sql(
                "SET IDENTITY_INSERT [Partidos] ON; " +
                "INSERT INTO [Partidos] (Id, Fecha, EstadioId, EquipoLocalId, EquipoVisitanteId, Fase, GolesLocal, GolesVisitante, GrupoId, GanadorPorPenalesId) " +
                "SELECT Id, Fecha, EstadioId, EquipoLocalId, EquipoVisitanteId, Fase, GolesLocal, GolesVisitante, GrupoId, GanadorPorPenalesId " +
                "FROM [Partidos_Temp]; " +
                "SET IDENTITY_INSERT [Partidos] OFF;");

            migrationBuilder.DropTable("Partidos_Temp");

            migrationBuilder.CreateIndex("IX_Partidos_EquipoLocalId", "Partidos", "EquipoLocalId");
            migrationBuilder.CreateIndex("IX_Partidos_EquipoVisitanteId", "Partidos", "EquipoVisitanteId");
            migrationBuilder.CreateIndex("IX_Partidos_EstadioId", "Partidos", "EstadioId");
            migrationBuilder.CreateIndex("IX_Partidos_GanadorPorPenalesId", "Partidos", "GanadorPorPenalesId");
            migrationBuilder.CreateIndex("IX_Partidos_GrupoId", "Partidos", "GrupoId");
        }
    }
}
