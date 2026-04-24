using WorldCupPlanner.Domain;

namespace WorldCupPlanner.Services;

public class ServicioFixture
{
    private const int CantidadMinimaEquipos = 48;
    private const int CantidadMinimaEstadios = 4;

    public Fixture GenerarFixture(List<Equipo> equipos, List<Estadio> estadios, ConfiguracionTorneo configuracionTorneo)
    {
        if (equipos.Count < CantidadMinimaEquipos)
        {
            throw new InvalidOperationException("No se puede generar el fixture con menos de 48 equipos.");
        }

        if (estadios.Count < CantidadMinimaEstadios)
        {
            throw new InvalidOperationException("No se puede generar el fixture con menos de 4 estadios.");
        }

        return new Fixture();
    }
}