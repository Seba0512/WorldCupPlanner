using WorldCupPlanner.Domain;

namespace WorldCupPlanner.Services;

public class ServicioCruces
{
    public List<Partido> GenerarPrimeraRondaEliminatoria(
        ClasificadosFaseGrupos clasificados,
        List<Estadio> estadios,
        ConfiguracionTorneo configuracionTorneo)
    {
        var partidos = new List<Partido>();

        AgregarCrucesPrimerosContraTerceros(
            partidos,
            clasificados.Primeros.Take(8).ToList(),
            clasificados.MejoresTerceros,
            estadios,
            configuracionTorneo
        );

        AgregarCrucesRestantesMinimos(
            partidos,
            clasificados,
            estadios,
            configuracionTorneo
        );

        return partidos;
    }

    private void AgregarCrucesPrimerosContraTerceros(
        List<Partido> partidos,
        List<Equipo> primeros,
        List<Equipo> terceros,
        List<Estadio> estadios,
        ConfiguracionTorneo configuracionTorneo)
    {
        for (int i = 0; i < primeros.Count; i++)
        {
            partidos.Add(CrearPartidoDePrimeraRonda(
                partidos.Count + 1,
                primeros[i],
                terceros[i],
                estadios[partidos.Count % estadios.Count],
                configuracionTorneo));
        }
    }

    private void AgregarCrucesRestantesMinimos(
        List<Partido> partidos,
        ClasificadosFaseGrupos clasificados,
        List<Estadio> estadios,
        ConfiguracionTorneo configuracionTorneo)
    {
        var equiposRestantes = clasificados.Primeros
            .Skip(8)
            .Concat(clasificados.Segundos)
            .ToList();

        for (int i = 0; i < equiposRestantes.Count; i += 2)
        {
            partidos.Add(CrearPartidoDePrimeraRonda(
                partidos.Count + 1,
                equiposRestantes[i],
                equiposRestantes[i + 1],
                estadios[partidos.Count % estadios.Count],
                configuracionTorneo));
        }
    }

    private Partido CrearPartidoDePrimeraRonda(
        int id,
        Equipo equipoLocal,
        Equipo equipoVisitante,
        Estadio estadio,
        ConfiguracionTorneo configuracionTorneo)
    {
        return new Partido
        {
            Id = id,
            EquipoLocal = equipoLocal,
            EquipoVisitante = equipoVisitante,
            Estadio = estadio,
            Fecha = configuracionTorneo.FechaInicioTorneo,
            Fase = FaseTorneo.PrimeraRondaEliminatoria
        };
    }
}
