using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.Servicios;

public class ServicioRankingDinamico
{
    private const int FactorK = 30;
    private const int RankingMinimo = 300;
    private const int RankingMaximo = 2500;
    private const double MultiplicadorFaseDeGrupos = 1.0;
    private const double MultiplicadorEliminatoria = 1.5;
    private const double BaseElo = 10.0;
    private const double DivisorElo = 1000.0;

    public double CalcularProbabilidad(int rankingPropio, int rankingRival)
    {
        return 1.0 / (1.0 + Math.Pow(BaseElo, (rankingRival - rankingPropio) / DivisorElo));
    }

    public int ActualizarRanking(int rankingActual, int rankingRival, double resultado, FaseTorneo fase)
    {
        var probabilidad = CalcularProbabilidad(rankingActual, rankingRival);
        var multiplicador = fase == FaseTorneo.FaseDeGrupos ? MultiplicadorFaseDeGrupos : MultiplicadorEliminatoria;
        var delta = (int)Math.Round(FactorK * (resultado - probabilidad) * multiplicador, MidpointRounding.AwayFromZero);
        return Math.Clamp(rankingActual + delta, RankingMinimo, RankingMaximo);
    }
}