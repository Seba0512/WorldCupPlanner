using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.Controladores;

public class ControladorPartido
{
    private readonly ServicioPartido _servicioPartido;

    public ControladorPartido(ServicioPartido servicioPartido)
    {
        _servicioPartido = servicioPartido;
    }

    public Partido? BuscarPartidoPorId(int idPartido) =>
        _servicioPartido.BuscarPartidoPorId(idPartido);

    public List<Partido> ObtenerPartidos() =>
        _servicioPartido.ObtenerPartidos();

    public bool EsFaseBloqueada(FaseTorneo fase) =>
        _servicioPartido.EsFaseBloqueada(fase);

    public void EditarFechaYEstadio(int idPartido, DateTime fecha, Estadio estadio) =>
        _servicioPartido.EditarFechaYEstadio(idPartido, fecha, estadio);

    public void CargarResultado(int idPartido, int golesLocal, int golesVisitante) =>
        _servicioPartido.CargarResultado(idPartido, golesLocal, golesVisitante);

    public IEnumerable<Partido> FiltrarPartidos(IEnumerable<Partido> partidos, FiltroPartidos filtro) =>
        _servicioPartido.FiltrarPartidos(partidos, filtro);

    public IEnumerable<string> ObtenerEstadiosDisponibles(IEnumerable<Partido> partidos) =>
        _servicioPartido.ObtenerEstadiosDisponibles(partidos);

    public IEnumerable<string> ObtenerGruposDisponibles(IEnumerable<Partido> partidos) =>
        _servicioPartido.ObtenerGruposDisponibles(partidos);

    public IEnumerable<FaseTorneo> ObtenerFasesDisponibles(IEnumerable<Partido> partidos) =>
        _servicioPartido.ObtenerFasesDisponibles(partidos);
}
