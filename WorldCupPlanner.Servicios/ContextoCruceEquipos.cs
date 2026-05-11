using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.Servicios;

public class ContextoCruceEquipos
{
    public List<Equipo> Locales { get; set; } = new();
    public List<Equipo> Visitantes { get; set; } = new();
    public ClasificadosFaseGrupos Clasificados { get; set; } = null!;
}
