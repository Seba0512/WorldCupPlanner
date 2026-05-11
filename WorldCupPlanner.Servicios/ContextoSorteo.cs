using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.Servicios;

public class ContextoSorteo
{
    public ClasificadosFaseGrupos Clasificados { get; set; } = null!;
    public List<Estadio> Estadios { get; set; } = new();
    public ConfiguracionTorneo ConfiguracionTorneo { get; set; } = null!;
}
