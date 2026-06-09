using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorldCupPlanner.AccesoADatos.Migrations
{
    [DbContext(typeof(SqlContext))]
    [Migration("20260609130000_AgregarMotorSimulacion")]
    public partial class AgregarMotorSimulacion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MotorSimulacion",
                table: "ConfiguracionesTorneo",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MotorSimulacion",
                table: "ConfiguracionesTorneo");
        }
    }
}
