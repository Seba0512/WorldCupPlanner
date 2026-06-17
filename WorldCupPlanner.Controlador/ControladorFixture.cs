using WorldCupPlanner.Dominio.DTO;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Dominio.Excepciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.Controladores;

public class ControladorFixture
{
    private readonly ServicioFixture _servicioFixture;
    private readonly ServicioOrquestadorFixture? _orquestadorFixture;
    private readonly ServicioPersistenciaFixture? _persistenciaFixture;
    private readonly ServicioEquipo? _servicioEquipo;
    private readonly ServicioEstadio? _servicioEstadio;
    private readonly ServicioUsuario? _servicioUsuario;

    public ControladorFixture(ServicioFixture servicioFixture, ServicioUsuario? servicioUsuario = null)
    {
        _servicioFixture = servicioFixture;
        _servicioUsuario = servicioUsuario;
    }

    public ControladorFixture(
        ServicioFixture servicioFixture,
        ServicioOrquestadorFixture orquestadorFixture,
        ServicioUsuario? servicioUsuario = null)
        : this(servicioFixture, servicioUsuario)
    {
        _orquestadorFixture = orquestadorFixture;
    }

    public ControladorFixture(
        ServicioFixture servicioFixture,
        ServicioPersistenciaFixture persistenciaFixture,
        ServicioEquipo servicioEquipo,
        ServicioEstadio servicioEstadio,
        ServicioUsuario? servicioUsuario = null)
        : this(servicioFixture, servicioUsuario)
    {
        _persistenciaFixture = persistenciaFixture;
        _servicioEquipo = servicioEquipo;
        _servicioEstadio = servicioEstadio;
    }

    public ControladorFixture(
        ServicioFixture servicioFixture,
        ServicioOrquestadorFixture orquestadorFixture,
        ServicioPersistenciaFixture persistenciaFixture,
        ServicioEquipo servicioEquipo,
        ServicioEstadio servicioEstadio,
        ServicioUsuario? servicioUsuario = null)
        : this(servicioFixture, orquestadorFixture, servicioUsuario)
    {
        _persistenciaFixture = persistenciaFixture;
        _servicioEquipo = servicioEquipo;
        _servicioEstadio = servicioEstadio;
    }

    public Fixture GenerarFixture(
        List<Equipo> equipos,
        List<Estadio> estadios,
        ConfiguracionTorneo configuracionTorneo) =>
        _servicioFixture.GenerarFixture(equipos, estadios, configuracionTorneo);

    public Fixture RegenerarFixture(ComandoRegenerarFixture comando) =>
        _servicioFixture.RegenerarFixture(
            comando.Equipos,
            comando.FixtureActual,
            comando.Estadios,
            comando.Configuracion,
            comando.PartidosEliminatorios);

    public bool FaseYaGenerada(FaseTorneo fase, EstadoTorneo estado) =>
        _servicioFixture.FaseYaGenerada(fase, estado);

    public bool PuedeGenerarFase(FaseTorneo fase, EstadoTorneo estado) =>
        _servicioFixture.PuedeGenerarFase(fase, estado);

    public string ObtenerMensajeCondicionFase(FaseTorneo fase, EstadoTorneo estado) =>
        _servicioFixture.ObtenerMensajeCondicionFase(fase, estado);

    public FaseTorneo? ObtenerFaseActualSimulable(EstadoTorneo estado) =>
        _servicioFixture.ObtenerFaseActualSimulable(estado);

    public int? ObtenerJornadaActivaParaFase(FaseTorneo fase, Fixture? fixture) =>
        _servicioFixture.ObtenerJornadaActivaParaFase(fase, fixture);

    public bool EsJornadaSeleccionable(int jornada, Fixture? fixture) =>
        _servicioFixture.EsJornadaSeleccionable(jornada, fixture);

    public Partido? ObtenerProximoPartidoSimulable(EstadoTorneo estado, FaseTorneo etapaActiva) =>
        _servicioFixture.ObtenerProximoPartidoSimulable(estado, etapaActiva);

    public bool EsPartidoSimulable(Partido? partido, EstadoTorneo estado, FaseTorneo etapaActiva) =>
        _servicioFixture.EsPartidoSimulable(partido, estado, etapaActiva);

    public List<Partido> ObtenerPartidosPendientesEtapaActiva(EstadoTorneo estado, FaseTorneo etapaActiva) =>
        _servicioFixture.ObtenerPartidosPendientesEtapaActiva(estado, etapaActiva);

    public List<Partido> ObtenerPartidosJornadaOFechaActual(
        EstadoTorneo estado,
        FaseTorneo etapaActiva,
        int jornadaActiva) =>
        _servicioFixture.ObtenerPartidosJornadaOFechaActual(estado, etapaActiva, jornadaActiva);

    public FixtureDTO? ObtenerFixtureDTO(EstadoTorneo estado) =>
        estado.Fixture == null ? null : FixtureDTO.FromEntity(estado.Fixture);

    public FixtureDTO? ObtenerFixtureAnteriorDTO(EstadoTorneo estado) =>
        estado.FixtureAnterior == null ? null : FixtureDTO.FromEntity(estado.FixtureAnterior);

    public List<GrupoDTO> ObtenerGruposDTO(EstadoTorneo estado) =>
        ObtenerFixtureDTO(estado)?.Grupos ?? [];

    public List<GrupoDTO> ObtenerGruposAnterioresDTO(EstadoTorneo estado) =>
        ObtenerFixtureAnteriorDTO(estado)?.Grupos ?? [];

    public GrupoDTO? ObtenerGrupoDTO(EstadoTorneo estado, string etiqueta) =>
        ObtenerGruposDTO(estado).FirstOrDefault(grupo => grupo.Etiqueta == etiqueta);

    public GrupoDTO? ObtenerGrupoAnteriorDTO(EstadoTorneo estado, string etiqueta) =>
        ObtenerGruposAnterioresDTO(estado).FirstOrDefault(grupo => grupo.Etiqueta == etiqueta);

    public List<PartidoDTO> ObtenerPartidosEliminatoriosDTO(EstadoTorneo estado) =>
        estado.PartidosEliminatorios.Select(PartidoDTO.FromEntity).ToList();

    public List<PartidoDTO> ObtenerPartidosEliminatoriosAnterioresDTO(EstadoTorneo estado) =>
        estado.PartidosEliminatoriosAnteriores.Select(PartidoDTO.FromEntity).ToList();

    public List<PartidoDTO> ObtenerPartidosEliminatoriosPorFaseDTO(EstadoTorneo estado, FaseTorneo fase) =>
        ObtenerPartidosEliminatoriosDTO(estado).Where(partido => partido.Fase == fase).ToList();

    public List<PartidoDTO> ObtenerPartidosEliminatoriosAnterioresPorFaseDTO(EstadoTorneo estado, FaseTorneo fase) =>
        ObtenerPartidosEliminatoriosAnterioresDTO(estado).Where(partido => partido.Fase == fase).ToList();

    public PartidoDTO? ObtenerFinalDTO(EstadoTorneo estado) =>
        ObtenerPartidosEliminatoriosDTO(estado).FirstOrDefault(partido => partido.Fase == FaseTorneo.Final);

    public PartidoDTO? ObtenerTercerPuestoDTO(EstadoTorneo estado) =>
        ObtenerPartidosEliminatoriosDTO(estado).FirstOrDefault(partido => partido.Fase == FaseTorneo.TercerPuesto);

    public PartidoDTO? ObtenerFinalAnteriorDTO(EstadoTorneo estado) =>
        ObtenerPartidosEliminatoriosAnterioresDTO(estado).FirstOrDefault(partido => partido.Fase == FaseTorneo.Final);

    public PartidoDTO? ObtenerTercerPuestoAnteriorDTO(EstadoTorneo estado) =>
        ObtenerPartidosEliminatoriosAnterioresDTO(estado).FirstOrDefault(partido => partido.Fase == FaseTorneo.TercerPuesto);

    public bool EstaJugado(PartidoDTO partido) =>
        partido.GolesLocal.HasValue && partido.GolesVisitante.HasValue;

    public bool EsEmpate(PartidoDTO partido) =>
        EstaJugado(partido) && partido.GolesLocal == partido.GolesVisitante;

    public bool FueDefinidoPorPenales(PartidoDTO partido) =>
        EsEmpate(partido) && partido.GanadorPorPenales != null;

    public bool EsGanador(PartidoDTO partido, EquipoDTO? equipo)
    {
        if (!EstaJugado(partido) || equipo == null)
            return false;

        if (FueDefinidoPorPenales(partido))
            return partido.GanadorPorPenales?.Nombre == equipo.Nombre;

        return (partido.EquipoLocal?.Nombre == equipo.Nombre && partido.GolesLocal > partido.GolesVisitante)
               || (partido.EquipoVisitante?.Nombre == equipo.Nombre && partido.GolesVisitante > partido.GolesLocal);
    }

    public string ObtenerMensajeExitoSimulacion(FaseTorneo fase) =>
        _servicioFixture.ObtenerMensajeExitoSimulacion(fase);

    public string ObtenerMensajeExitoGeneracionFase(FaseTorneo fase) =>
        _servicioFixture.ObtenerMensajeExitoGeneracionFase(fase);

    public void GenerarORegenerarFixture(MotorSimulacion motor, UsuarioDTO? usuario, EstadoTorneo estadoTorneo)
    {
        if (_orquestadorFixture == null)
            throw new InvalidOperationException("El orquestador de fixture no fue configurado.");

        _orquestadorFixture.GenerarORegenerarFixture(motor, _servicioUsuario.ResolverUsuario(usuario), estadoTorneo);
    }

    public void CrearNuevoTorneo(EstadoTorneo estadoTorneo)
    {
        if (_persistenciaFixture == null || _servicioEquipo == null || _servicioEstadio == null)
            throw new InvalidOperationException("Los servicios de cierre de torneo no fueron configurados.");

        estadoTorneo.ArchivarTorneoActual();
        _persistenciaFixture.ArchivarTorneoActual();
        _servicioEquipo.LimpiarFixtureGenerado();
        _servicioEstadio.LimpiarFixtureGenerado();
    }

}
