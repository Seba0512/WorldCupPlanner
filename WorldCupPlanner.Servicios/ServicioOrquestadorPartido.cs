using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.Servicios;

public class ServicioOrquestadorPartido
{
    private readonly ServicioPartido _partido;
    private readonly ServicioConfiguracionTorneo _configuracion;
    private readonly ServicioNotificacion _notificacion;
    private readonly ServicioUsuario _usuario;
    private readonly ServicioRankingDinamico _rankingDinamico;

    public ServicioOrquestadorPartido(
        ServicioPartido partido,
        ServicioConfiguracionTorneo configuracion,
        ServicioNotificacion notificacion,
        ServicioUsuario usuario,
        ServicioRankingDinamico rankingDinamico)
    {
        _partido = partido;
        _configuracion = configuracion;
        _notificacion = notificacion;
        _usuario = usuario;
        _rankingDinamico = rankingDinamico;
    }

    public void GuardarCambiosPartido(
        int partidoId,
        DateTime nuevaFecha,
        Estadio estadio,
        int? golesLocal,
        int? golesVisitante,
        Usuario? usuarioActual,
        EstadoTorneo estadoTorneo)
    {
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
