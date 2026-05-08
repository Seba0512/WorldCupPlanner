using WorldCupPlanner.Dominio.Entidades;

using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.Servicios;

public class ServicioSimulacion
{
    private const int GolesMinimos = 0;
    private const int GolesMaximosExclusivo = 6;
    private const int GolesBaseMaximosExclusivo = 4;
    private const int RankingBajo = 1000;
    private const int RankingAlto = 1800;
    private const int BonusRankingMedio = 1;
    private const int BonusRankingAlto = 2;

    public void SimularResultados(
        List<Partido> partidos,
        ConfiguracionTorneo configuracionTorneo)
    {
        var random = new Random(configuracionTorneo.SemillaSimulation);

        foreach (var partido in partidos)
        {
            if (partido.EstaJugado())
            {
                continue;
            }

            partido.GolesLocal = GenerarGoles(partido.EquipoLocal, random);
            partido.GolesVisitante = GenerarGoles(partido.EquipoVisitante, random);

            if (EsFaseEliminatoria(partido) && partido.EsEmpate())
            {
                DesempatarPartido(partido);
            }
        }
    }

    private bool EsFaseEliminatoria(Partido partido)
    {
        return partido.Fase != FaseTorneo.FaseDeGrupos;
    }

    private int GenerarGoles(Equipo? equipo, Random random)
    {
        var goles = random.Next(GolesMinimos, GolesBaseMaximosExclusivo)
            + ObtenerBonusPorRanking(equipo);

        return Math.Min(goles, GolesMaximosExclusivo - 1);
    }

    private int ObtenerBonusPorRanking(Equipo? equipo)
    {
        if (equipo == null)
        {
            return 0;
        }

        if (equipo.RankingFIFA >= RankingAlto)
        {
            return BonusRankingAlto;
        }

        if (equipo.RankingFIFA >= RankingBajo)
        {
            return BonusRankingMedio;
        }

        return 0;
    }

    private void DesempatarPartido(Partido partido)
    {
        if (partido.GolesLocal < GolesMaximosExclusivo - 1)
        {
            partido.GolesLocal++;
        }
        else
        {
            partido.GolesVisitante--;
        }
    }
}
