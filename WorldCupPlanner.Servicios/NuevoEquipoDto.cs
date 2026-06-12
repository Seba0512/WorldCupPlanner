using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.Servicios;

public class NuevoEquipoDto
{
    public string Nombre { get; set; } = string.Empty;
    public Confederacion? Confederacion { get; set; }
    public int RankingFIFA { get; set; }
    public string Bandera { get; set; } = string.Empty;
}
