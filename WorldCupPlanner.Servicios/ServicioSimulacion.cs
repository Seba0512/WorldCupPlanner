using WorldCupPlanner.Dominio.Entidades;

using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.Servicios;

public class ServicioSimulacion
{
    private const int GolesMinimos = 0;
    private const int GolesMaximosExclusivo = 6;

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

            partido.GolesLocal = random.Next(GolesMinimos, GolesMaximosExclusivo);
            partido.GolesVisitante = random.Next(GolesMinimos, GolesMaximosExclusivo);

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
