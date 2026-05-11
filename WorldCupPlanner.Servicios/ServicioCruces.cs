using  WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Dominio.Excepciones;

namespace WorldCupPlanner.Servicios;

public class ServicioCruces
{
    private const int PrimerIdPrimeraRondaEliminatoria = 73;

    public List<Partido> GenerarPrimeraRondaEliminatoria(ContextoSorteo sorteo, Random? random = null)
    {
        var partidos = new List<Partido>();
        random ??= new Random(sorteo.ConfiguracionTorneo.SemillaCrucesFase);

        var primerosParaTerceros = MezclarEquiposConFisherYates(
            new List<Equipo>(sorteo.Clasificados.Primeros.Take(8)),
            random);

        var terceros = MezclarEquiposConFisherYates(
            new List<Equipo>(sorteo.Clasificados.MejoresTerceros),
            random);

        var contexto = new ContextoGeneracionCruces
        {
            Partidos = partidos,
            Clasificados = sorteo.Clasificados,
            Estadios = sorteo.Estadios,
            ConfiguracionTorneo = sorteo.ConfiguracionTorneo
        };

        AgregarCrucesPrimerosContraTerceros(primerosParaTerceros, terceros, contexto);

        var primerosRestantes = MezclarEquiposConFisherYates(
            new List<Equipo>(sorteo.Clasificados.Primeros.Skip(8).Take(4)),
            random);

        var segundosDeMenorRendimiento = MezclarEquiposConFisherYates(
            new List<Equipo>(sorteo.Clasificados.Segundos.Skip(8).Take(4)),
            random);

        AgregarCrucesPrimerosRestantesContraSegundosDeMenorRendimiento(primerosRestantes, segundosDeMenorRendimiento, contexto);

        var segundosRestantes = MezclarEquiposConFisherYates(
            new List<Equipo>(sorteo.Clasificados.Segundos.Take(8)),
            random);

        AgregarCrucesSegundosRestantesMinimos(segundosRestantes, contexto);

        return partidos;
    }

    private void AgregarCrucesPrimerosContraTerceros(
        List<Equipo> primeros,
        List<Equipo> terceros,
        ContextoGeneracionCruces contexto)
    {
        var cruceEquipos = new ContextoCruceEquipos
        {
            Locales = primeros,
            Visitantes = terceros,
            Clasificados = contexto.Clasificados
        };
        ReordenarVisitantesParaEvitarMismoGrupo(cruceEquipos);

        var fechaInicioDeFase = contexto.ConfiguracionTorneo.FechaInicioTorneo.Date
            .AddDays(contexto.ConfiguracionTorneo.SeparacionEntreFechas);

        for (int i = 0; i < primeros.Count; i++)
        {
            contexto.Partidos.Add(CrearPartidoEliminatorio(new DatosPartidoEliminatorio
            {
                Id = PrimerIdPrimeraRondaEliminatoria + contexto.Partidos.Count,
                EquipoLocal = primeros[i],
                EquipoVisitante = terceros[i],
                Estadio = contexto.Estadios[contexto.Partidos.Count % contexto.Estadios.Count],
                Fecha = CalcularFechaPartidoEliminatorio(fechaInicioDeFase, contexto.Partidos.Count, contexto.ConfiguracionTorneo),
                Fase = FaseTorneo.PrimeraRondaEliminatoria
            }));
        }
    }

    private void AgregarCrucesPrimerosRestantesContraSegundosDeMenorRendimiento(
        List<Equipo> primerosRestantes,
        List<Equipo> segundosDeMenorRendimiento,
        ContextoGeneracionCruces contexto)
    {
        var cruceEquipos = new ContextoCruceEquipos
        {
            Locales = primerosRestantes,
            Visitantes = segundosDeMenorRendimiento,
            Clasificados = contexto.Clasificados
        };
        ReordenarVisitantesParaEvitarMismoGrupo(cruceEquipos);

        var fechaInicioDeFase = contexto.ConfiguracionTorneo.FechaInicioTorneo.Date
            .AddDays(contexto.ConfiguracionTorneo.SeparacionEntreFechas);

        for (int i = 0; i < primerosRestantes.Count; i++)
        {
            contexto.Partidos.Add(CrearPartidoEliminatorio(new DatosPartidoEliminatorio
            {
                Id = PrimerIdPrimeraRondaEliminatoria + contexto.Partidos.Count,
                EquipoLocal = primerosRestantes[i],
                EquipoVisitante = segundosDeMenorRendimiento[i],
                Estadio = contexto.Estadios[contexto.Partidos.Count % contexto.Estadios.Count],
                Fecha = CalcularFechaPartidoEliminatorio(fechaInicioDeFase, contexto.Partidos.Count, contexto.ConfiguracionTorneo),
                Fase = FaseTorneo.PrimeraRondaEliminatoria
            }));
        }
    }

    private void AgregarCrucesSegundosRestantesMinimos(
        List<Equipo> segundosRestantes,
        ContextoGeneracionCruces contexto)
    {
        ReordenarEquiposParaEvitarCrucesInternosDelMismoGrupo(segundosRestantes, contexto.Clasificados);

        var fechaInicioDeFase = contexto.ConfiguracionTorneo.FechaInicioTorneo.Date
            .AddDays(contexto.ConfiguracionTorneo.SeparacionEntreFechas);

        for (int i = 0; i < segundosRestantes.Count; i += 2)
        {
            contexto.Partidos.Add(CrearPartidoEliminatorio(new DatosPartidoEliminatorio
            {
                Id = PrimerIdPrimeraRondaEliminatoria + contexto.Partidos.Count,
                EquipoLocal = segundosRestantes[i],
                EquipoVisitante = segundosRestantes[i + 1],
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
            if (i == indiceActual)
            {
                continue;
            }

            var evitaCruceActual = !PertenecenAlMismoGrupo(
                cruceEquipos.Locales[indiceActual],
                cruceEquipos.Visitantes[i],
                cruceEquipos.Clasificados);

            var noRompeCruceFuturo = !PertenecenAlMismoGrupo(
                cruceEquipos.Locales[i],
                cruceEquipos.Visitantes[indiceActual],
                cruceEquipos.Clasificados);

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
            new ContextoValidacionFase
            {
                CantidadEsperada = 16,
                FaseEsperada = FaseTorneo.PrimeraRondaEliminatoria,
                FaseAGenerar = "octavos",
                FaseAnterior = "primera ronda",
                FaseAnteriorEsperada = "primera ronda eliminatoria"
            });

        var ganadores = ObtenerGanadores(partidosPrimeraRonda);
        var partidos = new List<Partido>();
        var proximoId = partidosPrimeraRonda.Max(p => p.Id) + 1;
        var fechaInicio = partidosPrimeraRonda.Max(p => p.Fecha.Date).AddDays(configuracionTorneo.SeparacionEntreFechas);

        for (int i = 0; i < 8; i++)
        {
            partidos.Add(CrearPartidoEliminatorio(new DatosPartidoEliminatorio
            {
                Id = proximoId++,
                EquipoLocal = ganadores[i],
                EquipoVisitante = ganadores[i + 8],
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
                CantidadEsperada = 8,
                FaseEsperada = FaseTorneo.Octavos,
                FaseAGenerar = "cuartos",
                FaseAnterior = "octavos"
            });

        var ganadores = ObtenerGanadores(octavos);
        var partidos = new List<Partido>();
        var proximoId = octavos.Max(p => p.Id) + 1;
        var fechaInicio = octavos.Max(p => p.Fecha.Date).AddDays(configuracionTorneo.SeparacionEntreFechas);

        for (int i = 0; i < 4; i++)
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
                CantidadEsperada = 4,
                FaseEsperada = FaseTorneo.Cuartos,
                FaseAGenerar = "semifinales",
                FaseAnterior = "cuartos"
            });

        var ganadores = ObtenerGanadores(cuartos);
        var partidos = new List<Partido>();
        var proximoId = cuartos.Max(p => p.Id) + 1;
        var fechaInicio = cuartos.Max(p => p.Fecha.Date).AddDays(configuracionTorneo.SeparacionEntreFechas);

        for (int i = 0; i < 2; i++)
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
        ConfiguracionTorneo configuracionTorneo)
    {
        ValidarPartidosDeFaseAnterior(
            semifinales,
            new ContextoValidacionFase
            {
                CantidadEsperada = 2,
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
        ConfiguracionTorneo configuracionTorneo)
    {
        ValidarPartidosDeFaseAnterior(
            semifinales,
            new ContextoValidacionFase
            {
                CantidadEsperada = 2,
                FaseEsperada = FaseTorneo.Semifinal,
                FaseAGenerar = "la final",
                FaseAnterior = "semifinales"
            });

        var ganadores = ObtenerGanadores(semifinales);

        var fechaFinal = CalcularFechaPartidoEliminatorio(
            semifinales.Max(p => p.Fecha.Date).AddDays(configuracionTorneo.SeparacionEntreFechas * 2),
            0,
            configuracionTorneo);

        return CrearPartidoEliminatorio(new DatosPartidoEliminatorio
        {
            Id = semifinales.Max(p => p.Id) + 2,
            EquipoLocal = ganadores[0],
            EquipoVisitante = ganadores[1],
            Estadio = estadios[0],
            Fecha = fechaFinal,
            Fase = FaseTorneo.Final
        });
    }
}
