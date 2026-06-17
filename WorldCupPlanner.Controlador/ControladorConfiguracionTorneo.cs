using WorldCupPlanner.Dominio.DTO;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Excepciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.Controladores;

public class ControladorConfiguracionTorneo
{
    private readonly ServicioConfiguracionTorneo _servicioConfiguracionTorneo;
    private readonly ServicioUsuario? _servicioUsuario;

    public ControladorConfiguracionTorneo(
        ServicioConfiguracionTorneo servicioConfiguracionTorneo,
        ServicioUsuario? servicioUsuario = null)
    {
        _servicioConfiguracionTorneo = servicioConfiguracionTorneo;
        _servicioUsuario = servicioUsuario;
    }

    public ConfiguracionTorneo ObtenerConfiguracionActual() =>
        _servicioConfiguracionTorneo.ObtenerConfiguracionActual();

    public ConfiguracionTorneoDTO ObtenerConfiguracionActualDTO() =>
        ConfiguracionTorneoDTO.FromEntity(ObtenerConfiguracionActual());

    public void ModificarConfiguracion(UsuarioDTO usuario, ConfiguracionTorneo configuracion) =>
        _servicioConfiguracionTorneo.ModificarConfiguracion(_servicioUsuario.ResolverUsuario(usuario)!, configuracion);

    public void ModificarConfiguracion(UsuarioDTO usuario, ConfiguracionTorneoDTO configuracion) =>
        _servicioConfiguracionTorneo.ModificarConfiguracion(
            _servicioUsuario.ResolverUsuario(usuario)!,
            MapeadorDtoDominio.AConfiguracionTorneo(configuracion));

    public bool PuedeModificarConfiguracion(UsuarioDTO? usuario) =>
        _servicioConfiguracionTorneo.PuedeModificarConfiguracion(_servicioUsuario.ResolverUsuario(usuario)!);

    public bool EsConfiguracionModificable =>
        _servicioConfiguracionTorneo.EsConfiguracionModificable;

    public void RegistrarFixtureGenerado(ConfiguracionTorneo configuracionUsada) =>
        _servicioConfiguracionTorneo.RegistrarFixtureGenerado(configuracionUsada);

    public void RegistrarFixtureGenerado(ConfiguracionTorneoDTO configuracionUsada) =>
        _servicioConfiguracionTorneo.RegistrarFixtureGenerado(
            MapeadorDtoDominio.AConfiguracionTorneo(configuracionUsada));

    public ConfiguracionTorneo ObtenerConfiguracionUsadaParaFixture() =>
        _servicioConfiguracionTorneo.ObtenerConfiguracionUsadaParaFixture();

    public ConfiguracionTorneoDTO ObtenerConfiguracionUsadaParaFixtureDTO() =>
        ConfiguracionTorneoDTO.FromEntity(ObtenerConfiguracionUsadaParaFixture());

}
