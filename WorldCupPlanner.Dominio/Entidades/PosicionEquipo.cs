namespace WorldCupPlanner.Dominio.Entidades;

public class PosicionEquipo
{
    private const int PosicionesQueClassifican = 2;
    private const int PosicionTercero = 3;

    public Equipo Equipo { get; set; } = null!;
    public Grupo Grupo { get; set; } = null!;
    public int Posicion { get; set; }
    public bool Clasifica => Posicion > 0 && Posicion <= PosicionesQueClassifican;
    public bool EsTercero => Posicion == PosicionTercero;
    public int PartidosJugados { get; set; }
    public int PartidosGanados { get; set; }
    public int PartidosEmpatados { get; set; }
    public int PartidosPerdidos { get; set; }
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
