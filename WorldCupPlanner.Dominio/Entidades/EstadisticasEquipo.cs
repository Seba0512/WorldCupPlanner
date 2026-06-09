namespace WorldCupPlanner.Dominio.Entidades;

public class EstadisticasEquipo
{
    public int PartidosJugados { get; set; }
    public int GolesAFavor { get; set; }
    public int GolesEnContra { get; set; }
    public int DiferenciaGoles => GolesAFavor - GolesEnContra;
    public int Puntos { get; set; }
}
