using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.Servicios;

public class ServicioOrquestadorFixture
{
    private readonly ServicioFixture _fixture;
    private readonly ServicioEquipo _equipo;
    private readonly ServicioEstadio _estadio;
    private readonly ServicioConfiguracionTorneo _configuracion;
    private readonly ServicioPersistenciaFixture _persistencia;
    private readonly ServicioRankingDinamico _rankingDinamico;

    public ServicioOrquestadorFixture(
        ServicioFixture fixture,
        ServicioEquipo equipo,
        ServicioEstadio estadio,
        ServicioConfiguracionTorneo configuracion,
        ServicioPersistenciaFixture persistencia,
        ServicioRankingDinamico rankingDinamico)
    {
        _fixture = fixture;
        _equipo = equipo;
        _estadio = estadio;
        _configuracion = configuracion;
        _persistencia = persistencia;
        _rankingDinamico = rankingDinamico;
    }

    public void GenerarORegenerarFixture(MotorSimulacion motor, Usuario? usuario, EstadoTorneo estadoTorneo)
    {
        _rankingDinamico.FijarRankingBaseDelTorneo(usuario);

        var equipos = _equipo.Buscar(_ => true);
        var estadios = _estadio.ObtenerEstadios();
        var config = _configuracion.ObtenerConfiguracionActual();
        config.MotorSimulacion = motor;

        var fixtureGenerado = estadoTorneo.Fixture == null
            ? _fixture.GenerarFixture(equipos, estadios, config)
            : _fixture.RegenerarFixture(
                equipos,
                estadoTorneo.Fixture,
                estadios,
                config,
                estadoTorneo.PartidosEliminatorios);

        estadoTorneo.EstablecerFixture(fixtureGenerado);
        _equipo.RegistrarFixtureGenerado(fixtureGenerado);
        _estadio.RegistrarFixtureGenerado(fixtureGenerado);
        _configuracion.RegistrarFixtureGenerado(config);
        _persistencia.GuardarFixture(fixtureGenerado);
    }
}
