using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.Servicios;

public class DatosPartidoDeGrupo
{
    public Grupo Grupo { get; set; } = null!;
    public Equipo EquipoLocal { get; set; } = null!;
    public Equipo EquipoVisitante { get; set; } = null!;
    public DateTime Fecha { get; set; }
    public Estadio Estadio { get; set; } = null!;
    public int Id { get; set; }
}
