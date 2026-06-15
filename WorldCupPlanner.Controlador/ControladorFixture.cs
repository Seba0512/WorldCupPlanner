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

    public Fixture RegenerarFixture(
        List<Equipo> equipos,
        Fixture fixtureActual,
        List<Estadio> estadios,
        ConfiguracionTorneo configuracionTorneo,
        IReadOnlyList<Partido>? partidosEliminatorios = null) =>
        _servicioFixture.RegenerarFixture(
            equipos,
            fixtureActual,
            estadios,
            configuracionTorneo,
            partidosEliminatorios);

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

    public string ObtenerMensajeExitoSimulacion(FaseTorneo fase) =>
        _servicioFixture.ObtenerMensajeExitoSimulacion(fase);

    public string ObtenerMensajeExitoGeneracionFase(FaseTorneo fase) =>
        _servicioFixture.ObtenerMensajeExitoGeneracionFase(fase);

    public void GenerarORegenerarFixture(MotorSimulacion motor, UsuarioDTO? usuario, EstadoTorneo estadoTorneo)
    {
        if (_orquestadorFixture == null)
            throw new InvalidOperationException("El orquestador de fixture no fue configurado.");

        _orquestadorFixture.GenerarORegenerarFixture(motor, ResolverUsuario(usuario), estadoTorneo);
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
