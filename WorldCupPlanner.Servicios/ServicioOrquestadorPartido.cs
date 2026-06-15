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

    public void GuardarCambiosPartido(ComandoGuardarPartido comando)
    {
        var estadio = _estadio.ObtenerPorNombre(comando.NombreEstadio);
        _partido.EditarFechaYEstadio(comando.PartidoId, comando.NuevaFecha, estadio);

        if (comando.GolesLocal.HasValue && comando.GolesVisitante.HasValue)
        {
            var config = _configuracion.ObtenerConfiguracionUsadaParaFixture();
            _partido.AplicarResultadoConPenales(comando.PartidoId, new ContextoResultadoPartido
            {
                GolesLocal = comando.GolesLocal.Value,
                GolesVisitante = comando.GolesVisitante.Value,
                SemillaSimulation = config.SemillaSimulation,
                Notificacion = _notificacion,
                Periodistas = _usuario.ObtenerPeriodistas()
            });
            _rankingDinamico.RecalcularRankings(comando.UsuarioActual, comando.PartidoId);
        }

        var actualizado = _partido.BuscarPartidoPorId(comando.PartidoId);
        if (actualizado != null)
            comando.EstadoTorneo.ActualizarPartido(actualizado);
        _rankingDinamico.SincronizarRankingsEnEstado(comando.EstadoTorneo);
    }
}
