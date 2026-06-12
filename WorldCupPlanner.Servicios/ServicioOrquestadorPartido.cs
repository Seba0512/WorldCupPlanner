using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.Servicios;

public class ServicioOrquestadorPartido
{
    private readonly ServicioPartido _partido;
    private readonly ServicioConfiguracionTorneo _configuracion;
    private readonly ServicioNotificacion _notificacion;
    private readonly ServicioUsuario _usuario;
    private readonly ServicioRankingDinamico _rankingDinamico;
    private readonly ServicioEstadio _estadio;

    public ServicioOrquestadorPartido(
        ServicioPartido partido,
        ServicioConfiguracionTorneo configuracion,
        ServicioNotificacion notificacion,
        ServicioUsuario usuario,
        ServicioRankingDinamico rankingDinamico,
        ServicioEstadio estadio)
    {
        _partido = partido;
        _configuracion = configuracion;
        _notificacion = notificacion;
        _usuario = usuario;
        _rankingDinamico = rankingDinamico;
        _estadio = estadio;
    }

    public void GuardarCambiosPartido(
        int partidoId,
        DateTime nuevaFecha,
        string nombreEstadio,
        int? golesLocal,
        int? golesVisitante,
        Usuario? usuarioActual,
        EstadoTorneo estadoTorneo)
    {
        var estadio = _estadio.ObtenerPorNombre(nombreEstadio);
        _partido.EditarFechaYEstadio(partidoId, nuevaFecha, estadio);

        if (golesLocal.HasValue && golesVisitante.HasValue)
        {
            var config = _configuracion.ObtenerConfiguracionUsadaParaFixture();
            _partido.AplicarResultadoConPenales(partidoId, new ContextoResultadoPartido
            {
                GolesLocal = golesLocal.Value,
                GolesVisitante = golesVisitante.Value,
                SemillaSimulation = config.SemillaSimulation,
                Notificacion = _notificacion,
                Periodistas = _usuario.ObtenerPeriodistas()
            });
            _rankingDinamico.RecalcularRankings(usuarioActual);
        }

        var actualizado = _partido.BuscarPartidoPorId(partidoId);
        if (actualizado != null)
            estadoTorneo.ActualizarPartido(actualizado);
        _rankingDinamico.SincronizarRankingsEnEstado(estadoTorneo);
    }
}
