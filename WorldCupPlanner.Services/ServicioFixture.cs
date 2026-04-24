using WorldCupPlanner.Domain;

namespace WorldCupPlanner.Services;

public class ServicioFixture
{
    private const int CantidadMinimaEquipos = 48;
    private const int CantidadMinimaEstadios = 4;
    private const char PrimeraEtiquetaGrupo = 'A';
    private const char UltimaEtiquetaGrupo = 'L';

    public Fixture GenerarFixture(List<Equipo> equipos, List<Estadio> estadios, ConfiguracionTorneo configuracionTorneo)
    {
        ValidarCantidadEquipos(equipos);
        ValidarCantidadEstadios(estadios);

        return CrearFixtureConGrupos();
    }

    private void ValidarCantidadEquipos(List<Equipo> equipos)
    {
        if (equipos.Count < CantidadMinimaEquipos)
        {
            throw new InvalidOperationException("No se puede generar el fixture con menos de 48 equipos.");
        }
    }

    private void ValidarCantidadEstadios(List<Estadio> estadios)
    {
        if (estadios.Count < CantidadMinimaEstadios)
        {
            throw new InvalidOperationException("No se puede generar el fixture con menos de 4 estadios.");
        }
    }

    private Fixture CrearFixtureConGrupos()
    {
        var fixture = new Fixture();

        for (char etiqueta = PrimeraEtiquetaGrupo; etiqueta <= UltimaEtiquetaGrupo; etiqueta++)
        {
            fixture.Grupos.Add(new Grupo(etiqueta.ToString()));
        }

        return fixture;
    }
}