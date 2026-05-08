using WorldCupPlanner.Dominio.Entidades;

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
        }
    }
}
