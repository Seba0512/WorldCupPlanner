using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorldCupPlanner.AccesoADatos.Migraciones
{
    /// <inheritdoc />
    public partial class AgregarGrupos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Grupos",
                columns: table => new
                {
                    Etiqueta = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grupos", x => x.Etiqueta);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Grupos");
        }
    }
}
