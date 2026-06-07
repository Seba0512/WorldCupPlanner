using WorldCupPlanner.AccesoADatos.Repositorios;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.ServicioTests;

[TestClass]
public class ServicioPersistenciaFixtureTests
{
    [TestMethod]
    public void GuardarFixture_SinDatosPrevios_GuardaGruposYPartidos()
    {
        // Arrange
        var repositorioGrupo = new RepositorioGrupoEnMemoria();
        var repositorioPartido = new RepositorioPartidoEnMemoria();
        var servicio = new ServicioPersistenciaFixture(repositorioGrupo, repositorioPartido);
        var grupo = new Grupo("A");
        var partido = CrearPartido(1, grupo);
        grupo.Partidos.Add(partido);
        var fixture = new Fixture { Grupos = new List<Grupo> { grupo } };

        // Act
        servicio.GuardarFixture(fixture);

        // Assert
        Assert.AreEqual(1, repositorioGrupo.Buscar(_ => true).Count);
        Assert.AreEqual(1, repositorioPartido.Buscar(_ => true).Count);
        Assert.AreSame(grupo, repositorioGrupo.Buscar(_ => true).Single());
        Assert.AreSame(partido, repositorioPartido.Buscar(_ => true).Single());
    }

    [TestMethod]
    public void GuardarFixture_ConGrupoExistente_NoDuplicaGrupo()
    {
        // Arrange
        var repositorioGrupo = new RepositorioGrupoEnMemoria();
        var repositorioPartido = new RepositorioPartidoEnMemoria();
        var servicio = new ServicioPersistenciaFixture(repositorioGrupo, repositorioPartido);
        repositorioGrupo.Agregar(new Grupo("A"));
        var grupoGenerado = new Grupo("A");
        var partido = CrearPartido(1, grupoGenerado);
        grupoGenerado.Partidos.Add(partido);
        var fixture = new Fixture { Grupos = new List<Grupo> { grupoGenerado } };

        // Act
        servicio.GuardarFixture(fixture);

        // Assert
        Assert.AreEqual(1, repositorioGrupo.Buscar(_ => true).Count);
        Assert.AreEqual(1, repositorioPartido.Buscar(_ => true).Count);
    }

    [TestMethod]
    public void GuardarPartidos_ConPartidoExistente_ActualizaSinDuplicar()
    {
        // Arrange
        var repositorioGrupo = new RepositorioGrupoEnMemoria();
        var repositorioPartido = new RepositorioPartidoEnMemoria();
        var servicio = new ServicioPersistenciaFixture(repositorioGrupo, repositorioPartido);
        var grupoOriginal = new Grupo("A");
        var grupoNuevo = new Grupo("B");
        var partidoExistente = CrearPartido(1, grupoOriginal);
        var partidoActualizado = CrearPartido(1, grupoNuevo);
        var ganadorPorPenales = partidoActualizado.EquipoVisitante;

        partidoActualizado.Fecha = new DateTime(2026, 7, 1, 21, 30, 0);
        partidoActualizado.Fase = FaseTorneo.Octavos;
        partidoActualizado.GolesLocal = 2;
        partidoActualizado.GolesVisitante = 2;
        partidoActualizado.GanadorPorPenales = ganadorPorPenales;
        repositorioPartido.Agregar(partidoExistente);

        // Act
        servicio.GuardarPartidos(new[] { partidoActualizado });

        // Assert
        var partidosGuardados = repositorioPartido.Buscar(_ => true);
        var guardado = partidosGuardados.Single();
        Assert.AreSame(partidoExistente, guardado);
        Assert.AreEqual(1, partidosGuardados.Count);
        Assert.AreEqual(partidoActualizado.Fecha, guardado.Fecha);
        Assert.AreSame(partidoActualizado.Estadio, guardado.Estadio);
        Assert.AreSame(partidoActualizado.EquipoLocal, guardado.EquipoLocal);
        Assert.AreSame(partidoActualizado.EquipoVisitante, guardado.EquipoVisitante);
        Assert.AreEqual(FaseTorneo.Octavos, guardado.Fase);
        Assert.AreEqual(2, guardado.GolesLocal);
        Assert.AreEqual(2, guardado.GolesVisitante);
        Assert.AreSame(grupoNuevo, guardado.Grupo);
        Assert.AreSame(ganadorPorPenales, guardado.GanadorPorPenales);
    }

    [TestMethod]
    public void CargarEnEstadoTorneo_ConFinalJugada_EstableceTorneoFinalizadoEnTrue()
    {
        // Arrange
        var repositorioGrupo = new RepositorioGrupoEnMemoria();
        var repositorioPartido = new RepositorioPartidoEnMemoria();
        var servicio = new ServicioPersistenciaFixture(repositorioGrupo, repositorioPartido);
        repositorioGrupo.Agregar(new Grupo("A"));
        repositorioPartido.Agregar(new Partido { Id = 100, Fase = FaseTorneo.Final, GolesLocal = 2, GolesVisitante = 1 });
        var estadoTorneo = new EstadoTorneo();

        // Act
        servicio.CargarEnEstadoTorneo(estadoTorneo);

        // Assert
        Assert.IsTrue(estadoTorneo.TorneoFinalizado);
    }

    [TestMethod]
    public void CargarEnEstadoTorneo_SinFinalJugada_TorneoNoFinalizado()
    {
        // Arrange
        var repositorioGrupo = new RepositorioGrupoEnMemoria();
        var repositorioPartido = new RepositorioPartidoEnMemoria();
        var servicio = new ServicioPersistenciaFixture(repositorioGrupo, repositorioPartido);
        repositorioGrupo.Agregar(new Grupo("A"));
        var estadoTorneo = new EstadoTorneo();

        // Act
        servicio.CargarEnEstadoTorneo(estadoTorneo);

        // Assert
        Assert.IsFalse(estadoTorneo.TorneoFinalizado);
    }

    [TestMethod]
    public void LimpiarFixture_ConDatosPrevios_EliminaGruposYPartidos()
    {
        // Arrange
        var repositorioGrupo = new RepositorioGrupoEnMemoria();
        var repositorioPartido = new RepositorioPartidoEnMemoria();
        var servicio = new ServicioPersistenciaFixture(repositorioGrupo, repositorioPartido);
        var grupo = new Grupo("A");
        var partido = CrearPartido(1, grupo);
        grupo.Partidos.Add(partido);
        var fixture = new Fixture { Grupos = new List<Grupo> { grupo } };
        servicio.GuardarFixture(fixture);

        // Act
        servicio.LimpiarFixture();

        // Assert
        Assert.AreEqual(0, repositorioGrupo.Buscar(_ => true).Count);
        Assert.AreEqual(0, repositorioPartido.Buscar(_ => true).Count);
    }

    [TestMethod]
    public void LimpiarFixture_SinDatosPrevios_NoLanzaExcepcion()
    {
        // Arrange
        var repositorioGrupo = new RepositorioGrupoEnMemoria();
        var repositorioPartido = new RepositorioPartidoEnMemoria();
        var servicio = new ServicioPersistenciaFixture(repositorioGrupo, repositorioPartido);

        // Act & Assert
        servicio.LimpiarFixture();
        Assert.AreEqual(0, repositorioGrupo.Buscar(_ => true).Count);
    }

    private static Partido CrearPartido(int id, Grupo grupo)
    {
        return new Partido
        {
            Id = id,
            Fecha = new DateTime(2026, 6, 15, 14, 0, 0),
            Estadio = CrearEstadio($"Estadio {id}"),
            EquipoLocal = CrearEquipo($"Local {id}", Confederacion.UEFA),
            EquipoVisitante = CrearEquipo($"Visitante {id}", Confederacion.CONMEBOL),
            Fase = FaseTorneo.FaseDeGrupos,
            Grupo = grupo
        };
    }

    private static Equipo CrearEquipo(string nombre, Confederacion confederacion)
    {
        return new Equipo
        {
            Nombre = nombre,
            RankingFIFA = 1200,
            Confederacion = confederacion
        };
    }

    private static Estadio CrearEstadio(string nombre)
    {
        return new Estadio
        {
            Nombre = nombre,
            Ciudad = "Montevideo",
            Capacidad = 40000
        };
    }
}
