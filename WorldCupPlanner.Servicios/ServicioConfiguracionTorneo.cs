using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Dominio.Excepciones;

namespace WorldCupPlanner.Servicios;

public class ServicioConfiguracionTorneo
{
    private readonly ServicioAuditoria? _auditoria;
    private ConfiguracionTorneo _configuracionActual = new();
    private ConfiguracionTorneo? _configuracionUsadaParaFixture;

    public ServicioConfiguracionTorneo(ServicioAuditoria? auditoria = null)
    {
        _auditoria = auditoria;
    }

    public bool EsConfiguracionModificable => _configuracionUsadaParaFixture == null;

    public void ModificarConfiguracion(Usuario usuario, ConfiguracionTorneo nuevaConfiguracion)
    {
        ValidarUsuarioAdministrador(usuario);
        ValidarConfiguracionNoBloqueada();
        _configuracionActual = CopiarConfiguracion(nuevaConfiguracion);
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
        if (_configuracionUsadaParaFixture != null)
        {
            throw new EntidadInvalidaExcepcion(
                "No se puede modificar la configuración después de generado el fixture.");
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
