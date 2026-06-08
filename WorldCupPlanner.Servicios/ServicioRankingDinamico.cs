namespace WorldCupPlanner.Servicios;

public class ServicioRankingDinamico
{
    public double CalcularProbabilidad(int rankingPropio, int rankingRival)
    {
        return 1.0 / (1.0 + Math.Pow(10, (rankingRival - rankingPropio) / 1000.0));
    }
}