using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.Servicios;

public class ServicioOrquestadorFaseEliminatoria
{
    private readonly ServicioClasificacion _clasificacion;
    private readonly ServicioEstadio _estadio;
    private readonly ServicioConfiguracionTorneo _configuracion;
    private readonly ServicioCruces _cruces;
    private readonly ServicioPersistenciaFixture _persistencia;
    private readonly ServicioConsultasTorneo _consultasTorneo;

    public ServicioOrquestadorFaseEliminatoria(
        ServicioClasificacion clasificacion,
        ServicioEstadio estadio,
        ServicioConfiguracionTorneo configuracion,
        ServicioCruces cruces,
        ServicioPersistenciaFixture persistencia,
        ServicioConsultasTorneo consultasTorneo)
    {
        _clasificacion = clasificacion;
        _estadio = estadio;
        _configuracion = configuracion;
        _cruces = cruces;
        _persistencia = persistencia;
        _consultasTorneo = consultasTorneo;
    }

    public void GenerarFase(FaseTorneo fase, EstadoTorneo estadoTorneo)
    {
        switch (fase)
        {
            case FaseTorneo.PrimeraRondaEliminatoria: GenerarPrimeraRonda(estadoTorneo); break;
            case FaseTorneo.Octavos:                  GenerarOctavos(estadoTorneo);       break;
            case FaseTorneo.Cuartos:                  GenerarCuartos(estadoTorneo);       break;
            case FaseTorneo.Semifinal:                GenerarSemifinales(estadoTorneo);   break;
            case FaseTorneo.Final:                    GenerarFinalYTercerPuesto(estadoTorneo); break;
            default: throw new ArgumentOutOfRangeException(nameof(fase), $"Fase no soportada: {fase}");
        }
    }

    private void GenerarPrimeraRonda(EstadoTorneo estado)
    {
        var config = _configuracion.ObtenerConfiguracionActual();
        var clasificados = _clasificacion.ObtenerClasificadosAFaseEliminatoria(estado.Fixture!, config);
        var estadios = _estadio.ObtenerEstadios();
        var partidos = _cruces.GenerarPrimeraRondaEliminatoria(new ContextoSorteo
        {
            Clasificados = clasificados,
            Estadios = estadios,
            ConfiguracionTorneo = config,
            FechaUltimoPartidoFaseGrupos = estado.Fixture!.Partidos.Max(p => p.Fecha)
        });
        estado.EstablecerPartidosEliminatorios(partidos);
        _persistencia.GuardarPartidos(partidos);
    }

    private void GenerarOctavos(EstadoTorneo estado)
    {
        var config = _configuracion.ObtenerConfiguracionActual();
        var estadios = _estadio.ObtenerEstadios();
        var octavos = _cruces.GenerarOctavos(
            _consultasTorneo.ObtenerPartidosDeFase(estado, FaseTorneo.PrimeraRondaEliminatoria), estadios, config);
        estado.AgregarPartidosEliminatorios(octavos);
        _persistencia.GuardarPartidos(octavos);
    }

    private void GenerarCuartos(EstadoTorneo estado)
    {
        var config = _configuracion.ObtenerConfiguracionActual();
        var estadios = _estadio.ObtenerEstadios();
        var cuartos = _cruces.GenerarCuartos(
            _consultasTorneo.ObtenerPartidosDeFase(estado, FaseTorneo.Octavos), estadios, config);
        estado.AgregarPartidosEliminatorios(cuartos);
        _persistencia.GuardarPartidos(cuartos);
    }

    private void GenerarSemifinales(EstadoTorneo estado)
    {
        var config = _configuracion.ObtenerConfiguracionActual();
        var estadios = _estadio.ObtenerEstadios();
        var semifinales = _cruces.GenerarSemifinales(
            _consultasTorneo.ObtenerPartidosDeFase(estado, FaseTorneo.Cuartos), estadios, config);
        estado.AgregarPartidosEliminatorios(semifinales);
        _persistencia.GuardarPartidos(semifinales);
    }

    private void GenerarFinalYTercerPuesto(EstadoTorneo estado)
    {
        var config = _configuracion.ObtenerConfiguracionActual();
        var estadios = _estadio.ObtenerEstadios();
        var semifinales = _consultasTorneo.ObtenerPartidosDeFase(estado, FaseTorneo.Semifinal);
        var existentes = estado.PartidosEliminatorios;
        var tercerPuesto = _cruces.GenerarTercerPuesto(semifinales, estadios, config, existentes);
        var final = _cruces.GenerarFinal(semifinales, estadios, config, existentes);
        var nuevosPartidos = new Partido[] { tercerPuesto, final };
        estado.AgregarPartidosEliminatorios(nuevosPartidos);
        _persistencia.GuardarPartidos(nuevosPartidos);
    }
}
