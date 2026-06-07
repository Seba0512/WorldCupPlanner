using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorldCupPlanner.AccesoADatos.Migraciones
{
    /// <inheritdoc />
    public partial class AgregarTorneoArchivado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TorneosArchivados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GruposJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PartidosEliminatoriosJson = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TorneosArchivados", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TorneosArchivados");
        }
    }
}
