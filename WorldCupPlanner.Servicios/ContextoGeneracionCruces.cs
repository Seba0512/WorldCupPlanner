using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.Servicios;

public class ContextoGeneracionCruces
{
    public List<Partido> Partidos { get; set; } = new();
    public ClasificadosFaseGrupos Clasificados { get; set; } = null!;
    public List<Estadio> Estadios { get; set; } = new();
    public ConfiguracionTorneo ConfiguracionTorneo { get; set; } = null!;
}
