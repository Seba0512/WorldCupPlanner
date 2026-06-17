using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.Servicios;

public class ContextoResultadoPartido
{
    public int GolesLocal { get; set; }
    public int GolesVisitante { get; set; }
    public int SemillaSimulation { get; set; }
    public ServicioNotificacion Notificacion { get; set; } = null!;
    public IEnumerable<Usuario> UsuariosNotificables { get; set; } = [];
}
