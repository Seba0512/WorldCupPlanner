using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorldCupPlanner.AccesoADatos.Migrations
{
    [DbContext(typeof(SqlContext))]
    [Migration("20260608120000_AgregarRankingInicialEquipo")]
    public partial class AgregarRankingInicialEquipo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RankingInicial",
                table: "Equipos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql("UPDATE Equipos SET RankingInicial = RankingFIFA WHERE RankingInicial = 0");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RankingInicial",
                table: "Equipos");
        }
    }
}
