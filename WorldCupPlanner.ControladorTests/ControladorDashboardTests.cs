using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Controladores;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.ControladorTests;

[TestClass]
public class ControladorDashboardTests
{
    [TestMethod]
    public void ObtenerDatos_DelegaEnServicioDashboard()
    {
        var contexto = CrearContextoConTorneoActivo();
        var servicio = CrearServicio(contexto);
        var controlador = new ControladorDashboard(servicio);

        var esperados = servicio.ObtenerDatos();
        var obtenidos = controlador.ObtenerDatos();

        AssertDatosEquivalentes(esperados, obtenidos);
    }

    [TestMethod]
    public void ObtenerDatos_ConFixtureYCruces_DevuelveDatosEsperadosParaDashboard()
    {
        var contexto = CrearContextoConTorneoActivo();
        var controlador = new ControladorDashboard(CrearServicio(contexto));

        var datos = controlador.ObtenerDatos();

        Assert.AreEqual(4, datos.CantidadEquipos);
        Assert.AreEqual(2, datos.CantidadEstadios);
        Assert.IsTrue(datos.FixtureGenerado);
        Assert.IsTrue(datos.CrucesGenerados);
        Assert.AreEqual(3, datos.TotalPartidos);
        Assert.AreEqual(2, datos.PartidosJugados);
    }

    [TestMethod]
    public void ObtenerDatos_SinFixture_DevuelveTorneoPendienteYSinPartidos()
    {
        var contexto = CrearContextoSinFixture();
        var controlador = new ControladorDashboard(CrearServicio(contexto));

        var datos = controlador.ObtenerDatos();

        Assert.AreEqual(1, datos.CantidadEquipos);
        Assert.AreEqual(1, datos.CantidadEstadios);
        Assert.IsFalse(datos.FixtureGenerado);
        Assert.IsFalse(datos.CrucesGenerados);
        Assert.AreEqual(0, datos.TotalPartidos);
        Assert.AreEqual(0, datos.PartidosJugados);
    }

    private static ServicioDashboard CrearServicio(ContextoDashboardTest contexto)
    {
        var servicioEquipo = new ServicioEquipo(contexto.RepositorioEquipos);
        var servicioEstadio = new ServicioEstadio(contexto.RepositorioEstadios);
        return new ServicioDashboard(
            servicioEquipo,
            servicioEstadio,
            contexto.EstadoTorneo,
            new ServicioConsultasTorneo());
    }

    private static ContextoDashboardTest CrearContextoConTorneoActivo()
    {
        var repositorioEquipos = new RepositorioDashboardFake<Equipo>();
        var repositorioEstadios = new RepositorioDashboardFake<Estadio>();

        var uruguay = CrearEquipo("Uruguay", Confederacion.CONMEBOL);
        var francia = CrearEquipo("Francia", Confederacion.UEFA);
        var japon = CrearEquipo("Japon", Confederacion.AFC);
        var marruecos = CrearEquipo("Marruecos", Confederacion.CAF);
        var centenario = CrearEstadio("Centenario");
        var nacional = CrearEstadio("Nacional");

        foreach (var equipo in new[] { uruguay, francia, japon, marruecos })
            repositorioEquipos.Agregar(equipo);

        repositorioEstadios.Agregar(centenario);
        repositorioEstadios.Agregar(nacional);

        var grupo = new Grupo("A", new List<Equipo> { uruguay, francia, japon, marruecos });
        grupo.Partidos.Add(CrearPartido(uruguay, francia, centenario, FaseTorneo.FaseDeGrupos, 2, 1));
        grupo.Partidos.Add(CrearPartido(japon, marruecos, nacional, FaseTorneo.FaseDeGrupos));

        var estado = new EstadoTorneo
        {
            Fixture = new Fixture { Grupos = { grupo } },
            PartidosEliminatorios =
            [
                CrearPartido(uruguay, japon, centenario, FaseTorneo.Octavos, 3, 0)
            ]
        };

        return new ContextoDashboardTest(repositorioEquipos, repositorioEstadios, estado);
    }

    private static ContextoDashboardTest CrearContextoSinFixture()
    {
        var repositorioEquipos = new RepositorioDashboardFake<Equipo>();
        var repositorioEstadios = new RepositorioDashboardFake<Estadio>();
        repositorioEquipos.Agregar(CrearEquipo("Uruguay", Confederacion.CONMEBOL));
        repositorioEstadios.Agregar(CrearEstadio("Centenario"));

        return new ContextoDashboardTest(repositorioEquipos, repositorioEstadios, new EstadoTorneo());
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

    private static Partido CrearPartido(
        Equipo local,
        Equipo visitante,
        Estadio estadio,
        FaseTorneo fase,
        int? golesLocal = null,
        int? golesVisitante = null) => new()
    {
        EquipoLocal = local,
        EquipoVisitante = visitante,
        Estadio = estadio,
        Fase = fase,
        Fecha = new DateTime(2026, 6, 1, 14, 0, 0),
        GolesLocal = golesLocal,
        GolesVisitante = golesVisitante
    };

    private static void AssertDatosEquivalentes(DatosDashboard esperados, DatosDashboard obtenidos)
    {
        Assert.AreEqual(esperados.CantidadEquipos, obtenidos.CantidadEquipos);
        Assert.AreEqual(esperados.CantidadEstadios, obtenidos.CantidadEstadios);
        Assert.AreEqual(esperados.FixtureGenerado, obtenidos.FixtureGenerado);
        Assert.AreEqual(esperados.CrucesGenerados, obtenidos.CrucesGenerados);
        Assert.AreEqual(esperados.TotalPartidos, obtenidos.TotalPartidos);
        Assert.AreEqual(esperados.PartidosJugados, obtenidos.PartidosJugados);
    }
}

internal sealed record ContextoDashboardTest(
    RepositorioDashboardFake<Equipo> RepositorioEquipos,
    RepositorioDashboardFake<Estadio> RepositorioEstadios,
    EstadoTorneo EstadoTorneo);

internal sealed class RepositorioDashboardFake<T> : IRepositorio<T>
{
    private readonly List<T> _entidades = new();

    public void Agregar(T entidad) => _entidades.Add(entidad);

    public void Actualizar(T entidad)
    {
    }

    public void Eliminar(T entidad) => _entidades.Remove(entidad);

    public List<T> Buscar(Func<T, bool> predicado) =>
        _entidades.Where(predicado).ToList();
}

