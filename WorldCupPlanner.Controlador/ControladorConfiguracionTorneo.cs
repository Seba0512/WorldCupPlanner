using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.Controladores;

public class ControladorConfiguracionTorneo
{
    private readonly ServicioConfiguracionTorneo _servicioConfiguracionTorneo;

    public ControladorConfiguracionTorneo(ServicioConfiguracionTorneo servicioConfiguracionTorneo)
    {
        _servicioConfiguracionTorneo = servicioConfiguracionTorneo;
    }

    public ConfiguracionTorneo ObtenerConfiguracionActual() =>
        _servicioConfiguracionTorneo.ObtenerConfiguracionActual();

    public void ModificarConfiguracion(Usuario usuario, ConfiguracionTorneo configuracion) =>
        _servicioConfiguracionTorneo.ModificarConfiguracion(usuario, configuracion);

    public bool PuedeModificarConfiguracion(Usuario? usuario) =>
        _servicioConfiguracionTorneo.PuedeModificarConfiguracion(usuario);

    public bool EsConfiguracionModificable =>
        _servicioConfiguracionTorneo.EsConfiguracionModificable;

    public void RegistrarFixtureGenerado(ConfiguracionTorneo configuracionUsada) =>
        _servicioConfiguracionTorneo.RegistrarFixtureGenerado(configuracionUsada);

    public ConfiguracionTorneo ObtenerConfiguracionUsadaParaFixture() =>
        _servicioConfiguracionTorneo.ObtenerConfiguracionUsadaParaFixture();
}
