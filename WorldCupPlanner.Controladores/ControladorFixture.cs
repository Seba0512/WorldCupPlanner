using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.Controladores;

public class ControladorFixture
{
    private readonly ServicioFixture _servicioFixture;

    public ControladorFixture(ServicioFixture servicioFixture)
    {
        _servicioFixture = servicioFixture;
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
}
