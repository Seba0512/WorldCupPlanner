using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.Servicios;

public class FiltroPartidos
{
    public DateTime? Fecha { get; set; }
    public string Estadio { get; set; } = string.Empty;
    public string Grupo { get; set; } = string.Empty;
    public string Fase { get; set; } = string.Empty;
}
