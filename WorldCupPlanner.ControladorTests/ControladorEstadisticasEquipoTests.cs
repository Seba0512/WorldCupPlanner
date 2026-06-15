using WorldCupPlanner.Controladores;
using WorldCupPlanner.Dominio.DTO;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.ControladorTests;

[TestClass]
public class ControladorEstadisticasEquipoTests
{
    [TestMethod]
    public void ObtenerEstadisticasPorEquipo_DelegaEnServicioEstadisticasEquipo()
    {
        var estado = CrearEstadoTorneo();
        var servicio = CrearServicio(estado);
        var controlador = new ControladorEstadisticasEquipo(servicio);
        var equipos = estado.Fixture!.Grupos[0].Equipos;

        var esperadas = servicio.ObtenerEstadisticasPorEquipo(equipos);
        var obtenidas = controlador.ObtenerEstadisticasPorEquipo(equipos);

        AssertDiccionariosEquivalentes(esperadas, obtenidas);
    }

    [TestMethod]
    public void ObtenerEstadisticasPorEquipo_DesdeDTO_DelegaEnServicioEstadisticasEquipo()
    {
        var estado = CrearEstadoTorneo();
        var servicio = CrearServicio(estado);
        var controlador = new ControladorEstadisticasEquipo(servicio);
        var equipos = estado.Fixture!.Grupos[0].Equipos.Select(EquipoDTO.FromEntity).ToList();

        var estadisticas = controlador.ObtenerEstadisticasPorEquipo(equipos);

        Assert.IsTrue(estadisticas.ContainsKey("Uruguay"));
        Assert.IsTrue(estadisticas.ContainsKey("Francia"));
    }

    [TestMethod]
    public void ObtenerEstadisticasPorEquipo_DevuelveDatosEsperadosParaTablaDeEquipos()
    {
        var estado = CrearEstadoTorneo();
        var controlador = new ControladorEstadisticasEquipo(CrearServicio(estado));
        var equipos = estado.Fixture!.Grupos[0].Equipos;

        var estadisticas = controlador.ObtenerEstadisticasPorEquipo(equipos);
        var uruguay = estadisticas["Uruguay"];
        var japon = estadisticas["Japon"];

        Assert.AreEqual(2, uruguay.PartidosJugados);
        Assert.AreEqual(3, uruguay.GolesAFavor);
        Assert.AreEqual(4, uruguay.GolesEnContra);
        Assert.AreEqual(-1, uruguay.DiferenciaGoles);
        Assert.AreEqual(3, uruguay.Puntos);

        Assert.AreEqual(1, japon.PartidosJugados);
        Assert.AreEqual(3, japon.GolesAFavor);
        Assert.AreEqual(1, japon.GolesEnContra);
        Assert.AreEqual(2, japon.DiferenciaGoles);
        Assert.AreEqual(0, japon.Puntos);
    }

    [TestMethod]
    public void ObtenerEstadisticas_DelegaConsultaPuntualEnServicioEstadisticasEquipo()
    {
        var estado = CrearEstadoTorneo();
        var servicio = CrearServicio(estado);
        var controlador = new ControladorEstadisticasEquipo(servicio);
        var equipo = estado.Fixture!.Grupos[0].Equipos.Single(e => e.Nombre == "Francia");

        var esperada = servicio.ObtenerEstadisticas(equipo);
        var obtenida = controlador.ObtenerEstadisticas(equipo);

        AssertEstadisticasEquivalentes(esperada, obtenida);
    }

    [TestMethod]
    public void ObtenerEstadisticas_EquipoSinPartidos_DevuelveEstadisticasEnCero()
    {
        var estado = CrearEstadoTorneo();
        var controlador = new ControladorEstadisticasEquipo(CrearServicio(estado));
        var equipoSinPartidos = CrearEquipo("Canada", Confederacion.CONCACAF);

        var estadisticas = controlador.ObtenerEstadisticas(equipoSinPartidos);

        Assert.AreEqual(0, estadisticas.PartidosJugados);
        Assert.AreEqual(0, estadisticas.GolesAFavor);
        Assert.AreEqual(0, estadisticas.GolesEnContra);
        Assert.AreEqual(0, estadisticas.DiferenciaGoles);
        Assert.AreEqual(0, estadisticas.Puntos);
    }

    private static ServicioEstadisticasEquipo CrearServicio(EstadoTorneo estado) =>
        new(estado, new ServicioConsultasTorneo());

    private static EstadoTorneo CrearEstadoTorneo()
    {
        var uruguay = CrearEquipo("Uruguay", Confederacion.CONMEBOL);
        var francia = CrearEquipo("Francia", Confederacion.UEFA);
        var japon = CrearEquipo("Japon", Confederacion.AFC);
        var marruecos = CrearEquipo("Marruecos", Confederacion.CAF);
        var estadio = CrearEstadio("Centenario");
        var grupo = new Grupo("A", new List<Equipo> { uruguay, francia, japon, marruecos });

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
                    Fecha = new DateTime(2026, 7, 4, 18, 0, 0),
                    Fase = FaseTorneo.Octavos,
                    EquipoLocal = uruguay,
                    EquipoVisitante = japon,
                    Estadio = estadio,
                    GolesLocal = 1,
                    GolesVisitante = 3
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

    private static void AssertDiccionariosEquivalentes(
        Dictionary<string, EstadisticasEquipo> esperado,
        Dictionary<string, EstadisticasEquipo> obtenido)
    {
        CollectionAssert.AreEquivalent(esperado.Keys.ToList(), obtenido.Keys.ToList());

        foreach (var nombreEquipo in esperado.Keys)
        {
            AssertEstadisticasEquivalentes(esperado[nombreEquipo], obtenido[nombreEquipo]);
        }
    }

    private static void AssertEstadisticasEquivalentes(EstadisticasEquipo esperada, EstadisticasEquipo obtenida)
    {
        Assert.AreEqual(esperada.PartidosJugados, obtenida.PartidosJugados);
        Assert.AreEqual(esperada.GolesAFavor, obtenida.GolesAFavor);
        Assert.AreEqual(esperada.GolesEnContra, obtenida.GolesEnContra);
        Assert.AreEqual(esperada.DiferenciaGoles, obtenida.DiferenciaGoles);
        Assert.AreEqual(esperada.Puntos, obtenida.Puntos);
    }
}

