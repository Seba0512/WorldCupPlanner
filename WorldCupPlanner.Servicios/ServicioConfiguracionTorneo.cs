using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Dominio.Excepciones;

namespace WorldCupPlanner.Servicios;

public class ServicioConfiguracionTorneo
{
    private readonly ServicioAuditoria? _auditoria;
    private readonly IRepositorioConfiguracionTorneo? _repositorio;
    private readonly EstadoTorneo? _estadoTorneo;
    private ConfiguracionTorneo _configuracionActual = new();
    private ConfiguracionTorneo? _configuracionUsadaParaFixture;

    public ServicioConfiguracionTorneo(
        ServicioAuditoria? auditoria = null,
        IRepositorioConfiguracionTorneo? repositorio = null,
        EstadoTorneo? estadoTorneo = null)
    {
        _auditoria = auditoria;
        _repositorio = repositorio;
        _estadoTorneo = estadoTorneo;

        if (_repositorio != null)
        {
            _configuracionActual = _repositorio.ObtenerConfiguracionActual();
            _configuracionUsadaParaFixture = _repositorio.ObtenerConfiguracionUsadaParaFixture();
        }
    }

    // Desbloqueada cuando: nunca se generó fixture, el torneo ya finalizó,
    // o el fixture fue archivado (Fixture==null con EstadoTorneo presente).
    public bool EsConfiguracionModificable =>
        _configuracionUsadaParaFixture == null
        || _estadoTorneo?.TorneoFinalizado == true
        || (_estadoTorneo != null && _estadoTorneo.Fixture == null);

    private const int MinMaxPartidosPorDia = 2;

    public void ModificarConfiguracion(Usuario usuario, ConfiguracionTorneo nuevaConfiguracion)
    {
        ValidarUsuarioAdministrador(usuario);
        ValidarConfiguracionNoBloqueada();
        ValidarSemillasNoNegativas(nuevaConfiguracion);
        ValidarParametrosNumericos(nuevaConfiguracion);
        _configuracionActual = CopiarConfiguracion(nuevaConfiguracion);
        _repositorio?.GuardarConfiguracionActual(_configuracionActual);
        _auditoria?.Registrar(
            $"{usuario.Nombre} {usuario.Apellido}",
            "Modificación de configuración del torneo",
            $"SemillaFixture={nuevaConfiguracion.SemillaFixture}, SemillaCompletar={nuevaConfiguracion.SemillaCompletar}, " +
            $"SemillaCrucesFase={nuevaConfiguracion.SemillaCrucesFase}, SemillaSimulation={nuevaConfiguracion.SemillaSimulation}, " +
            $"FechaInicio={nuevaConfiguracion.FechaInicioTorneo:yyyy-MM-dd}, MaxPartidosPorDia={nuevaConfiguracion.MaxPartidosPorDia}");
    }

    public ConfiguracionTorneo ObtenerConfiguracionActual()
    {
        return CopiarConfiguracion(_configuracionActual);
    }

    public void RegistrarFixtureGenerado(ConfiguracionTorneo configuracionUsada)
    {
        _configuracionUsadaParaFixture = CopiarConfiguracion(configuracionUsada);
        _repositorio?.GuardarConfiguracionUsadaParaFixture(_configuracionUsadaParaFixture);
    }

    public ConfiguracionTorneo ObtenerConfiguracionUsadaParaFixture()
    {
        if (_configuracionUsadaParaFixture == null)
        {
            throw new EntidadInvalidaExcepcion("Todavía no hay configuración usada para generar fixture.");
        }

        return CopiarConfiguracion(_configuracionUsadaParaFixture);
    }

    private void ValidarUsuarioAdministrador(Usuario usuario)
    {
        if (!usuario.TieneRol(RolUsuario.AdministradorDelSistema))
        {
            throw new EntidadInvalidaExcepcion(
                "Solo el Administrador del Sistema puede modificar la configuración del torneo.");
        }
    }

    private void ValidarConfiguracionNoBloqueada()
    {
        if (!EsConfiguracionModificable)
        {
            throw new EntidadInvalidaExcepcion(
                "No se puede modificar la configuración después de generado el fixture.");
        }
    }

    private void ValidarParametrosNumericos(ConfiguracionTorneo configuracion)
    {
        if (configuracion.MaxPartidosPorDia < MinMaxPartidosPorDia)
            throw new EntidadInvalidaExcepcion(
                $"MaxPartidosPorDia debe ser al menos {MinMaxPartidosPorDia}.");
    }

    private void ValidarSemillasNoNegativas(ConfiguracionTorneo configuracion)
    {
        if (configuracion.SemillaFixture < 0 ||
            configuracion.SemillaCompletar < 0 ||
            configuracion.SemillaCrucesFase < 0 ||
            configuracion.SemillaSimulation < 0)
        {
            throw new EntidadInvalidaExcepcion("Las semillas deben ser enteros mayores o iguales a cero.");
        }
    }

    private ConfiguracionTorneo CopiarConfiguracion(ConfiguracionTorneo configuracion)
    {
        return new ConfiguracionTorneo
        {
            SemillaFixture = configuracion.SemillaFixture,
            SemillaCompletar = configuracion.SemillaCompletar,
            SemillaCrucesFase = configuracion.SemillaCrucesFase,
            SemillaSimulation = configuracion.SemillaSimulation,
            FechaInicioTorneo = configuracion.FechaInicioTorneo,
            MaxPartidosPorDia = configuracion.MaxPartidosPorDia,
            SeparacionEntreFechas = configuracion.SeparacionEntreFechas,
            HoraInicioPartidos = configuracion.HoraInicioPartidos,
            SeparacionHorasEntrePartidos = configuracion.SeparacionHorasEntrePartidos
        };
    }
}
