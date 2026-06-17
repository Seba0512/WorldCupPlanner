using WorldCupPlanner.AccesoADatos;
using WorldCupPlanner.AccesoADatos.Repositorios;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.AccesoADatosTest;

[TestClass]
public class RepositorioIncidenciaSqlTests
{
    private static Equipo SeedEquipo(SqlContext contexto, string nombre = "Uruguay")
    {
        var equipo = new Equipo { Nombre = nombre, Confederacion = Confederacion.CONMEBOL, RankingFIFA = 1000 };
        contexto.Equipos.Add(equipo);
        contexto.SaveChanges();
        return equipo;
    }

    [TestMethod]
    public void AgregarIncidenciaSql_DebePersistirTarjetaAmarillaConEquipoYPartidoId()
    {
        // Arrange
        using var contexto = SqlContextFactory.Crear(nameof(AgregarIncidenciaSql_DebePersistirTarjetaAmarillaConEquipoYPartidoId));
        var equipo = SeedEquipo(contexto);
        var repositorio = new RepositorioIncidenciaSql(contexto);
        var tarjeta = new TarjetaAmarilla { MinutoDelPartido = 30, Equipo = equipo, PartidoId = 1 };

        // Act
        repositorio.Agregar(tarjeta);

        // Assert
        using var contextoVerificacion = SqlContextFactory.Crear(nameof(AgregarIncidenciaSql_DebePersistirTarjetaAmarillaConEquipoYPartidoId));
        var repositorioVerificacion = new RepositorioIncidenciaSql(contextoVerificacion);
        var incidencias = repositorioVerificacion.ObtenerPorPartido(1);
        Assert.AreEqual(1, incidencias.Count);
        Assert.IsInstanceOfType(incidencias[0], typeof(TarjetaAmarilla));
        Assert.AreEqual(30, incidencias[0].MinutoDelPartido);
        Assert.AreEqual("Uruguay", incidencias[0].Equipo!.Nombre);
        Assert.AreEqual(1, incidencias[0].PartidoId);
    }

    [TestMethod]
    public void AgregarIncidenciaSql_DebePersistirTarjetaRoja()
    {
        // Arrange
        using var contexto = SqlContextFactory.Crear(nameof(AgregarIncidenciaSql_DebePersistirTarjetaRoja));
        var equipo = SeedEquipo(contexto, "Brasil");
        var repositorio = new RepositorioIncidenciaSql(contexto);
        var tarjeta = new TarjetaRoja { MinutoDelPartido = 75, Equipo = equipo, PartidoId = 2 };

        // Act
        repositorio.Agregar(tarjeta);

        // Assert
        using var contextoVerificacion = SqlContextFactory.Crear(nameof(AgregarIncidenciaSql_DebePersistirTarjetaRoja));
        var repositorioVerificacion = new RepositorioIncidenciaSql(contextoVerificacion);
        var incidencias = repositorioVerificacion.ObtenerPorPartido(2);
        Assert.AreEqual(1, incidencias.Count);
        Assert.IsInstanceOfType(incidencias[0], typeof(TarjetaRoja));
        Assert.AreEqual(75, incidencias[0].MinutoDelPartido);
    }

    [TestMethod]
    public void ObtenerPorPartidoSql_DebeRetornarSoloIncidenciasDelPartido()
    {
        // Arrange
        using var contexto = SqlContextFactory.Crear(nameof(ObtenerPorPartidoSql_DebeRetornarSoloIncidenciasDelPartido));
        var equipo = SeedEquipo(contexto);
        var repositorio = new RepositorioIncidenciaSql(contexto);
        repositorio.Agregar(new TarjetaAmarilla { MinutoDelPartido = 10, Equipo = equipo, PartidoId = 1 });
        repositorio.Agregar(new TarjetaRoja { MinutoDelPartido = 55, Equipo = equipo, PartidoId = 1 });
        repositorio.Agregar(new TarjetaAmarilla { MinutoDelPartido = 30, Equipo = equipo, PartidoId = 99 });

        // Act
        var resultado = repositorio.ObtenerPorPartido(1);

        // Assert
        Assert.AreEqual(2, resultado.Count);
        Assert.IsTrue(resultado.All(i => i.PartidoId == 1));
    }

    [TestMethod]
    public void EliminarIncidenciaSql_DebeEliminarCorrectamente()
    {
        // Arrange
        using var contexto = SqlContextFactory.Crear(nameof(EliminarIncidenciaSql_DebeEliminarCorrectamente));
        var equipo = SeedEquipo(contexto);
        var repositorio = new RepositorioIncidenciaSql(contexto);
        var tarjeta = new TarjetaAmarilla { MinutoDelPartido = 40, Equipo = equipo, PartidoId = 1 };
        repositorio.Agregar(tarjeta);

        // Act
        repositorio.Eliminar(tarjeta);

        // Assert
        using var contextoVerificacion = SqlContextFactory.Crear(nameof(EliminarIncidenciaSql_DebeEliminarCorrectamente));
        var repositorioVerificacion = new RepositorioIncidenciaSql(contextoVerificacion);
        Assert.AreEqual(0, repositorioVerificacion.ObtenerPorPartido(1).Count);
    }

    [TestMethod]
    public void AgregarIncidenciaSql_AlRecrearContexto_DebeRecuperarIncidencia()
    {
        // Arrange
        const string nombreBase = nameof(AgregarIncidenciaSql_AlRecrearContexto_DebeRecuperarIncidencia);

        using (var contexto = SqlContextFactory.Crear(nombreBase))
        {
            var equipo = SeedEquipo(contexto);
            var repositorio = new RepositorioIncidenciaSql(contexto);
            repositorio.Agregar(new TarjetaAmarilla
            {
                MinutoDelPartido = 88,
                Equipo = equipo,
                PartidoId = 7
            });
        }

        // Act
        using var contextoNuevo = SqlContextFactory.Crear(nombreBase);
        var repositorioNuevo = new RepositorioIncidenciaSql(contextoNuevo);
        var incidencias = repositorioNuevo.ObtenerPorPartido(7);

        // Assert
        Assert.AreEqual(1, incidencias.Count);
        Assert.AreEqual(88, incidencias[0].MinutoDelPartido);
        Assert.AreEqual(7, incidencias[0].PartidoId);
        Assert.IsInstanceOfType(incidencias[0], typeof(TarjetaAmarilla));
    }

    [TestMethod]
    public void BuscarIncidenciaSql_DebeFiltrarPorPredicado()
    {
        // Arrange
        using var contexto = SqlContextFactory.Crear(nameof(BuscarIncidenciaSql_DebeFiltrarPorPredicado));
        var equipo = SeedEquipo(contexto);
        var repositorio = new RepositorioIncidenciaSql(contexto);
        repositorio.Agregar(new TarjetaAmarilla { MinutoDelPartido = 15, Equipo = equipo, PartidoId = 1 });
        repositorio.Agregar(new TarjetaRoja { MinutoDelPartido = 80, Equipo = equipo, PartidoId = 1 });

        // Act
        var tarjetasRojas = repositorio.Buscar(i => i is TarjetaRoja);

        // Assert
        Assert.AreEqual(1, tarjetasRojas.Count);
        Assert.IsInstanceOfType(tarjetasRojas[0], typeof(TarjetaRoja));
    }
}
