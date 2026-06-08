using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.Servicios;

public class ServicioRankingDinamico
{
    public double CalcularProbabilidad(int rankingPropio, int rankingRival)
    {
        return 1.0 / (1.0 + Math.Pow(10, (rankingRival - rankingPropio) / 1000.0));
    }

    public int ActualizarRanking(int rankingActual, int rankingRival, double resultado, FaseTorneo fase)
    {
        var probabilidad = CalcularProbabilidad(rankingActual, rankingRival);
        var multiplicador = fase == FaseTorneo.FaseDeGrupos ? 1.0 : 1.5;
        var nuevo = rankingActual + (int)Math.Round(30 * (resultado - probabilidad) * multiplicador, MidpointRounding.AwayFromZero);
        return Math.Clamp(nuevo, 300, 2500);
    }
}