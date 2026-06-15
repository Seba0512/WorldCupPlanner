using WorldCupPlanner.Controladores;
using WorldCupPlanner.Dominio.DTO;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.ControladorTests;

[TestClass]
public class ControladorConsultasTorneoTests
{
    [TestMethod]
    public void ObtenerTodosLosPartidos_DevuelveGruposYEliminatorias()
    {
        var controlador = CrearControlador();
        var estado = CrearEstadoTorneo();

        var partidos = controlador.ObtenerTodosLosPartidos(estado);

        Assert.AreEqual(2, partidos.Count);
        Assert.IsTrue(partidos.Any(p => p.Fase == FaseTorneo.FaseDeGrupos));
        Assert.IsTrue(partidos.Any(p => p.Fase == FaseTorneo.Final));
    }

    [TestMethod]
    public void ObtenerPartidosDeFase_DevuelveSoloLaFaseSolicitada()
    {
        var controlador = CrearControlador();
        var estado = CrearEstadoTorneo();

        var finales = controlador.ObtenerPartidosDeFase(estado, FaseTorneo.Final);

        Assert.AreEqual(1, finales.Count);
        Assert.AreEqual(FaseTorneo.Final, finales[0].Fase);
    }

    [TestMethod]
    public void Contadores_DelegaTotalesYJugados()
    {
        var controlador = CrearControlador();
        var estado = CrearEstadoTorneo();

        Assert.AreEqual(2, controlador.ContarTotalPartidos(estado));
        Assert.AreEqual(1, controlador.ContarPartidosJugados(estado));
    }

    [TestMethod]
    public void ParticipacionEquipoYEstadio_MantieneComportamientoDelServicio()
    {
        var controlador = CrearControlador();
        var estado = CrearEstadoTorneo();
        var equipoParticipante = CrearEquipo("Uruguay", Confederacion.CONMEBOL);
        var equipoAjeno = CrearEquipo("Canada", Confederacion.CONCACAF);
        var estadioParticipante = CrearEstadio("Centenario");
        var estadioAjeno = CrearEstadio("Otro");

        Assert.IsTrue(controlador.EquipoParticipaEnFixture(estado, equipoParticipante));
        Assert.IsFalse(controlador.EquipoParticipaEnFixture(estado, equipoAjeno));
        Assert.IsTrue(controlador.EstadioParticipaEnFixture(estado, estadioParticipante));
        Assert.IsFalse(controlador.EstadioParticipaEnFixture(estado, estadioAjeno));
    }

    [TestMethod]
    public void ParticipacionEquipoYEstadio_AceptaDTOsSinExigirEntidadesEnUI()
    {
        var controlador = CrearControlador();
        var estado = CrearEstadoTorneo();

        Assert.IsTrue(controlador.EquipoParticipaEnFixture(estado, new EquipoDTO
        {
            Nombre = "Uruguay",
            Confederacion = Confederacion.CONMEBOL,
            RankingFIFA = 1200
        }));
        Assert.IsTrue(controlador.EstadioParticipaEnFixture(estado, new EstadioDTO
        {
            Nombre = "Centenario",
            Ciudad = "Montevideo",
            Capacidad = 60000
        }));
    }

    [TestMethod]
    public void ObtenerFaseConfiguracion_DelegaResultadoDelServicio()
    {
        var controlador = CrearControlador();

        Assert.AreEqual(
            FaseConfiguracionTorneo.Incompleto,
            controlador.ObtenerFaseConfiguracion(new EstadoTorneo(), 10));
        Assert.AreEqual(
            FaseConfiguracionTorneo.ListoParaFixture,
            controlador.ObtenerFaseConfiguracion(new EstadoTorneo(), 48));
        Assert.AreEqual(
            FaseConfiguracionTorneo.FixtureGenerado,
            controlador.ObtenerFaseConfiguracion(CrearEstadoTorneo(), 48));
    }

    [TestMethod]
    public void ObtenerTodosLosEquipos_DevuelveEquiposDelFixture()
    {
        var controlador = CrearControlador();
        var estado = CrearEstadoTorneo();

        var equipos = controlador.ObtenerTodosLosEquipos(estado).Select(e => e.Nombre).ToList();

        CollectionAssert.Contains(equipos, "Uruguay");
        CollectionAssert.Contains(equipos, "Francia");
        CollectionAssert.Contains(equipos, "Japon");
    }

    private static ControladorConsultasTorneo CrearControlador() =>
        new(new ServicioConsultasTorneo());

    private static EstadoTorneo CrearEstadoTorneo()
    {
        var uruguay = CrearEquipo("Uruguay", Confederacion.CONMEBOL);
        var francia = CrearEquipo("Francia", Confederacion.UEFA);
        var japon = CrearEquipo("Japon", Confederacion.AFC);
        var marruecos = CrearEquipo("Marruecos", Confederacion.CAF);
        var estadio = CrearEstadio("Centenario");
        var grupo = new Grupo("A", [uruguay, francia, japon, marruecos]);
        grupo.Partidos.Add(new Partido
        {
            Id = 1,
            Fecha = new DateTime(2026, 6, 1, 14, 0, 0),
            Fase = FaseTorneo.FaseDeGrupos,
            Grupo = grupo,
            EquipoLocal = uruguay,
            EquipoVisitante = francia,
            Estadio = estadio,
            GolesLocal = 2,
            GolesVisitante = 1
        });

        return new EstadoTorneo
        {
            Fixture = new Fixture { Grupos = { grupo } },
            PartidosEliminatorios =
            [
                new Partido
                {
                    Id = 2,
                    Fecha = new DateTime(2026, 7, 19, 14, 0, 0),
                    Fase = FaseTorneo.Final,
                    EquipoLocal = uruguay,
                    EquipoVisitante = japon,
                    Estadio = estadio
                }
            ]
        };
    }

    private static Equipo CrearEquipo(string nombre, Confederacion confederacion) => new()
    {
        Nombre = nombre,
        Confederacion = confederacion,
        RankingFIFA = 1200
    };

    private static Estadio CrearEstadio(string nombre) => new()
    {
        Nombre = nombre,
        Ciudad = "Montevideo",
        Capacidad = 60000
    };
}

