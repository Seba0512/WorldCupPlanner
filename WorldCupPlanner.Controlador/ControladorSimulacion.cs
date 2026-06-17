using WorldCupPlanner.Dominio.DTO;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Dominio.Excepciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.Controladores;

public class ControladorSimulacion
{
    private readonly ServicioSimulacion _servicioSimulacion;
    private readonly ServicioOrquestadorSimulacion _orquestadorSimulacion;
    private readonly ServicioUsuario? _servicioUsuario;
    private readonly ServicioConfiguracionTorneo? _servicioConfiguracionTorneo;
    private readonly ServicioFixture? _servicioFixture;

    public ControladorSimulacion(
        ServicioSimulacion servicioSimulacion,
        ServicioOrquestadorSimulacion orquestadorSimulacion,
        ServicioUsuario? servicioUsuario = null,
        ServicioConfiguracionTorneo? servicioConfiguracionTorneo = null,
        ServicioFixture? servicioFixture = null)
    {
        _servicioSimulacion = servicioSimulacion;
        _orquestadorSimulacion = orquestadorSimulacion;
        _servicioUsuario = servicioUsuario;
        _servicioConfiguracionTorneo = servicioConfiguracionTorneo;
        _servicioFixture = servicioFixture;
    }

    public IReadOnlyList<MotorSimulacionDisponible> ObtenerMotoresDisponibles() =>
        _servicioSimulacion.ObtenerMotoresDisponibles();

    public string ObtenerNombreMotor(MotorSimulacion motor) =>
        _servicioSimulacion.ObtenerNombreMotor(motor);

    public void SimularYPersistirPartidos(ComandoSimularPartidos comando) =>
        _orquestadorSimulacion.SimularYPersistirPartidos(
            comando.Partidos,
            comando.Configuracion,
            _servicioUsuario.ResolverUsuario(comando.Usuario),
            comando.EstadoTorneo);

    public void SimularPartido(
        int partidoId,
        EstadoTorneo estadoTorneo,
        UsuarioDTO? usuarioActual)
    {
        var partido = ResolverPartido(partidoId, estadoTorneo);
        var configuracion = ObtenerConfiguracionUsadaParaFixture();
        SimularYPersistirPartidos(new ComandoSimularPartidos(new List<Partido> { partido }, configuracion, estadoTorneo, usuarioActual));
    }

    public void SimularJornadaOFecha(ComandoSimularJornada comando)
    {
        var partidos = ObtenerPartidosJornadaOFechaActual(comando.EtapaActiva, comando.JornadaActiva, comando.EstadoTorneo);
        var configuracion = ObtenerConfiguracionUsadaParaFixture();
        SimularYPersistirPartidos(new ComandoSimularPartidos(partidos, configuracion, comando.EstadoTorneo, comando.Usuario));
    }

    public bool HayPartidosJornadaOFechaSimulables(ComandoSimularJornada comando) =>
        ObtenerPartidosJornadaOFechaActual(comando.EtapaActiva, comando.JornadaActiva, comando.EstadoTorneo).Any();

    public void SimularFaseActiva(
        FaseTorneo etapaActiva,
        EstadoTorneo estadoTorneo,
        UsuarioDTO? usuarioActual)
    {
        var partidos = ObtenerPartidosPendientesEtapaActiva(etapaActiva, estadoTorneo);
        var configuracion = ObtenerConfiguracionUsadaParaFixture();
        SimularYPersistirPartidos(new ComandoSimularPartidos(partidos, configuracion, estadoTorneo, usuarioActual));
    }

    public bool HayPartidosFaseActivaSimulables(
        FaseTorneo etapaActiva,
        EstadoTorneo estadoTorneo) =>
        ObtenerPartidosPendientesEtapaActiva(etapaActiva, estadoTorneo).Any();

    public bool EsPartidoSimulable(
        int partidoId,
        EstadoTorneo estadoTorneo,
        FaseTorneo etapaActiva)
    {
        var partido = ResolverPartido(partidoId, estadoTorneo);

        if (_servicioFixture == null)
            throw new InvalidOperationException("El servicio de fixture no fue configurado.");

        return _servicioFixture.EsPartidoSimulable(partido, estadoTorneo, etapaActiva);
    }

    private List<Partido> ObtenerPartidosJornadaOFechaActual(
        FaseTorneo etapaActiva,
        int jornadaActiva,
        EstadoTorneo estadoTorneo)
    {
        if (_servicioFixture == null)
            throw new InvalidOperationException("El servicio de fixture no fue configurado.");

        return _servicioFixture.ObtenerPartidosJornadaOFechaActual(estadoTorneo, etapaActiva, jornadaActiva);
    }

    private List<Partido> ObtenerPartidosPendientesEtapaActiva(
        FaseTorneo etapaActiva,
        EstadoTorneo estadoTorneo)
    {
        if (_servicioFixture == null)
            throw new InvalidOperationException("El servicio de fixture no fue configurado.");

        return _servicioFixture.ObtenerPartidosPendientesEtapaActiva(estadoTorneo, etapaActiva);
    }

    private Partido ResolverPartido(int partidoId, EstadoTorneo estadoTorneo)
    {
        var partido = estadoTorneo.Fixture?.Partidos.FirstOrDefault(p => p.Id == partidoId)
                      ?? estadoTorneo.PartidosEliminatorios.FirstOrDefault(p => p.Id == partidoId);

        return partido ?? throw new EntidadInexistenteExcepcion("El partido no existe.");
    }

    private ConfiguracionTorneo ObtenerConfiguracionUsadaParaFixture()
    {
        if (_servicioConfiguracionTorneo == null)
            throw new InvalidOperationException("El servicio de configuracion del torneo no fue configurado.");

        return _servicioConfiguracionTorneo.ObtenerConfiguracionUsadaParaFixture();
    }

}
