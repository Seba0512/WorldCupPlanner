using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.Servicios;

public class ContextoCruceGrupo
{
    public PosicionEnGrupo PosicionEquipoLocal { get; set; }
    public PosicionEnGrupo PosicionEquipoVisitante { get; set; }
    public DateTime Fecha { get; set; }
}
