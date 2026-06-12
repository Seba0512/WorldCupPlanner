using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.Servicios;

public class ServicioOrquestadorSimulacion
{
    private readonly ServicioSimulacion _simulacion;
    private readonly ServicioPersistenciaFixture _persistencia;
    private readonly ServicioIncidencia _incidencia;
    private readonly ServicioRankingDinamico _rankingDinamico;
    private readonly ServicioUsuario _usuario;
    private readonly ServicioNotificacion _notificacion;

    public ServicioOrquestadorSimulacion(
        ServicioSimulacion simulacion,
        ServicioPersistenciaFixture persistencia,
        ServicioIncidencia incidencia,
        ServicioRankingDinamico rankingDinamico,
        ServicioUsuario usuario,
        ServicioNotificacion notificacion)
    {
        _simulacion = simulacion;
        _persistencia = persistencia;
        _incidencia = incidencia;
        _rankingDinamico = rankingDinamico;
        _usuario = usuario;
        _notificacion = notificacion;
    }

    public void SimularYPersistirPartidos(
        List<Partido> partidos,
        ConfiguracionTorneo config,
        Usuario? usuarioActual,
        EstadoTorneo estadoTorneo)
    {
        var pendientes = partidos
            .Where(p => !p.EstaJugado())
            .OrderBy(p => p.Fecha)
            .ThenBy(p => p.Id)
            .ToList();

        if (!pendientes.Any())
            throw new InvalidOperationException("No hay partidos pendientes para simular en la fase actual.");

        foreach (var partido in pendientes)
        {
            _simulacion.SimularResultados(
                new List<Partido> { partido },
                config,
                usuarioActual,
                estadoTorneo.ObtenerTodosLosPartidos());
            _persistencia.GuardarPartidos(new[] { partido });
            _incidencia.PersistirIncidenciasDePartido(partido);
            _rankingDinamico.RecalcularRankings(usuarioActual);
            _rankingDinamico.SincronizarRankingsEnEstado(estadoTorneo);
        }

        var periodistas = _usuario.ObtenerPeriodistas().ToList();
        _notificacion.GenerarNotificacionesPorResultados(pendientes, periodistas);
        estadoTorneo.ActualizarEstadoFinalizacion();
    }
}
