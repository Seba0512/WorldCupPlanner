using  WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Dominio.Excepciones;

namespace WorldCupPlanner.Servicios;

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
            clasificados,
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
            clasificados,
            estadios,
            configuracionTorneo
        );

        var segundosRestantes = MezclarEquiposConFisherYates(
            clasificados.Segundos.Take(8).ToList(),
            random);

        AgregarCrucesSegundosRestantesMinimos(
            partidos,
            segundosRestantes,
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
        ClasificadosFaseGrupos clasificados,
        List<Estadio> estadios,
        ConfiguracionTorneo configuracionTorneo)
    {
        ReordenarVisitantesParaEvitarMismoGrupo(primeros, terceros, clasificados);

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
        ClasificadosFaseGrupos clasificados,
        List<Estadio> estadios,
        ConfiguracionTorneo configuracionTorneo)
    {
        ReordenarVisitantesParaEvitarMismoGrupo(
            primerosRestantes,
            segundosDeMenorRendimiento,
            clasificados);

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
        ClasificadosFaseGrupos clasificados,
        List<Estadio> estadios,
        ConfiguracionTorneo configuracionTorneo)
    {
        ReordenarEquiposParaEvitarCrucesInternosDelMismoGrupo(segundosRestantes, clasificados);

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

    private void ReordenarVisitantesParaEvitarMismoGrupo(
        List<Equipo> locales,
        List<Equipo> visitantes,
        ClasificadosFaseGrupos clasificados)
    {
        for (int i = 0; i < locales.Count; i++)
        {
            if (!PertenecenAlMismoGrupo(locales[i], visitantes[i], clasificados))
            {
                continue;
            }

            var indiceAlternativo = BuscarVisitanteAlternativo(locales, visitantes, clasificados, i);

            if (indiceAlternativo.HasValue)
            {
                (visitantes[i], visitantes[indiceAlternativo.Value]) =
                    (visitantes[indiceAlternativo.Value], visitantes[i]);
            }
        }
    }

    private int? BuscarVisitanteAlternativo(
        List<Equipo> locales,
        List<Equipo> visitantes,
        ClasificadosFaseGrupos clasificados,
        int indiceActual)
    {
        for (int i = 0; i < visitantes.Count; i++)
        {
            if (i == indiceActual)
            {
                continue;
            }

            var evitaCruceActual = !PertenecenAlMismoGrupo(
                locales[indiceActual],
                visitantes[i],
                clasificados);

            var noRompeCruceFuturo = !PertenecenAlMismoGrupo(
                locales[i],
                visitantes[indiceActual],
                clasificados);

            if (evitaCruceActual && noRompeCruceFuturo)
            {
                return i;
            }
        }

        return null;
    }

    private void ReordenarEquiposParaEvitarCrucesInternosDelMismoGrupo(
        List<Equipo> equipos,
        ClasificadosFaseGrupos clasificados)
    {
        for (int i = 0; i < equipos.Count; i += 2)
        {
            if (!PertenecenAlMismoGrupo(equipos[i], equipos[i + 1], clasificados))
            {
                continue;
            }

            var indiceAlternativo = BuscarRivalAlternativo(equipos, clasificados, i);

            if (indiceAlternativo.HasValue)
            {
                (equipos[i + 1], equipos[indiceAlternativo.Value]) =
                    (equipos[indiceAlternativo.Value], equipos[i + 1]);
            }
        }
    }

    private int? BuscarRivalAlternativo(
        List<Equipo> equipos,
        ClasificadosFaseGrupos clasificados,
        int indiceLocal)
    {
        var indiceVisitante = indiceLocal + 1;

        for (int i = 0; i < equipos.Count; i++)
        {
            if (i == indiceLocal || i == indiceVisitante)
            {
                continue;
            }

            var indiceCompanero = ObtenerIndiceCompaneroDeCruce(i);

            var evitaCruceActual = !PertenecenAlMismoGrupo(
                equipos[indiceLocal],
                equipos[i],
                clasificados);

            var noRompeCruceAlternativo = !PertenecenAlMismoGrupo(
                equipos[indiceCompanero],
                equipos[indiceVisitante],
                clasificados);

            if (evitaCruceActual && noRompeCruceAlternativo)
            {
                return i;
            }
        }

        return null;
    }

    private int ObtenerIndiceCompaneroDeCruce(int indice)
    {
        return indice % 2 == 0 ? indice + 1 : indice - 1;
    }

    private bool PertenecenAlMismoGrupo(
        Equipo equipo1,
        Equipo equipo2,
        ClasificadosFaseGrupos clasificados)
    {
        var grupo1 = clasificados.ObtenerGrupoDeOrigen(equipo1);
        var grupo2 = clasificados.ObtenerGrupoDeOrigen(equipo2);

        return grupo1 != null && ReferenceEquals(grupo1, grupo2);
    }
    
    public List<Partido> GenerarOctavos(
        List<Partido> partidosPrimeraRonda,
        List<Estadio> estadios,
        ConfiguracionTorneo configuracionTorneo)
    {
        ValidarPartidosDeFaseAnterior(
            partidosPrimeraRonda,
            16,
            FaseTorneo.PrimeraRondaEliminatoria,
            "octavos",
            "primera ronda",
            "primera ronda eliminatoria");

        var ganadores = ObtenerGanadores(partidosPrimeraRonda);
        var partidos = new List<Partido>();
        var proximoId = partidosPrimeraRonda.Max(p => p.Id) + 1;
        var fecha = configuracionTorneo.FechaInicioTorneo.AddDays(configuracionTorneo.SeparacionEntreFechas);

        for (int i = 0; i < 8; i++)
        {
            partidos.Add(CrearPartidoEliminatorio(
                proximoId++,
                ganadores[i],
                ganadores[i + 8],
                estadios[partidos.Count % estadios.Count],
                fecha,
                FaseTorneo.Octavos));
        }

        return partidos;
    }

    public List<Partido> GenerarCuartos(
        List<Partido> octavos,
        List<Estadio> estadios,
        ConfiguracionTorneo configuracionTorneo)
    {
        ValidarPartidosDeFaseAnterior(
            octavos,
            8,
            FaseTorneo.Octavos,
            "cuartos",
            "octavos");

        var ganadores = ObtenerGanadores(octavos);
        var partidos = new List<Partido>();
        var proximoId = octavos.Max(p => p.Id) + 1;
        var fecha = configuracionTorneo.FechaInicioTorneo.AddDays(configuracionTorneo.SeparacionEntreFechas * 2);

        for (int i = 0; i < 4; i++)
        {
            partidos.Add(CrearPartidoEliminatorio(
                proximoId++,
                ganadores[i * 2],
                ganadores[i * 2 + 1],
                estadios[partidos.Count % estadios.Count],
                fecha,
                FaseTorneo.Cuartos));
        }

        return partidos;
    }

    public List<Partido> GenerarSemifinales(
        List<Partido> cuartos,
        List<Estadio> estadios,
        ConfiguracionTorneo configuracionTorneo)
    {
        ValidarPartidosDeFaseAnterior(
            cuartos,
            4,
            FaseTorneo.Cuartos,
            "semifinales",
            "cuartos");

        var ganadores = ObtenerGanadores(cuartos);
        var partidos = new List<Partido>();
        var proximoId = cuartos.Max(p => p.Id) + 1;
        var fecha = configuracionTorneo.FechaInicioTorneo.AddDays(configuracionTorneo.SeparacionEntreFechas * 3);

        for (int i = 0; i < 2; i++)
        {
            partidos.Add(CrearPartidoEliminatorio(
                proximoId++,
                ganadores[i * 2],
                ganadores[i * 2 + 1],
                estadios[partidos.Count % estadios.Count],
                fecha,
                FaseTorneo.Semifinal));
        }

        return partidos;
    }

    private void ValidarPartidosDeFaseAnterior(
        List<Partido> partidos,
        int cantidadEsperada,
        FaseTorneo faseEsperada,
        string faseAGenerar,
        string faseAnterior,
        string? faseAnteriorEsperada = null)
    {
        ValidarCantidadPartidos(partidos, cantidadEsperada, faseAGenerar, faseAnterior);
        ValidarFasePartidos(partidos, faseEsperada, faseAGenerar, faseAnteriorEsperada ?? faseAnterior);
        ValidarPartidosJugados(partidos, faseAGenerar, faseAnterior);
        ValidarPartidosConGanador(partidos, faseAGenerar, faseAnterior);
    }

    private void ValidarCantidadPartidos(
        List<Partido> partidos,
        int cantidadEsperada,
        string faseAGenerar,
        string faseAnterior)
    {
        if (partidos.Count != cantidadEsperada)
        {
            throw new EntidadInvalidaExcepcion(
                $"Para generar {faseAGenerar} deben existir exactamente {cantidadEsperada} partidos de {faseAnterior}.");
        }
    }

    private void ValidarFasePartidos(
        List<Partido> partidos,
        FaseTorneo faseEsperada,
        string faseAGenerar,
        string faseAnterior)
    {
        if (partidos.Any(p => p.Fase != faseEsperada))
        {
            throw new EntidadInvalidaExcepcion(
                $"Para generar {faseAGenerar} todos los partidos deben ser de {faseAnterior}.");
        }
    }

    private void ValidarPartidosJugados(
        List<Partido> partidos,
        string faseAGenerar,
        string faseAnterior)
    {
        if (partidos.Any(p => !p.EstaJugado()))
        {
            throw new EntidadInvalidaExcepcion(
                $"No se pueden generar {faseAGenerar} si hay partidos de {faseAnterior} sin jugar.");
        }
    }

    private void ValidarPartidosConGanador(
        List<Partido> partidos,
        string faseAGenerar,
        string faseAnterior)
    {
        if (partidos.Any(p => p.Ganador() == null))
        {
            throw new EntidadInvalidaExcepcion(
                $"No se pueden generar {faseAGenerar} si hay partidos de {faseAnterior} empatados.");
        }
    }

    private List<Equipo> ObtenerGanadores(List<Partido> partidos)
    {
        return partidos
            .Select(p => p.Ganador()!)
            .ToList();
    }

    private Partido CrearPartidoEliminatorio(
        int id,
        Equipo equipoLocal,
        Equipo equipoVisitante,
        Estadio estadio,
        DateTime fecha,
        FaseTorneo fase)
    {
        return new Partido
        {
            Id = id,
            EquipoLocal = equipoLocal,
            EquipoVisitante = equipoVisitante,
            Estadio = estadio,
            Fecha = fecha,
            Fase = fase
        };
    }
    
    public Partido GenerarTercerPuesto(
        List<Partido> semifinales,
        List<Estadio> estadios,
        ConfiguracionTorneo configuracionTorneo)
    {
        ValidarCantidadSemifinales(semifinales);
        ValidarFaseSemifinales(semifinales);

        var perdedores = semifinales
            .Select(ObtenerPerdedor)
            .ToList();

        return new Partido
        {
            Id = semifinales.Max(p => p.Id) + 1,
            EquipoLocal = perdedores[0],
            EquipoVisitante = perdedores[1],
            Estadio = estadios[0],
            Fecha = configuracionTorneo.FechaInicioTorneo.AddDays(configuracionTorneo.SeparacionEntreFechas * 4),
            Fase = FaseTorneo.TercerPuesto
        };
    }
    
    private void ValidarFaseSemifinales(List<Partido> semifinales)
    {
        if (semifinales.Any(p => p.Fase != FaseTorneo.Semifinal))
        {
            throw new EntidadInvalidaExcepcion("Para generar tercer puesto todos los partidos deben ser semifinales.");
        }
    }
    
    private void ValidarCantidadSemifinales(List<Partido> semifinales)
    {
        if (semifinales.Count != 2)
        {
            throw new EntidadInvalidaExcepcion("Para generar tercer puesto deben existir exactamente 2 semifinales.");
        }
    }

    private Equipo? ObtenerPerdedor(Partido partido)
    {
        if (!partido.EstaJugado())
        {
            return null;
        }

        if (partido.GolesLocal < partido.GolesVisitante)
        {
            return partido.EquipoLocal;
        }

        if (partido.GolesVisitante < partido.GolesLocal)
        {
            return partido.EquipoVisitante;
        }

        return null;
    }
    
    
}
