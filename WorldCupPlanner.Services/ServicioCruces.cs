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
        var random = new Random(configuracionTorneo.SemillaCrucesFase);

        var primerosParaTerceros = MezclarEquiposConFisherYates(
            clasificados.Primeros.Take(8).ToList(),
            random);

        var terceros = MezclarEquiposConFisherYates(
            clasificados.MejoresTerceros.ToList(),
            random);

        AgregarCrucesPrimerosContraTerceros(
            partidos,
            primerosParaTerceros,
            terceros,
            estadios,
            configuracionTorneo
        );

        var primerosRestantes = MezclarEquiposConFisherYates(
            clasificados.Primeros.Skip(8).Take(4).ToList(),
            random);

        var segundosDeMenorRendimiento = MezclarEquiposConFisherYates(
            clasificados.Segundos.Skip(8).Take(4).ToList(),
            random);

        AgregarCrucesPrimerosRestantesContraSegundosDeMenorRendimiento(
            partidos,
            primerosRestantes,
            segundosDeMenorRendimiento,
            estadios,
            configuracionTorneo
        );

        var segundosRestantes = MezclarEquiposConFisherYates(
            clasificados.Segundos.Take(8).ToList(),
            random);

        AgregarCrucesSegundosRestantesMinimos(
            partidos,
            segundosRestantes,
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

    private void AgregarCrucesPrimerosRestantesContraSegundosDeMenorRendimiento(
        List<Partido> partidos,
        List<Equipo> primerosRestantes,
        List<Equipo> segundosDeMenorRendimiento,
        List<Estadio> estadios,
        ConfiguracionTorneo configuracionTorneo)
    {
        for (int i = 0; i < primerosRestantes.Count; i++)
        {
            partidos.Add(CrearPartidoDePrimeraRonda(
                partidos.Count + 1,
                primerosRestantes[i],
                segundosDeMenorRendimiento[i],
                estadios[partidos.Count % estadios.Count],
                configuracionTorneo));
        }
    }

    private void AgregarCrucesSegundosRestantesMinimos(
        List<Partido> partidos,
        List<Equipo> segundosRestantes,
        List<Estadio> estadios,
        ConfiguracionTorneo configuracionTorneo)
    {
        for (int i = 0; i < segundosRestantes.Count; i += 2)
        {
            partidos.Add(CrearPartidoDePrimeraRonda(
                partidos.Count + 1,
                segundosRestantes[i],
                segundosRestantes[i + 1],
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

    private List<Equipo> MezclarEquiposConFisherYates(List<Equipo> equipos, Random random)
    {
        for (int i = equipos.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (equipos[i], equipos[j]) = (equipos[j], equipos[i]);
        }

        return equipos;
    }
}
