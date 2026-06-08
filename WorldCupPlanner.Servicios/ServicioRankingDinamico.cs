using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Dominio.Entidades;
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

    private readonly IRepositorio<Equipo>? _repositorioEquipo;

    public ServicioRankingDinamico(IRepositorio<Equipo>? repositorioEquipo = null)
    {
        _repositorioEquipo = repositorioEquipo;
    }

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

    public void ActualizarRankingPartido(Partido partido)
    {
        if (!partido.EstaJugado()) return;

        var resultadoLocal = DeterminarResultado(partido.GolesLocal!.Value, partido.GolesVisitante!.Value);
        ActualizarEquipo(partido.EquipoLocal!, partido.EquipoVisitante!.RankingFIFA, resultadoLocal, partido.Fase);
        ActualizarEquipo(partido.EquipoVisitante!, partido.EquipoLocal!.RankingFIFA, 1.0 - resultadoLocal, partido.Fase);
    }

    private void ActualizarEquipo(Equipo equipo, int rankingRival, double resultado, FaseTorneo fase)
    {
        equipo.RankingFIFA = ActualizarRanking(equipo.RankingFIFA, rankingRival, resultado, fase);
        _repositorioEquipo?.Actualizar(equipo);
    }

    private static double DeterminarResultado(int golesLocal, int golesVisitante)
    {
        if (golesLocal > golesVisitante) return 1.0;
        if (golesVisitante > golesLocal) return 0.0;
        return 0.5;
    }
}