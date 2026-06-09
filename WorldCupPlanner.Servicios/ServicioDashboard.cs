namespace WorldCupPlanner.Servicios;

public class ServicioDashboard
{
    private readonly ServicioEquipo _equipo;
    private readonly ServicioEstadio _estadio;
    private readonly EstadoTorneo _estadoTorneo;

    public ServicioDashboard(ServicioEquipo equipo, ServicioEstadio estadio, EstadoTorneo estadoTorneo)
    {
        _equipo = equipo;
        _estadio = estadio;
        _estadoTorneo = estadoTorneo;
    }

    public DatosDashboard ObtenerDatos() => new()
    {
        CantidadEquipos  = _equipo.Buscar(_ => true).Count,
        CantidadEstadios = _estadio.ObtenerEstadios().Count,
        FixtureGenerado  = _estadoTorneo.Fixture != null,
        CrucesGenerados  = _estadoTorneo.PartidosEliminatorios.Any(),
        TotalPartidos    = _estadoTorneo.ContarTotalPartidos(),
        PartidosJugados  = _estadoTorneo.ContarPartidosJugados()
    };
}
