using WorldCupPlanner.Dominio.DTO;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Dominio.Excepciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.Controladores;

public class ControladorPartido
{
    private readonly ServicioPartido _servicioPartido;
    private readonly ServicioOrquestadorPartido? _orquestadorPartido;
    private readonly ServicioUsuario? _servicioUsuario;

    public ControladorPartido(ServicioPartido servicioPartido, ServicioUsuario? servicioUsuario = null)
    {
        _servicioPartido = servicioPartido;
        _servicioUsuario = servicioUsuario;
    }

    public ControladorPartido(
        ServicioPartido servicioPartido,
        ServicioOrquestadorPartido orquestadorPartido,
        ServicioUsuario? servicioUsuario = null)
        : this(servicioPartido, servicioUsuario)
    {
        _orquestadorPartido = orquestadorPartido;
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

    public void GuardarCambiosPartido(
        int partidoId,
        DateTime nuevaFecha,
        string nombreEstadio,
        int? golesLocal,
        int? golesVisitante,
        UsuarioDTO? usuarioActual,
        EstadoTorneo estadoTorneo)
    {
        if (_orquestadorPartido == null)
            throw new InvalidOperationException("El orquestador de partido no fue configurado.");

        _orquestadorPartido.GuardarCambiosPartido(
            partidoId,
            nuevaFecha,
            nombreEstadio,
            golesLocal,
            golesVisitante,
            ResolverUsuario(usuarioActual),
            estadoTorneo);
    }

    private Usuario? ResolverUsuario(UsuarioDTO? usuarioActual)
    {
        if (usuarioActual == null)
            return null;

        if (_servicioUsuario == null)
            throw new InvalidOperationException("El servicio de usuario no fue configurado.");

        return _servicioUsuario.ObtenerPorEmail(usuarioActual.Email)
               ?? throw new EntidadInexistenteExcepcion($"No existe un usuario con el email '{usuarioActual.Email}'.");
    }
}
