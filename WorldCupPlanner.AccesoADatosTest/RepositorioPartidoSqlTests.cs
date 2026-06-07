using Microsoft.EntityFrameworkCore;
using WorldCupPlanner.AccesoADatos;
using WorldCupPlanner.AccesoADatos.Repositorios;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.AccesoADatosTest;

[TestClass]
public class RepositorioPartidoSqlTests
{
    private static SqlContext CrearContexto(string nombre)
    {
        var opciones = new DbContextOptionsBuilder<SqlContext>()
            .UseInMemoryDatabase(nombre)
            .Options;
        return new SqlContext(opciones);
    }

    private static (Equipo, Equipo, Estadio) SeedEntidades(SqlContext contexto)
    {
        var equipo1 = new Equipo { Nombre = "Uruguay", Confederacion = Confederacion.CONMEBOL, RankingFIFA = 1000 };
        var equipo2 = new Equipo { Nombre = "Argentina", Confederacion = Confederacion.CONMEBOL, RankingFIFA = 900 };
        var estadio = new Estadio { Nombre = "Estadio Test", Ciudad = "Montevideo", Capacidad = 50000 };
        contexto.Equipos.AddRange(equipo1, equipo2);
        contexto.Estadios.Add(estadio);
        contexto.SaveChanges();
        return (equipo1, equipo2, estadio);
    }

    [TestMethod]
    public void AgregarPartidoSql_NoDebeCambiarIdDeterministico()
    {
        // Arrange
        using var contexto = CrearContexto(nameof(AgregarPartidoSql_NoDebeCambiarIdDeterministico));
        var (equipo1, equipo2, estadio) = SeedEntidades(contexto);
        var partido = new Partido
        {
            Id = 73,
            Fecha = new DateTime(2026, 6, 1),
            Estadio = estadio,
            EquipoLocal = equipo1,
            EquipoVisitante = equipo2,
            Fase = FaseTorneo.FaseDeGrupos
        };
        var repositorio = new RepositorioPartidoSql(contexto);

        // Act
        repositorio.Agregar(partido);

        // Assert
        using var contextoVerificacion = CrearContexto(nameof(AgregarPartidoSql_NoDebeCambiarIdDeterministico));
        var repositorioVerificacion = new RepositorioPartidoSql(contextoVerificacion);
        var partidos = repositorioVerificacion.Buscar(p => p.Id == 73);
        Assert.AreEqual(1, partidos.Count);
        Assert.AreEqual(73, partidos[0].Id);
    }

    [TestMethod]
    public void AgregarPartidoSql_DebePersistirEquiposYEstadio()
    {
        // Arrange
        using var contexto = CrearContexto(nameof(AgregarPartidoSql_DebePersistirEquiposYEstadio));
        var (equipo1, equipo2, estadio) = SeedEntidades(contexto);
        var partido = new Partido
        {
            Id = 1,
            Fecha = new DateTime(2026, 6, 1),
            Estadio = estadio,
            EquipoLocal = equipo1,
            EquipoVisitante = equipo2,
            Fase = FaseTorneo.FaseDeGrupos
        };
        var repositorio = new RepositorioPartidoSql(contexto);

        // Act
        repositorio.Agregar(partido);

        // Assert
        using var contextoVerificacion = CrearContexto(nameof(AgregarPartidoSql_DebePersistirEquiposYEstadio));
        var repositorioVerificacion = new RepositorioPartidoSql(contextoVerificacion);
        var partidos = repositorioVerificacion.Buscar(p => p.Id == 1);
        Assert.AreEqual(1, partidos.Count);
        Assert.AreEqual("Uruguay", partidos[0].EquipoLocal!.Nombre);
        Assert.AreEqual("Argentina", partidos[0].EquipoVisitante!.Nombre);
        Assert.AreEqual("Estadio Test", partidos[0].Estadio!.Nombre);
    }

    [TestMethod]
    public void ActualizarResultadoSql_DebeGuardarGoles()
    {
        // Arrange
        using var contexto = CrearContexto(nameof(ActualizarResultadoSql_DebeGuardarGoles));
        var (equipo1, equipo2, estadio) = SeedEntidades(contexto);
        var partido = new Partido
        {
            Id = 5,
            Fecha = new DateTime(2026, 6, 1),
            Estadio = estadio,
            EquipoLocal = equipo1,
            EquipoVisitante = equipo2,
            Fase = FaseTorneo.FaseDeGrupos
        };
        var repositorio = new RepositorioPartidoSql(contexto);
        repositorio.Agregar(partido);

        // Act
        partido.GolesLocal = 2;
        partido.GolesVisitante = 1;
        repositorio.Actualizar(partido);

        // Assert
        using var contextoVerificacion = CrearContexto(nameof(ActualizarResultadoSql_DebeGuardarGoles));
        var repositorioVerificacion = new RepositorioPartidoSql(contextoVerificacion);
        var resultado = repositorioVerificacion.Buscar(p => p.Id == 5);
        Assert.AreEqual(1, resultado.Count);
        Assert.AreEqual(2, resultado[0].GolesLocal);
        Assert.AreEqual(1, resultado[0].GolesVisitante);
    }

    [TestMethod]
    public void ActualizarResultadoSql_ConInstanciaDesconectada_DebeGuardarGoles()
    {
        // Arrange
        const string nombreBase = nameof(ActualizarResultadoSql_ConInstanciaDesconectada_DebeGuardarGoles);
        int equipoLocalId;
        int equipoVisitanteId;
        int estadioId;

        using (var contexto = CrearContexto(nombreBase))
        {
            var (equipo1, equipo2, estadio) = SeedEntidades(contexto);
            var repositorio = new RepositorioPartidoSql(contexto);
            repositorio.Agregar(new Partido
            {
                Id = 9,
                Fecha = new DateTime(2026, 6, 1),
                Estadio = estadio,
                EquipoLocal = equipo1,
                EquipoVisitante = equipo2,
                Fase = FaseTorneo.FaseDeGrupos
            });

            equipoLocalId = equipo1.Id;
            equipoVisitanteId = equipo2.Id;
            estadioId = estadio.Id;
        }

        var partidoSimulado = new Partido
        {
            Id = 9,
            Fecha = new DateTime(2026, 6, 1),
            Estadio = new Estadio { Id = estadioId, Nombre = "Estadio Test" },
            EquipoLocal = new Equipo { Id = equipoLocalId, Nombre = "Uruguay" },
            EquipoVisitante = new Equipo { Id = equipoVisitanteId, Nombre = "Argentina" },
            Fase = FaseTorneo.FaseDeGrupos,
            GolesLocal = 3,
            GolesVisitante = 2
        };

        // Act
        using (var contextoActualizacion = CrearContexto(nombreBase))
        {
            var repositorio = new RepositorioPartidoSql(contextoActualizacion);
            repositorio.Actualizar(partidoSimulado);
        }

        // Assert
        using var contextoVerificacion = CrearContexto(nombreBase);
        var repositorioVerificacion = new RepositorioPartidoSql(contextoVerificacion);
        var resultado = repositorioVerificacion.Buscar(p => p.Id == 9);
        Assert.AreEqual(1, resultado.Count);
        Assert.AreEqual(3, resultado[0].GolesLocal);
        Assert.AreEqual(2, resultado[0].GolesVisitante);
    }
}
