using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.AccesoADatos.Interfaces;

public interface IRepositorioConfiguracionTorneo
{
    ConfiguracionTorneo ObtenerConfiguracionActual();
    void GuardarConfiguracionActual(ConfiguracionTorneo configuracion);
    ConfiguracionTorneo? ObtenerConfiguracionUsadaParaFixture();
    void GuardarConfiguracionUsadaParaFixture(ConfiguracionTorneo configuracion);
}
