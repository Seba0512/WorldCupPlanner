using WorldCupPlanner.Controladores;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Dominio.Excepciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.ServicioTests;

[TestClass]
public class ControladorFixtureTests
{
    [TestMethod]
    public void GenerarFixture_DelegaEnServicioFixture()
    {
        var servicio = new ServicioFixture();
        var controlador = new ControladorFixture(servicio);
        var equipos = CrearEquiposValidos();
        var estadios = CrearEstadiosValidos();
        var configuracion = CrearConfiguracion();

        var fixture = controlador.GenerarFixture(equipos, estadios, configuracion);

        Assert.AreEqual(12, fixture.Grupos.Count);
        Assert.AreEqual(72, fixture.Partidos.Count);
        Assert.IsTrue(fixture.Grupos.All(g => g.EsValido()));
    }

    [TestMethod]
    public void RegenerarFixture_DelegaEnServicioFixtureYPropagaValidaciones()
    {
        var servicio = new ServicioFixture();
        var controlador = new ControladorFixture(servicio);
        var equipos = CrearEquiposValidos();
        var estadios = CrearEstadiosValidos();
        var configuracion = CrearConfiguracion();
        var fixtureActual = servicio.GenerarFixture(equipos, estadios, configuracion);
        fixtureActual.Partidos[0].GolesLocal = 1;
        fixtureActual.Partidos[0].GolesVisitante = 0;

        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            controlador.RegenerarFixture(equipos, fixtureActual, estadios, configuracion));
    }

    [TestMethod]
    public void Fases_DelegaEstadoDeGeneracionYPermisosEnServicioFixture()
    {
        var servicio = new ServicioFixture();
        var controlador = new ControladorFixture(servicio);
        var estado = CrearEstadoConFixtureDeGruposCompleto();

        Assert.AreEqual(
            servicio.FaseYaGenerada(FaseTorneo.PrimeraRondaEliminatoria, estado),
            controlador.FaseYaGenerada(FaseTorneo.PrimeraRondaEliminatoria, estado));
        Assert.AreEqual(
            servicio.PuedeGenerarFase(FaseTorneo.PrimeraRondaEliminatoria, estado),
            controlador.PuedeGenerarFase(FaseTorneo.PrimeraRondaEliminatoria, estado));
        Assert.AreEqual(
            servicio.ObtenerMensajeCondicionFase(FaseTorneo.PrimeraRondaEliminatoria, estado),
            controlador.ObtenerMensajeCondicionFase(FaseTorneo.PrimeraRondaEliminatoria, estado));
    }

    [TestMethod]
    public void Simulacion_DelegaConsultasDeFaseJornadaYPartidos()
    {
        var servicio = new ServicioFixture();
        var controlador = new ControladorFixture(servicio);
        var (estado, proximoPartido, segundoPartido) = CrearEstadoConDosPartidosPendientes();

        Assert.AreEqual(
            servicio.ObtenerFaseActualSimulable(estado),
            controlador.ObtenerFaseActualSimulable(estado));
        Assert.AreEqual(
            servicio.ObtenerJornadaActivaParaFase(FaseTorneo.FaseDeGrupos, estado.Fixture),
            controlador.ObtenerJornadaActivaParaFase(FaseTorneo.FaseDeGrupos, estado.Fixture));
        Assert.AreEqual(
            servicio.EsJornadaSeleccionable(ServicioFixture.PrimeraJornada, estado.Fixture),
            controlador.EsJornadaSeleccionable(ServicioFixture.PrimeraJornada, estado.Fixture));
        Assert.AreSame(
            servicio.ObtenerProximoPartidoSimulable(estado, FaseTorneo.FaseDeGrupos),
            controlador.ObtenerProximoPartidoSimulable(estado, FaseTorneo.FaseDeGrupos));
        Assert.IsTrue(controlador.EsPartidoSimulable(proximoPartido, estado, FaseTorneo.FaseDeGrupos));
        Assert.IsFalse(controlador.EsPartidoSimulable(segundoPartido, estado, FaseTorneo.FaseDeGrupos));
    }

    [TestMethod]
    public void ObtenerPartidos_DelegaPendientesEtapaYJornadaOFecha()
    {
        var servicio = new ServicioFixture();
        var controlador = new ControladorFixture(servicio);
        var (estado, _, _) = CrearEstadoConDosPartidosPendientes();

        var pendientesEsperados = servicio.ObtenerPartidosPendientesEtapaActiva(estado, FaseTorneo.FaseDeGrupos);
        var pendientesObtenidos = controlador.ObtenerPartidosPendientesEtapaActiva(estado, FaseTorneo.FaseDeGrupos);
        var jornadaEsperada = servicio.ObtenerPartidosJornadaOFechaActual(
            estado,
            FaseTorneo.FaseDeGrupos,
            ServicioFixture.PrimeraJornada);
        var jornadaObtenida = controlador.ObtenerPartidosJornadaOFechaActual(
            estado,
            FaseTorneo.FaseDeGrupos,
            ServicioFixture.PrimeraJornada);

        CollectionAssert.AreEqual(pendientesEsperados, pendientesObtenidos);
        CollectionAssert.AreEqual(jornadaEsperada, jornadaObtenida);
    }

    [TestMethod]
    public void Mensajes_DelegaTextosDeExitoEnServicioFixture()
    {
        var servicio = new ServicioFixture();
        var controlador = new ControladorFixture(servicio);

        Assert.AreEqual(
            servicio.ObtenerMensajeExitoSimulacion(FaseTorneo.FaseDeGrupos),
            controlador.ObtenerMensajeExitoSimulacion(FaseTorneo.FaseDeGrupos));
        Assert.AreEqual(
            servicio.ObtenerMensajeExitoGeneracionFase(FaseTorneo.Final),
            controlador.ObtenerMensajeExitoGeneracionFase(FaseTorneo.Final));
    }

    private static ConfiguracionTorneo CrearConfiguracion() => new()
    {
        SemillaFixture = 1,
        FechaInicioTorneo = new DateTime(2026, 6, 1),
        MaxPartidosPorDia = 3,
        SeparacionEntreFechas = 3,
        HoraInicioPartidos = new TimeSpan(14, 0, 0),
        SeparacionHorasEntrePartidos = 4
    };

    private static List<Estadio> CrearEstadiosValidos() =>
    [
        CrearEstadio("Estadio1"),
        CrearEstadio("Estadio2"),
        CrearEstadio("Estadio3"),
        CrearEstadio("Estadio4")
    ];

    private static List<Equipo> CrearEquiposValidos()
    {
        var distribucion = new (Confederacion Confederacion, int Cantidad)[]
        {
            (Confederacion.UEFA, 16),
            (Confederacion.CONMEBOL, 7),
            (Confederacion.CONCACAF, 7),
            (Confederacion.CAF, 9),
            (Confederacion.AFC, 8),
            (Confederacion.OFC, 1)
        };

        var equipos = new List<Equipo>();
        var contador = 1;

        foreach (var (confederacion, cantidad) in distribucion)
        {
            for (var i = 0; i < cantidad; i++)
            {
                equipos.Add(new Equipo
                {
                    Id = contador,
                    Nombre = $"Equipo{contador:00}",
                    Confederacion = confederacion,
                    RankingFIFA = 2500 - contador
                });
                contador++;
            }
        }

        return equipos;
    }

    private static EstadoTorneo CrearEstadoConFixtureDeGruposCompleto()
    {
        var (estado, _, _) = CrearEstadoConDosPartidosPendientes();

        foreach (var partido in estado.Fixture!.Partidos)
        {
            partido.GolesLocal = 1;
            partido.GolesVisitante = 0;
        }

        return estado;
    }

    private static (EstadoTorneo Estado, Partido ProximoPartido, Partido SegundoPartido)
        CrearEstadoConDosPartidosPendientes()
    {
        var estadio = CrearEstadio("Estadio");
        var grupo = new Grupo("A", new List<Equipo>
        {
            CrearEquipo("E1", Confederacion.UEFA, 1),
            CrearEquipo("E2", Confederacion.UEFA, 2),
            CrearEquipo("E3", Confederacion.CONMEBOL, 3),
            CrearEquipo("E4", Confederacion.CAF, 4)
        });

        var proximoPartido = CrearPartido(
            grupo.Equipos[0],
            grupo.Equipos[1],
            estadio,
            new DateTime(2026, 6, 1),
            1);
        var segundoPartido = CrearPartido(
            grupo.Equipos[2],
            grupo.Equipos[3],
            estadio,
            new DateTime(2026, 6, 2),
            2);

        grupo.Partidos.Add(proximoPartido);
        grupo.Partidos.Add(segundoPartido);

        var estado = new EstadoTorneo();
        estado.EstablecerFixture(new Fixture { Grupos = { grupo } });

        return (estado, proximoPartido, segundoPartido);
    }

    private static Equipo CrearEquipo(string nombre, Confederacion confederacion, int id) => new()
    {
        Id = id,
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

    private static Partido CrearPartido(
        Equipo local,
        Equipo visitante,
        Estadio estadio,
        DateTime fecha,
        int id) => new()
    {
        Id = id,
        EquipoLocal = local,
        EquipoVisitante = visitante,
        Estadio = estadio,
        Fecha = fecha,
        Fase = FaseTorneo.FaseDeGrupos
    };
}
