using Microsoft.EntityFrameworkCore;
using WorldCupPlanner.AccesoADatos;

namespace WorldCupPlanner.AccesoADatosTest;

internal static class SqlContextFactory
{
    internal static SqlContext Crear(string nombre) =>
        new SqlContext(
            new DbContextOptionsBuilder<SqlContext>()
                .UseInMemoryDatabase(nombre)
                .Options
        );
}
