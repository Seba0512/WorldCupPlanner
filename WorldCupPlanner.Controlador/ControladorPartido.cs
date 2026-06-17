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

    public PartidoDTO? BuscarPartidoPorIdDTO(int idPartido)
    {
        var partido = BuscarPartidoPorId(idPartido);
        return partido == null ? null : PartidoDTO.FromEntity(partido);
    }

    public List<Partido> ObtenerPartidos() =>
        _servicioPartido.ObtenerPartidos();

    public List<PartidoDTO> ObtenerPartidosDTO() =>
        ObtenerPartidos().Select(PartidoDTO.FromEntity).ToList();

    public List<PartidoDTO> ConvertirADTOs(IEnumerable<Partido> partidos) =>
        partidos.Select(PartidoDTO.FromEntity).ToList();

    public bool EsFaseBloqueada(FaseTorneo fase) =>
        _servicioPartido.EsFaseBloqueada(fase);

    public void EditarFechaYEstadio(int idPartido, DateTime fecha, Estadio estadio) =>
        _servicioPartido.EditarFechaYEstadio(idPartido, fecha, estadio);

    public void CargarResultado(int idPartido, int golesLocal, int golesVisitante) =>
        _servicioPartido.CargarResultado(idPartido, golesLocal, golesVisitante);

    public IEnumerable<Partido> FiltrarPartidos(IEnumerable<Partido> partidos, FiltroPartidos filtro) =>
        _servicioPartido.FiltrarPartidos(partidos, filtro);

    public IEnumerable<PartidoDTO> FiltrarPartidosDTO(IEnumerable<PartidoDTO> partidos, FiltroPartidos filtro)
    {
        return partidos
            .Where(p => filtro.Fecha == null || p.Fecha.Date == filtro.Fecha.Value.Date)
            .Where(p => string.IsNullOrEmpty(filtro.Estadio) || p.Estadio?.Nombre == filtro.Estadio)
            .Where(p => string.IsNullOrEmpty(filtro.Grupo) || p.GrupoEtiqueta == filtro.Grupo)
            .Where(p => string.IsNullOrEmpty(filtro.Fase) || p.Fase.ToString() == filtro.Fase);
    }

    public IEnumerable<string> ObtenerEstadiosDisponibles(IEnumerable<Partido> partidos) =>
        _servicioPartido.ObtenerEstadiosDisponibles(partidos);

    public IEnumerable<string> ObtenerEstadiosDisponiblesDTO(IEnumerable<PartidoDTO> partidos) =>
        partidos.Select(p => p.Estadio?.Nombre).Where(n => n != null).Distinct().OrderBy(n => n)!;

    public IEnumerable<string> ObtenerGruposDisponibles(IEnumerable<Partido> partidos) =>
        _servicioPartido.ObtenerGruposDisponibles(partidos);

    public IEnumerable<string> ObtenerGruposDisponiblesDTO(IEnumerable<PartidoDTO> partidos) =>
        partidos.Select(p => p.GrupoEtiqueta).Where(e => e != null).Distinct().OrderBy(e => e)!;

    public IEnumerable<FaseTorneo> ObtenerFasesDisponibles(IEnumerable<Partido> partidos) =>
        _servicioPartido.ObtenerFasesDisponibles(partidos);

    public IEnumerable<FaseTorneo> ObtenerFasesDisponiblesDTO(IEnumerable<PartidoDTO> partidos) =>
        partidos.Select(p => p.Fase).Distinct().OrderBy(f => (int)f);

    public bool EstaJugado(PartidoDTO partido) =>
        partido.GolesLocal.HasValue && partido.GolesVisitante.HasValue;

    public bool FueDefinidoPorPenales(PartidoDTO partido) =>
        EstaJugado(partido) &&
        partido.GolesLocal == partido.GolesVisitante &&
        partido.GanadorPorPenales != null;

    public bool EsGanadorPorPenales(PartidoDTO partido, EquipoDTO? equipo) =>
        FueDefinidoPorPenales(partido) &&
        equipo != null &&
        partido.GanadorPorPenales?.Nombre == equipo.Nombre;

    public void GuardarCambiosPartido(
        PartidoDTO partido,
        UsuarioDTO? usuarioActual,
        EstadoTorneo estadoTorneo) =>
        GuardarCambiosPartido(new ComandoGuardarPartido(
            partido.Id,
            partido.Fecha,
            partido.Estadio?.Nombre ?? string.Empty,
            partido.GolesLocal,
            partido.GolesVisitante,
            estadoTorneo,
            usuarioActual));

    public void GuardarCambiosPartido(ComandoGuardarPartido comando)
    {
        if (_orquestadorPartido == null)
            throw new InvalidOperationException("El orquestador de partido no fue configurado.");

        _orquestadorPartido.GuardarCambiosPartido(
            comando.PartidoId,
            comando.NuevaFecha,
            comando.NombreEstadio,
            comando.GolesLocal,
            comando.GolesVisitante,
            _servicioUsuario.ResolverUsuario(comando.Usuario),
            comando.EstadoTorneo);
    }

}
