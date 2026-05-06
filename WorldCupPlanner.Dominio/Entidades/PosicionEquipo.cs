namespace WorldCupPlanner.Dominio.Entidades;

public class PosicionEquipo
{
    public Equipo Equipo { get; set; } = null!;
    public Grupo Grupo { get; set; } = null!;
    public int Puntos { get; set; }
    public int GolesAFavor { get; set; }
    public int GolesEnContra { get; set; }

    public int DiferenciaGoles
    {
        get
        {
            return GolesAFavor - GolesEnContra;
        }
    }
}
