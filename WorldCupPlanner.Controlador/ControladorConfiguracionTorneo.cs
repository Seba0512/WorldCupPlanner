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

    public void ModificarConfiguracion(UsuarioDTO usuario, ConfiguracionTorneo configuracion) =>
        _servicioConfiguracionTorneo.ModificarConfiguracion(ResolverUsuario(usuario), configuracion);

    public bool PuedeModificarConfiguracion(UsuarioDTO? usuario) =>
        _servicioConfiguracionTorneo.PuedeModificarConfiguracion(ResolverUsuarioOpcional(usuario));

    public bool EsConfiguracionModificable =>
        _servicioConfiguracionTorneo.EsConfiguracionModificable;

    public void RegistrarFixtureGenerado(ConfiguracionTorneo configuracionUsada) =>
        _servicioConfiguracionTorneo.RegistrarFixtureGenerado(configuracionUsada);

    public ConfiguracionTorneo ObtenerConfiguracionUsadaParaFixture() =>
        _servicioConfiguracionTorneo.ObtenerConfiguracionUsadaParaFixture();

    private Usuario ResolverUsuario(UsuarioDTO usuarioActual)
    {
        if (_servicioUsuario == null)
            throw new InvalidOperationException("El servicio de usuario no fue configurado.");

        return _servicioUsuario.ObtenerPorEmail(usuarioActual.Email)
               ?? throw new EntidadInexistenteExcepcion($"No existe un usuario con el email '{usuarioActual.Email}'.");
    }

    private Usuario? ResolverUsuarioOpcional(UsuarioDTO? usuarioActual) =>
        usuarioActual == null ? null : ResolverUsuario(usuarioActual);
}
