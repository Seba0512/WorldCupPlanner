namespace WorldCupPlanner.Servicios;

public class ServicioDashboard
{
    private readonly ServicioEquipo _equipo;
    private readonly ServicioEstadio _estadio;
    private readonly EstadoTorneo _estadoTorneo;
    private readonly ServicioConsultasTorneo _consultasTorneo;

    public ServicioDashboard(
        ServicioEquipo equipo,
        ServicioEstadio estadio,
        EstadoTorneo estadoTorneo,
        ServicioConsultasTorneo consultasTorneo)
    {
        _equipo = equipo;
        _estadio = estadio;
        _estadoTorneo = estadoTorneo;
        _consultasTorneo = consultasTorneo;
    }

    public DatosDashboard ObtenerDatos() => new()
    {
        CantidadEquipos  = _equipo.Buscar(_ => true).Count,
        CantidadEstadios = _estadio.ObtenerEstadios().Count,
        FixtureGenerado  = _estadoTorneo.Fixture != null,
        CrucesGenerados  = _estadoTorneo.PartidosEliminatorios.Any(),
        TotalPartidos    = _consultasTorneo.ContarTotalPartidos(_estadoTorneo),
        PartidosJugados  = _consultasTorneo.ContarPartidosJugados(_estadoTorneo)
    };
}
