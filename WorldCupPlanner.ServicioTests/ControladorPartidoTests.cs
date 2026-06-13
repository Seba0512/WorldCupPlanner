using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Controladores;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Dominio.Excepciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.ServicioTests;

[TestClass]
public class ControladorPartidoTests
{
    [TestMethod]
    public void BuscarPartidoPorId_DevuelvePartidoDelServicio()
    {
        var repositorio = new RepositorioPartidoControladorFake();
        var partido = CrearPartido(1);
        repositorio.Agregar(partido);
        var controlador = CrearControlador(repositorio);

        var encontrado = controlador.BuscarPartidoPorId(1);

        Assert.AreSame(partido, encontrado);
    }

    [TestMethod]
    public void ObtenerPartidos_DevuelveTodosLosPartidos()
    {
        var repositorio = new RepositorioPartidoControladorFake();
        repositorio.Agregar(CrearPartido(1));
        repositorio.Agregar(CrearPartido(2));
        var controlador = CrearControlador(repositorio);

        var partidos = controlador.ObtenerPartidos();

        Assert.AreEqual(2, partidos.Count);
    }

    [TestMethod]
    public void EditarFechaYEstadio_DelegaEdicionEnServicio()
    {
        var repositorio = new RepositorioPartidoControladorFake();
        var partido = CrearPartido(1);
        repositorio.Agregar(partido);
        var controlador = CrearControlador(repositorio);
        var nuevaFecha = new DateTime(2026, 6, 10, 18, 0, 0);
        var nuevoEstadio = CrearEstadio("Estadio Nuevo");

        controlador.EditarFechaYEstadio(partido.Id, nuevaFecha, nuevoEstadio);

        Assert.AreEqual(nuevaFecha, partido.Fecha);
        Assert.AreSame(nuevoEstadio, partido.Estadio);
    }

    [TestMethod]
    public void CargarResultado_ConGolesNegativos_MantieneValidacionDelServicio()
    {
        var repositorio = new RepositorioPartidoControladorFake();
        var partido = CrearPartido(1);
        repositorio.Agregar(partido);
        var controlador = CrearControlador(repositorio);

        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            controlador.CargarResultado(partido.Id, -1, 0));
    }

    [TestMethod]
    public void EsFaseBloqueada_CuandoHayFasePosterior_DelegadaAlServicio()
    {
        var repositorio = new RepositorioPartidoControladorFake();
        repositorio.Agregar(CrearPartido(1, FaseTorneo.FaseDeGrupos));
        repositorio.Agregar(CrearPartido(2, FaseTorneo.PrimeraRondaEliminatoria));
        var controlador = CrearControlador(repositorio);

        Assert.IsTrue(controlador.EsFaseBloqueada(FaseTorneo.FaseDeGrupos));
    }

    [TestMethod]
    public void FiltrarPartidos_DevuelveCoincidenciasDelFiltro()
    {
        var controlador = CrearControlador(new RepositorioPartidoControladorFake());
        var partidos = new List<Partido>
        {
            CrearPartido(1, estadio: "Estadio A", grupo: "A"),
            CrearPartido(2, estadio: "Estadio B", grupo: "B")
        };

        var filtrados = controlador.FiltrarPartidos(partidos, new FiltroPartidos { Grupo = "A" }).ToList();

        Assert.AreEqual(1, filtrados.Count);
        Assert.AreEqual("A", filtrados[0].Grupo!.Etiqueta);
    }

    [TestMethod]
    public void HelpersDisponibles_DeleganListadosDelServicio()
    {
        var controlador = CrearControlador(new RepositorioPartidoControladorFake());
        var partidos = new List<Partido>
        {
            CrearPartido(1, estadio: "Estadio B", grupo: "B"),
            CrearPartido(2, estadio: "Estadio A", grupo: "A", fase: FaseTorneo.Octavos)
        };

        CollectionAssert.AreEqual(
            new[] { "Estadio A", "Estadio B" },
            controlador.ObtenerEstadiosDisponibles(partidos).ToArray());
        CollectionAssert.AreEqual(
            new[] { "A", "B" },
            controlador.ObtenerGruposDisponibles(partidos).ToArray());
        CollectionAssert.AreEqual(
            new[] { FaseTorneo.FaseDeGrupos, FaseTorneo.Octavos },
            controlador.ObtenerFasesDisponibles(partidos).ToArray());
    }

    private static ControladorPartido CrearControlador(IRepositorio<Partido> repositorio)
    {
        var servicio = new ServicioPartido(new ProveedorPartidosRepositorio(repositorio));
        return new ControladorPartido(servicio);
    }

    private static Partido CrearPartido(
        int id,
        FaseTorneo fase = FaseTorneo.FaseDeGrupos,
        string estadio = "Estadio Inicial",
        string? grupo = null)
    {
        return new Partido
        {
            Id = id,
            Fecha = new DateTime(2026, 6, id, 14, 0, 0),
            Estadio = CrearEstadio(estadio),
            EquipoLocal = CrearEquipo($"Local {id}"),
            EquipoVisitante = CrearEquipo($"Visitante {id}"),
            Grupo = grupo == null ? null : new Grupo(grupo),
            Fase = fase
        };
    }

    private static Equipo CrearEquipo(string nombre) => new()
    {
        Nombre = nombre,
        Confederacion = Confederacion.UEFA,
        RankingFIFA = 1200
    };

    private static Estadio CrearEstadio(string nombre) => new()
    {
        Nombre = nombre,
        Ciudad = "Ciudad",
        Capacidad = 30000
    };
}

file sealed class RepositorioPartidoControladorFake : IRepositorio<Partido>
{
    private readonly List<Partido> _partidos = new();

    public void Agregar(Partido partido) => _partidos.Add(partido);

    public void Actualizar(Partido partido)
    {
        var indice = _partidos.FindIndex(p => p.Id == partido.Id);
        if (indice == -1)
            throw new EntidadInexistenteExcepcion("El partido no existe.");
        _partidos[indice] = partido;
    }

    public void Eliminar(Partido partido) => _partidos.Remove(partido);

    public List<Partido> Buscar(Func<Partido, bool> predicado) =>
        _partidos.Where(predicado).ToList();
}
