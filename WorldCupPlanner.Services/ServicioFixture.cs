using WorldCupPlanner.Domain;

namespace WorldCupPlanner.Services;

public class ServicioFixture
{
    private const int CantidadMinimaEquipos = 48;
    private const int CantidadMinimaEstadios = 4;
    private const char PrimeraEtiquetaGrupo = 'A';
    private const char UltimaEtiquetaGrupo = 'L';
    private const int CantidadEquiposPorGrupo = 4;

    public Fixture GenerarFixture(List<Equipo> equipos, List<Estadio> estadios, ConfiguracionTorneo configuracionTorneo)
    {
        ValidarCantidadEquipos(equipos);
        ValidarCantidadEstadios(estadios);

        return CrearFixtureConGrupos(equipos);
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

    private Fixture CrearFixtureConGrupos(List<Equipo> equipos)
    {
        var fixture = new Fixture();
        var indiceEquipo = 0;

        for (char etiqueta = PrimeraEtiquetaGrupo; etiqueta <= UltimaEtiquetaGrupo; etiqueta++)
        {
            var equiposDelGrupo = new List<Equipo>();

            for (int i = 0; i < CantidadEquiposPorGrupo; i++)
            {
                equiposDelGrupo.Add(equipos[indiceEquipo]);
                indiceEquipo++;
            }

            fixture.Grupos.Add(new Grupo(etiqueta.ToString(), equiposDelGrupo));
        }

        return fixture;
    }
}