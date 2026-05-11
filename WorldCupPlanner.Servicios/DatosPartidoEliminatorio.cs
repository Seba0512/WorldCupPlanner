using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.Servicios;

public class DatosPartidoEliminatorio
{
    public int Id { get; set; }
    public Equipo EquipoLocal { get; set; } = null!;
    public Equipo EquipoVisitante { get; set; } = null!;
    public Estadio Estadio { get; set; } = null!;
    public DateTime Fecha { get; set; }
    public FaseTorneo Fase { get; set; }
}
