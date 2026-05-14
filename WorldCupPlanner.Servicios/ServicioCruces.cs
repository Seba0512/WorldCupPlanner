using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Dominio.Excepciones;

namespace WorldCupPlanner.Servicios;

public class ServicioCruces
{
    private const int PrimerIdPrimeraRondaEliminatoria = 73;

    private const int CantidadPrimerosSorteadosContraTerceros = 8;
    private const int CantidadPrimerosRestantes = 4;
    private const int PasoEntreParesDeEquipos = 2;

    private const int CantidadPartidosPrimeraRonda = 16;
    private const int CantidadPartidosOctavos = 8;
    private const int CantidadPartidosCuartos = 4;
    private const int CantidadPartidosSemifinales = 2;

    // La final se programa con el doble de separación respecto a la semifinal para dar lugar al partido de tercer puesto
    private const int MultiplicadorSeparacionFinal = 2;
    // El tercer puesto ocupa el ID siguiente al de las semifinales (+1); la final va un lugar después
    private const int OffsetIdFinal = 2;

    public List<Partido> GenerarPrimeraRondaEliminatoria(ContextoSorteo sorteo, Random? random = null)
    {
        var partidos = new List<Partido>();
        random ??= new Random(sorteo.ConfiguracionTorneo.SemillaCrucesFase);
        var contexto = CrearContextoGeneracion(sorteo, partidos);

        var (primerosParaTerceros, terceros) = PrepararPrimerosYTerceros(sorteo, random);
        AgregarCrucesPrimerosContraTerceros(primerosParaTerceros, terceros, contexto);

        var (primerosRestantes, segundosDeMenorRendimiento) = PrepararPrimerosYSegundosRestantes(sorteo, random);
        AgregarCrucesPrimerosRestantesContraSegundosDeMenorRendimiento(primerosRestantes, segundosDeMenorRendimiento, contexto);

        var segundosRestantes = PrepararSegundosParaCruceMinimo(sorteo, random);
        AgregarCrucesSegundosRestantesMinimos(segundosRestantes, contexto);

        return partidos;
    }

    private ContextoGeneracionCruces CrearContextoGeneracion(ContextoSorteo sorteo, List<Partido> partidos)
    {
        return new ContextoGeneracionCruces
        {
            Partidos = partidos,
            Clasificados = sorteo.Clasificados,
            Estadios = sorteo.Estadios,
            ConfiguracionTorneo = sorteo.ConfiguracionTorneo,
            FechaUltimoPartidoFaseGrupos = sorteo.FechaUltimoPartidoFaseGrupos
        };
    }

    private (List<Equipo> Primeros, List<Equipo> Terceros) PrepararPrimerosYTerceros(ContextoSorteo sorteo, Random random)
    {
        var primeros = MezclarEquiposConFisherYates(
            new List<Equipo>(sorteo.Clasificados.Primeros.Take(CantidadPrimerosSorteadosContraTerceros)), random);
        var terceros = MezclarEquiposConFisherYates(
            new List<Equipo>(sorteo.Clasificados.MejoresTerceros), random);
        return (primeros, terceros);
    }

    private (List<Equipo> Primeros, List<Equipo> Segundos) PrepararPrimerosYSegundosRestantes(ContextoSorteo sorteo, Random random)
    {
        var primeros = MezclarEquiposConFisherYates(
            new List<Equipo>(sorteo.Clasificados.Primeros.Skip(CantidadPrimerosSorteadosContraTerceros).Take(CantidadPrimerosRestantes)), random);
        var segundos = MezclarEquiposConFisherYates(
            new List<Equipo>(sorteo.Clasificados.Segundos.Skip(CantidadPrimerosSorteadosContraTerceros).Take(CantidadPrimerosRestantes)), random);
        return (primeros, segundos);
    }

    private List<Equipo> PrepararSegundosParaCruceMinimo(ContextoSorteo sorteo, Random random)
    {
        return MezclarEquiposConFisherYates(
            new List<Equipo>(sorteo.Clasificados.Segundos.Take(CantidadPrimerosSorteadosContraTerceros)), random);
    }

    private void AgregarCrucesPrimerosContraTerceros(
        List<Equipo> primeros,
        List<Equipo> terceros,
        ContextoGeneracionCruces contexto)
    {
        ReordenarVisitantesParaEvitarMismoGrupo(new ContextoCruceEquipos
        {
            Locales = primeros,
            Visitantes = terceros,
            Clasificados = contexto.Clasificados
        });
        AgregarCrucesEntreListas(primeros, terceros, contexto);
    }

    private void AgregarCrucesPrimerosRestantesContraSegundosDeMenorRendimiento(
        List<Equipo> primerosRestantes,
        List<Equipo> segundosDeMenorRendimiento,
        ContextoGeneracionCruces contexto)
    {
        ReordenarVisitantesParaEvitarMismoGrupo(new ContextoCruceEquipos
        {
            Locales = primerosRestantes,
            Visitantes = segundosDeMenorRendimiento,
            Clasificados = contexto.Clasificados
        });
        AgregarCrucesEntreListas(primerosRestantes, segundosDeMenorRendimiento, contexto);
    }

    private void AgregarCrucesSegundosRestantesMinimos(
        List<Equipo> segundosRestantes,
        ContextoGeneracionCruces contexto)
    {
        ReordenarEquiposParaEvitarCrucesInternosDelMismoGrupo(segundosRestantes, contexto.Clasificados);
        var locales = segundosRestantes.Where((_, i) => i % PasoEntreParesDeEquipos == 0).ToList();
        var visitantes = segundosRestantes.Where((_, i) => i % PasoEntreParesDeEquipos != 0).ToList();
        AgregarCrucesEntreListas(locales, visitantes, contexto);
    }

    private void AgregarCrucesEntreListas(
        List<Equipo> locales,
        List<Equipo> visitantes,
        ContextoGeneracionCruces contexto)
    {
        var fechaInicioDeFase = ObtenerFechaInicioPrimeraRonda(contexto);
        for (int i = 0; i < locales.Count; i++)
        {
            contexto.Partidos.Add(CrearPartidoEliminatorio(new DatosPartidoEliminatorio
            {
                Id = PrimerIdPrimeraRondaEliminatoria + contexto.Partidos.Count,
                EquipoLocal = locales[i],
                EquipoVisitante = visitantes[i],
                Estadio = contexto.Estadios[contexto.Partidos.Count % contexto.Estadios.Count],
                Fecha = CalcularFechaPartidoEliminatorio(fechaInicioDeFase, contexto.Partidos.Count, contexto.ConfiguracionTorneo),
                Fase = FaseTorneo.PrimeraRondaEliminatoria
            }));
        }
    }

    private Partido CrearPartidoEliminatorio(DatosPartidoEliminatorio datos)
    {
        return new Partido
        {
            Id = datos.Id,
            EquipoLocal = datos.EquipoLocal,
            EquipoVisitante = datos.EquipoVisitante,
            Estadio = datos.Estadio,
            Fecha = datos.Fecha,
            Fase = datos.Fase
        };
    }

    private DateTime ObtenerFechaInicioPrimeraRonda(ContextoGeneracionCruces contexto)
    {
        var fechaBase = contexto.FechaUltimoPartidoFaseGrupos?.Date
            ?? contexto.ConfiguracionTorneo.FechaInicioTorneo.Date;

        return fechaBase.AddDays(contexto.ConfiguracionTorneo.SeparacionEntreFechas);
    }

    private List<Equipo> MezclarEquiposConFisherYates(List<Equipo> equipos, Random random)
    {
        AlgoritmoFisherYates.Mezclar(equipos, random);
        return equipos;
    }

    private void ReordenarVisitantesParaEvitarMismoGrupo(ContextoCruceEquipos cruceEquipos)
    {
        for (int i = 0; i < cruceEquipos.Locales.Count; i++)
        {
            if (!PertenecenAlMismoGrupo(cruceEquipos.Locales[i], cruceEquipos.Visitantes[i], cruceEquipos.Clasificados))
            {
                continue;
            }

            var indiceAlternativo = BuscarVisitanteAlternativo(cruceEquipos, i);

            if (indiceAlternativo.HasValue)
            {
                (cruceEquipos.Visitantes[i], cruceEquipos.Visitantes[indiceAlternativo.Value]) =
                    (cruceEquipos.Visitantes[indiceAlternativo.Value], cruceEquipos.Visitantes[i]);
            }
        }
    }

    private int? BuscarVisitanteAlternativo(ContextoCruceEquipos cruceEquipos, int indiceActual)
    {
        for (int i = 0; i < cruceEquipos.Visitantes.Count; i++)
        {
            if (i == indiceActual) continue;
            if (EsVisitanteAlternativoValido(cruceEquipos, indiceActual, i)) return i;
        }

        return null;
    }

    private bool EsVisitanteAlternativoValido(ContextoCruceEquipos cruceEquipos, int indiceActual, int indiceAlternativo)
    {
        var evitaCruceActual = !PertenecenAlMismoGrupo(
            cruceEquipos.Locales[indiceActual],
            cruceEquipos.Visitantes[indiceAlternativo],
            cruceEquipos.Clasificados);

        var noRompeCruceFuturo = !PertenecenAlMismoGrupo(
            cruceEquipos.Locales[indiceAlternativo],
            cruceEquipos.Visitantes[indiceActual],
            cruceEquipos.Clasificados);

        return evitaCruceActual && noRompeCruceFuturo;
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

    private int? BuscarRivalAlternativo(List<Equipo> equipos, ClasificadosFaseGrupos clasificados, int indiceLocal)
    {
        var indiceVisitante = indiceLocal + 1;

        for (int i = 0; i < equipos.Count; i++)
        {
            if (i == indiceLocal || i == indiceVisitante) continue;
            if (EsIndiceDeIntercambioValido(equipos, clasificados, indiceLocal, indiceVisitante, i)) return i;
        }

        return null;
    }

    private bool EsIndiceDeIntercambioValido(
        List<Equipo> equipos,
        ClasificadosFaseGrupos clasificados,
        int indiceLocal,
        int indiceVisitante,
        int indiceAlternativo)
    {
        var indiceCompanero = ObtenerIndiceCompaneroDeCruce(indiceAlternativo);
        return !PertenecenAlMismoGrupo(equipos[indiceLocal], equipos[indiceAlternativo], clasificados)
            && !PertenecenAlMismoGrupo(equipos[indiceCompanero], equipos[indiceVisitante], clasificados);
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
            new ContextoValidacionFase
            {
                CantidadEsperada = CantidadPartidosPrimeraRonda,
                FaseEsperada = FaseTorneo.PrimeraRondaEliminatoria,
                FaseAGenerar = "octavos",
                FaseAnterior = "primera ronda",
                FaseAnteriorEsperada = "primera ronda eliminatoria"
            });

        var ganadores = ObtenerGanadores(partidosPrimeraRonda);
        var partidos = new List<Partido>();
        var proximoId = partidosPrimeraRonda.Max(p => p.Id) + 1;
        var fechaInicio = partidosPrimeraRonda.Max(p => p.Fecha.Date).AddDays(configuracionTorneo.SeparacionEntreFechas);

        for (int i = 0; i < CantidadPartidosOctavos; i++)
        {
            partidos.Add(CrearPartidoEliminatorio(new DatosPartidoEliminatorio
            {
                Id = proximoId++,
                EquipoLocal = ganadores[i],
                EquipoVisitante = ganadores[i + CantidadPartidosOctavos],
                Estadio = estadios[partidos.Count % estadios.Count],
                Fecha = CalcularFechaPartidoEliminatorio(fechaInicio, i, configuracionTorneo),
                Fase = FaseTorneo.Octavos
            }));
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
            new ContextoValidacionFase
            {
                CantidadEsperada = CantidadPartidosOctavos,
                FaseEsperada = FaseTorneo.Octavos,
                FaseAGenerar = "cuartos",
                FaseAnterior = "octavos"
            });

        var ganadores = ObtenerGanadores(octavos);
        var partidos = new List<Partido>();
        var proximoId = octavos.Max(p => p.Id) + 1;
        var fechaInicio = octavos.Max(p => p.Fecha.Date).AddDays(configuracionTorneo.SeparacionEntreFechas);

        for (int i = 0; i < CantidadPartidosCuartos; i++)
        {
            partidos.Add(CrearPartidoEliminatorio(new DatosPartidoEliminatorio
            {
                Id = proximoId++,
                EquipoLocal = ganadores[i * 2],
                EquipoVisitante = ganadores[i * 2 + 1],
                Estadio = estadios[partidos.Count % estadios.Count],
                Fecha = CalcularFechaPartidoEliminatorio(fechaInicio, i, configuracionTorneo),
                Fase = FaseTorneo.Cuartos
            }));
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
            new ContextoValidacionFase
            {
                CantidadEsperada = CantidadPartidosCuartos,
                FaseEsperada = FaseTorneo.Cuartos,
                FaseAGenerar = "semifinales",
                FaseAnterior = "cuartos"
            });

        var ganadores = ObtenerGanadores(cuartos);
        var partidos = new List<Partido>();
        var proximoId = cuartos.Max(p => p.Id) + 1;
        var fechaInicio = cuartos.Max(p => p.Fecha.Date).AddDays(configuracionTorneo.SeparacionEntreFechas);

        for (int i = 0; i < CantidadPartidosSemifinales; i++)
        {
            partidos.Add(CrearPartidoEliminatorio(new DatosPartidoEliminatorio
            {
                Id = proximoId++,
                EquipoLocal = ganadores[i * 2],
                EquipoVisitante = ganadores[i * 2 + 1],
                Estadio = estadios[partidos.Count % estadios.Count],
                Fecha = CalcularFechaPartidoEliminatorio(fechaInicio, i, configuracionTorneo),
                Fase = FaseTorneo.Semifinal
            }));
        }

        return partidos;
    }

    private void ValidarFaseNoGenerada(IReadOnlyList<Partido>? existentes, FaseTorneo fase, string nombreFase)
    {
        if (existentes != null && existentes.Any(p => p.Fase == fase))
        {
            throw new EntidadInvalidaExcepcion($"Ya existe un partido de {nombreFase}. No se puede generar nuevamente.");
        }
    }

    private void ValidarPartidosDeFaseAnterior(List<Partido> partidos, ContextoValidacionFase contexto)
    {
        ValidarCantidadPartidos(partidos, contexto);
        ValidarFasePartidos(partidos, contexto);
        ValidarPartidosJugados(partidos, contexto.FaseAGenerar, contexto.FaseAnterior);
        ValidarPartidosConGanador(partidos, contexto.FaseAGenerar, contexto.FaseAnterior);
    }

    private void ValidarCantidadPartidos(List<Partido> partidos, ContextoValidacionFase contexto)
    {
        if (partidos.Count != contexto.CantidadEsperada)
        {
            throw new EntidadInvalidaExcepcion(
                $"Para generar {contexto.FaseAGenerar} deben existir exactamente {contexto.CantidadEsperada} partidos de {contexto.FaseAnterior}.");
        }
    }

    private void ValidarFasePartidos(List<Partido> partidos, ContextoValidacionFase contexto)
    {
        if (partidos.Any(p => p.Fase != contexto.FaseEsperada))
        {
            var faseAnteriorDescripcion = contexto.FaseAnteriorEsperada ?? contexto.FaseAnterior;
            throw new EntidadInvalidaExcepcion(
                $"Para generar {contexto.FaseAGenerar} todos los partidos deben ser de {faseAnteriorDescripcion}.");
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
        return new List<Equipo>(partidos
            .Select(p => p.Ganador()!));
    }

    public Partido GenerarTercerPuesto(
        List<Partido> semifinales,
        List<Estadio> estadios,
        ConfiguracionTorneo configuracionTorneo,
        IReadOnlyList<Partido>? existentes = null)
    {
        ValidarFaseNoGenerada(existentes, FaseTorneo.TercerPuesto, "tercer puesto");
        ValidarPartidosDeFaseAnterior(
            semifinales,
            new ContextoValidacionFase
            {
                CantidadEsperada = CantidadPartidosSemifinales,
                FaseEsperada = FaseTorneo.Semifinal,
                FaseAGenerar = "tercer puesto",
                FaseAnterior = "semifinales"
            });

        var perdedores = ObtenerPerdedores(semifinales);

        var fechaTercerPuesto = CalcularFechaPartidoEliminatorio(
            semifinales.Max(p => p.Fecha.Date).AddDays(configuracionTorneo.SeparacionEntreFechas),
            0,
            configuracionTorneo);

        return CrearPartidoEliminatorio(new DatosPartidoEliminatorio
        {
            Id = semifinales.Max(p => p.Id) + 1,
            EquipoLocal = perdedores[0],
            EquipoVisitante = perdedores[1],
            Estadio = estadios[0],
            Fecha = fechaTercerPuesto,
            Fase = FaseTorneo.TercerPuesto
        });
    }

    private DateTime CalcularFechaPartidoEliminatorio(
        DateTime fechaInicioDeFase,
        int indicePartidoEnFase,
        ConfiguracionTorneo configuracionTorneo)
    {
        var diaOffset = indicePartidoEnFase / configuracionTorneo.MaxPartidosPorDia;
        var horaOffset = indicePartidoEnFase % configuracionTorneo.MaxPartidosPorDia;

        return fechaInicioDeFase.Date
            .AddDays(diaOffset)
            .Add(configuracionTorneo.HoraInicioPartidos)
            .AddHours(configuracionTorneo.SeparacionHorasEntrePartidos * horaOffset);
    }

    private List<Equipo> ObtenerPerdedores(List<Partido> partidos)
    {
        return new List<Equipo>(partidos
            .Select(ObtenerPerdedor));
    }

    private Equipo ObtenerPerdedor(Partido partido)
    {
        var ganador = partido.Ganador();

        if (ganador == null)
        {
            throw new EntidadInvalidaExcepcion("No se puede obtener perdedor de un partido sin ganador.");
        }

        if (ReferenceEquals(ganador, partido.EquipoLocal))
        {
            return partido.EquipoVisitante!;
        }

        return partido.EquipoLocal!;
    }

    public Partido GenerarFinal(
        List<Partido> semifinales,
        List<Estadio> estadios,
        ConfiguracionTorneo configuracionTorneo,
        IReadOnlyList<Partido>? existentes = null)
    {
        ValidarFaseNoGenerada(existentes, FaseTorneo.Final, "la final");
        ValidarPartidosDeFaseAnterior(
            semifinales,
            new ContextoValidacionFase
            {
                CantidadEsperada = CantidadPartidosSemifinales,
                FaseEsperada = FaseTorneo.Semifinal,
                FaseAGenerar = "la final",
                FaseAnterior = "semifinales"
            });

        var ganadores = ObtenerGanadores(semifinales);

        var fechaFinal = CalcularFechaPartidoEliminatorio(
            semifinales.Max(p => p.Fecha.Date).AddDays(configuracionTorneo.SeparacionEntreFechas * MultiplicadorSeparacionFinal),
            0,
            configuracionTorneo);

        return CrearPartidoEliminatorio(new DatosPartidoEliminatorio
        {
            Id = semifinales.Max(p => p.Id) + OffsetIdFinal,
            EquipoLocal = ganadores[0],
            EquipoVisitante = ganadores[1],
            Estadio = estadios[0],
            Fecha = fechaFinal,
            Fase = FaseTorneo.Final
        });
    }
}
