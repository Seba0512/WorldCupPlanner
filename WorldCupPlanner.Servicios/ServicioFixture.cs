using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Dominio.Excepciones;

namespace WorldCupPlanner.Servicios;

public class ServicioFixture
{
    private const int CantidadExactaEquipos = 48;
    private const int CantidadMinimaEstadios = 4;
    private const int CantidadGrupos = 12;
    private const int CantidadEquiposPorGrupo = 4;
    private const int MaximoEquiposUefaPorGrupo = 2;
    private const int MaximoEquiposOtrasConfederacionesPorGrupo = 1;

    private const int IndiceJornadaUno = 0;
    private const int IndiceJornadaDos = 1;
    private const int IndiceJornadaTres = 2;
    private const int CantidadJornadasPorGrupo = 3;
    private const int CantidadPartidosPorJornadaDeGrupo = 2;

    private const char PrimeraEtiquetaGrupo = 'A';
    private const char UltimaEtiquetaGrupo = 'L';

    private const int MaxIntentosGeneracionFixture = 100;

    public Fixture GenerarFixture(List<Equipo> equipos, List<Estadio> estadios, ConfiguracionTorneo configuracionTorneo)
    {
        ValidarCantidadEquipos(equipos);
        ValidarCantidadEstadios(estadios);
        ValidarMaxPartidosPorDia(configuracionTorneo);

        for (int offset = 0; offset < MaxIntentosGeneracionFixture; offset++)
        {
            try
            {
                var config = new ConfiguracionTorneo
                {
                    SemillaFixture = configuracionTorneo.SemillaFixture + offset,
                    SemillaCompletar = configuracionTorneo.SemillaCompletar,
                    SemillaCrucesFase = configuracionTorneo.SemillaCrucesFase,
                    SemillaSimulation = configuracionTorneo.SemillaSimulation,
                    FechaInicioTorneo = configuracionTorneo.FechaInicioTorneo,
                    MaxPartidosPorDia = configuracionTorneo.MaxPartidosPorDia,
                    SeparacionEntreFechas = configuracionTorneo.SeparacionEntreFechas,
                    HoraInicioPartidos = configuracionTorneo.HoraInicioPartidos,
                    SeparacionHorasEntrePartidos = configuracionTorneo.SeparacionHorasEntrePartidos,
                };
                return CrearFixtureConDistribucionRoundRobin(equipos, estadios, config);
            }
            catch (EntidadInvalidaExcepcion)
            {
                // la semilla actual genera un ordenamiento inválido, se prueba la siguiente
            }
        }

        throw new EntidadInvalidaExcepcion(
            "No se pudo generar el fixture respetando las restricciones de confederación.");
    }

    public Fixture RegenerarFixture(
        List<Equipo> equipos,
        Fixture fixtureActual,
        List<Estadio> estadios,
        ConfiguracionTorneo configuracionTorneo)
    {
        ValidarFixtureSinResultados(fixtureActual);

        return GenerarFixture(equipos, estadios, configuracionTorneo);
    }

    private void ValidarFixtureSinResultados(Fixture fixture)
    {
        if (fixture.Partidos.Any(p => p.EstaJugado()))
        {
            throw new EntidadInvalidaExcepcion(
                "No se puede regenerar el fixture si ya hay resultados cargados.");
        }
    }

    private void ValidarCantidadEquipos(List<Equipo> equipos)
    {
        ValidarCantidadExactaDeEquipos(equipos);
        ValidarTodosLosEquiposSonValidos(equipos);
    }

    private void ValidarCantidadExactaDeEquipos(List<Equipo> equipos)
    {
        if (equipos.Count < CantidadExactaEquipos)
        {
            int faltantes = CantidadExactaEquipos - equipos.Count;
            throw new EntidadInvalidaExcepcion(
                $"No es posible generar el fixture. Faltan {faltantes} equipo(s) por registrar o completar.");
        }

        if (equipos.Count > CantidadExactaEquipos)
        {
            throw new EntidadInvalidaExcepcion("No se puede generar el fixture con más de 48 equipos.");
        }
    }

    private void ValidarTodosLosEquiposSonValidos(List<Equipo> equipos)
    {
        if (equipos.Any(e => !e.EsValido()))
        {
            throw new EntidadInvalidaExcepcion("No se puede generar el fixture con equipos inválidos.");
        }
    }

    private void ValidarCantidadEstadios(List<Estadio> estadios)
    {
        ValidarCantidadMinimaDeEstadios(estadios);
        ValidarTodosLosEstadiosSonValidos(estadios);
    }

    private void ValidarCantidadMinimaDeEstadios(List<Estadio> estadios)
    {
        if (estadios.Count < CantidadMinimaEstadios)
        {
            throw new EntidadInvalidaExcepcion("No se puede generar el fixture con menos de 4 estadios.");
        }
    }

    private void ValidarTodosLosEstadiosSonValidos(List<Estadio> estadios)
    {
        if (estadios.Any(e => !e.EsValido()))
        {
            throw new EntidadInvalidaExcepcion("No se puede generar el fixture con estadios inválidos.");
        }
    }

    private Fixture CrearFixtureConDistribucionRoundRobin(
        List<Equipo> equipos,
        List<Estadio> estadios,
        ConfiguracionTorneo configuracionTorneo)
    {
        var fixture = CrearFixtureVacioConGrupos();
        var equiposOrdenados = OrdenarEquiposParaFixture(equipos, configuracionTorneo.SemillaFixture);

        AsignarEquiposAGrupos(fixture, equiposOrdenados);

        GenerarPartidosDeGrupos(fixture, configuracionTorneo, estadios);

        return fixture;
    }

    private List<Equipo> OrdenarEquiposParaFixture(List<Equipo> equipos, int semillaFixture)
    {
        var random = new Random(semillaFixture);
        var equiposOrdenados = new List<Equipo>();

        foreach (var grupoDeRanking in ObtenerGruposPorRankingDescendente(equipos))
        {
            var equiposMezclados = MezclarEquiposConFisherYates(new List<Equipo>(grupoDeRanking), random);
            equiposOrdenados.AddRange(equiposMezclados);
        }

        return equiposOrdenados;
    }

    private IEnumerable<IGrouping<int, Equipo>> ObtenerGruposPorRankingDescendente(List<Equipo> equipos)
    {
        return equipos
            .GroupBy(e => e.RankingFIFA)
            .OrderByDescending(g => g.Key);
    }

    private List<Equipo> MezclarEquiposConFisherYates(List<Equipo> equipos, Random random)
    {
        AlgoritmoFisherYates.Mezclar(equipos, random);
        return equipos;
    }

    private Fixture CrearFixtureVacioConGrupos()
    {
        var fixture = new Fixture();

        for (char etiqueta = PrimeraEtiquetaGrupo; etiqueta <= UltimaEtiquetaGrupo; etiqueta++)
        {
            fixture.Grupos.Add(new Grupo(etiqueta.ToString()));
        }

        return fixture;
    }

    private void AsignarEquiposAGrupos(Fixture fixture, List<Equipo> equiposOrdenados)
    {
        if (IntentarAsignarEquiposAGrupos(fixture, equiposOrdenados, 0))
        {
            return;
        }

        throw new EntidadInvalidaExcepcion(
            "No se pudo asignar los equipos a grupos respetando las restricciones de confederación. " +
            $"Estado de grupos: {DescribirEstadoGrupos(fixture)}");
    }

    private bool IntentarAsignarEquiposAGrupos(
        Fixture fixture,
        List<Equipo> equiposOrdenados,
        int indiceEquipo)
    {
        if (indiceEquipo == equiposOrdenados.Count)
        {
            return true;
        }

        var equipo = equiposOrdenados[indiceEquipo];

        for (int intento = 0; intento < CantidadGrupos; intento++)
        {
            var grupo = ObtenerGrupoCandidato(fixture, indiceEquipo, intento);

            if (PuedeAsignarseEquipoAGrupo(equipo, grupo))
            {
                grupo.Equipos.Add(equipo);

                if (IntentarAsignarEquiposAGrupos(fixture, equiposOrdenados, indiceEquipo + 1))
                {
                    return true;
                }

                grupo.Equipos.Remove(equipo);
            }
        }

        return false;
    }

    private string DescribirEstadoGrupos(Fixture fixture)
    {
        return string.Join("; ", fixture.Grupos.Select(g =>
            $"{g.Etiqueta}:[{string.Join(",", g.Equipos.Select(e => e.Confederacion))}]"));
    }

    private Grupo ObtenerGrupoCandidato(Fixture fixture, int indiceEquipo, int intento)
    {
        int indiceGrupo = (indiceEquipo + intento) % CantidadGrupos;
        return fixture.Grupos[indiceGrupo];
    }

    private bool PuedeAsignarseEquipoAGrupo(Equipo equipo, Grupo grupo)
    {
        if (grupo.Equipos.Count >= CantidadEquiposPorGrupo)
        {
            return false;
        }

        if (!equipo.EsValido())
        {
            return false;
        }

        return CumpleRestriccionDeConfederacion(equipo, grupo);
    }

    private bool CumpleRestriccionDeConfederacion(Equipo equipo, Grupo grupo)
    {
        var confederacion = equipo.Confederacion!.Value;
        int cantidadMismaConfederacion = grupo.Equipos.Count(e => e.Confederacion == confederacion);

        if (confederacion == Confederacion.UEFA)
        {
            return cantidadMismaConfederacion < MaximoEquiposUefaPorGrupo;
        }

        return cantidadMismaConfederacion < MaximoEquiposOtrasConfederacionesPorGrupo;
    }

    private void GenerarPartidosDeGrupos(
        Fixture fixture,
        ConfiguracionTorneo configuracionTorneo,
        List<Estadio> estadios)
    {
        ValidarMaxPartidosPorDia(configuracionTorneo);

        var contexto = new ContextoGeneracionFixture
        {
            EstadiosOrdenados = OrdenarEstadiosPorNombreNormalizado(estadios),
            ConfiguracionTorneo = configuracionTorneo
        };

        foreach (var grupo in fixture.Grupos)
        {
            GenerarPartidosDelGrupo(grupo, contexto);
        }
    }

    private void GenerarPartidosDelGrupo(Grupo grupo, ContextoGeneracionFixture contexto)
    {
        var fechaBase = ObtenerFechaBaseDisponibleParaGrupo(contexto);
        AgregarPartidosJornada1(grupo, fechaBase, contexto);
        AgregarPartidosJornada2(grupo, fechaBase, contexto);
        AgregarPartidosJornada3(grupo, fechaBase, contexto);
    }

    private void AgregarPartidosJornada1(Grupo grupo, DateTime fechaBase, ContextoGeneracionFixture contexto)
    {
        var (partido1, partido2) = ObtenerFechasJornada1(contexto.ConfiguracionTorneo, fechaBase);
        AgregarPartidoDeGrupo(grupo, new ContextoCruceGrupo { PosicionEquipoLocal = PosicionEnGrupo.Primera, PosicionEquipoVisitante = PosicionEnGrupo.Cuarta, Fecha = partido1 }, contexto);
        AgregarPartidoDeGrupo(grupo, new ContextoCruceGrupo { PosicionEquipoLocal = PosicionEnGrupo.Segunda, PosicionEquipoVisitante = PosicionEnGrupo.Tercera, Fecha = partido2 }, contexto);
    }

    private void AgregarPartidosJornada2(Grupo grupo, DateTime fechaBase, ContextoGeneracionFixture contexto)
    {
        var (partido1, partido2) = ObtenerFechasJornada2(contexto.ConfiguracionTorneo, fechaBase);
        AgregarPartidoDeGrupo(grupo, new ContextoCruceGrupo { PosicionEquipoLocal = PosicionEnGrupo.Primera, PosicionEquipoVisitante = PosicionEnGrupo.Tercera, Fecha = partido1 }, contexto);
        AgregarPartidoDeGrupo(grupo, new ContextoCruceGrupo { PosicionEquipoLocal = PosicionEnGrupo.Segunda, PosicionEquipoVisitante = PosicionEnGrupo.Cuarta, Fecha = partido2 }, contexto);
    }

    private void AgregarPartidosJornada3(Grupo grupo, DateTime fechaBase, ContextoGeneracionFixture contexto)
    {
        var fechaJornada3 = ObtenerFechasJornada3(contexto.ConfiguracionTorneo, fechaBase);
        AgregarPartidoDeGrupo(grupo, new ContextoCruceGrupo { PosicionEquipoLocal = PosicionEnGrupo.Primera, PosicionEquipoVisitante = PosicionEnGrupo.Segunda, Fecha = fechaJornada3 }, contexto);
        AgregarPartidoDeGrupo(grupo, new ContextoCruceGrupo { PosicionEquipoLocal = PosicionEnGrupo.Tercera, PosicionEquipoVisitante = PosicionEnGrupo.Cuarta, Fecha = fechaJornada3 }, contexto);
    }

    private void AgregarPartidoDeGrupo(Grupo grupo, ContextoCruceGrupo cruce, ContextoGeneracionFixture contexto)
    {
        var datos = CrearDatosPartidoDeGrupo(grupo, cruce, contexto);
        grupo.Partidos.Add(CrearPartidoDeGrupoConId(datos));
        contexto.RegistrarPartido(cruce.Fecha);
    }

    private DatosPartidoDeGrupo CrearDatosPartidoDeGrupo(Grupo grupo, ContextoCruceGrupo cruce, ContextoGeneracionFixture contexto)
    {
        return new DatosPartidoDeGrupo
        {
            Grupo = grupo,
            EquipoLocal = grupo.Equipos[(int)cruce.PosicionEquipoLocal],
            EquipoVisitante = grupo.Equipos[(int)cruce.PosicionEquipoVisitante],
            Fecha = cruce.Fecha,
            Estadio = ObtenerEstadioParaPartido(contexto.EstadiosOrdenados, contexto.IndicePartidoGlobal),
            Id = ObtenerSiguienteIdPartido(contexto.IndicePartidoGlobal)
        };
    }

    private void ValidarMaxPartidosPorDia(ConfiguracionTorneo configuracionTorneo)
    {
        if (configuracionTorneo.MaxPartidosPorDia < CantidadPartidosPorJornadaDeGrupo)
        {
            throw new EntidadInvalidaExcepcion(
                "El máximo de partidos por día debe permitir al menos una jornada completa de grupo.");
        }
    }

    private DateTime ObtenerFechaBaseDisponibleParaGrupo(ContextoGeneracionFixture contexto)
    {
        var fechaBase = contexto.ConfiguracionTorneo.FechaInicioTorneo.Date;

        while (!HayCupoParaJornadasDelGrupo(fechaBase, contexto))
        {
            fechaBase = fechaBase.AddDays(1);
        }

        return fechaBase;
    }

    private bool HayCupoParaJornadasDelGrupo(DateTime fechaBase, ContextoGeneracionFixture contexto)
    {
        for (int indiceJornada = 0; indiceJornada < CantidadJornadasPorGrupo; indiceJornada++)
        {
            var fechaJornada = ObtenerFechaDeJornadaDelGrupo(fechaBase, contexto.ConfiguracionTorneo, indiceJornada).Date;

            if (contexto.ObtenerPartidosDelDia(fechaJornada)
                + CantidadPartidosPorJornadaDeGrupo > contexto.ConfiguracionTorneo.MaxPartidosPorDia)
            {
                return false;
            }
        }

        return true;
    }

    private DateTime ObtenerFechaDeJornadaDelGrupo(DateTime fechaBase, ConfiguracionTorneo configuracionTorneo, int indiceJornada)
    {
        return fechaBase.Date
            .Add(configuracionTorneo.HoraInicioPartidos)
            .AddDays(configuracionTorneo.SeparacionEntreFechas * indiceJornada);
    }


    private DateTime ObtenerFechaSegundoPartidoDeJornada(ConfiguracionTorneo configuracionTorneo, DateTime fechaPrimerPartido)
    {
        return fechaPrimerPartido.AddHours(configuracionTorneo.SeparacionHorasEntrePartidos);
    }

    private (DateTime Partido1, DateTime Partido2) ObtenerFechasJornada1(ConfiguracionTorneo configuracionTorneo, DateTime fechaBaseGrupo)
    {
        var fechaPartido1 = ObtenerFechaDeJornadaDelGrupo(fechaBaseGrupo, configuracionTorneo, IndiceJornadaUno);
        var fechaPartido2 = ObtenerFechaSegundoPartidoDeJornada(configuracionTorneo, fechaPartido1);
        return (fechaPartido1, fechaPartido2);
    }

    private (DateTime Partido1, DateTime Partido2) ObtenerFechasJornada2(ConfiguracionTorneo configuracionTorneo, DateTime fechaBaseGrupo)
    {
        var fechaPartido1 = ObtenerFechaDeJornadaDelGrupo(fechaBaseGrupo, configuracionTorneo, IndiceJornadaDos);
        var fechaPartido2 = ObtenerFechaSegundoPartidoDeJornada(configuracionTorneo, fechaPartido1);
        return (fechaPartido1, fechaPartido2);
    }

    private DateTime ObtenerFechasJornada3(ConfiguracionTorneo configuracionTorneo, DateTime fechaBaseGrupo)
    {
        return ObtenerFechaDeJornadaDelGrupo(fechaBaseGrupo, configuracionTorneo, IndiceJornadaTres);
    }

    private List<Estadio> OrdenarEstadiosPorNombreNormalizado(List<Estadio> estadios)
    {
        return new List<Estadio>(estadios
            .OrderBy(e => Normalizar.NormalizarTexto(e.Nombre))
        );
    }

    private Estadio ObtenerEstadioParaPartido(IReadOnlyList<Estadio> estadiosOrdenados, int indicePartidoGlobal)
    {
        return estadiosOrdenados[indicePartidoGlobal % estadiosOrdenados.Count];
    }

    private int ObtenerSiguienteIdPartido(int indicePartidoGlobal)
    {
        return indicePartidoGlobal + 1;
    }

    private Partido CrearPartidoDeGrupoConId(DatosPartidoDeGrupo datos)
    {
        return new Partido
        {
            Id = datos.Id,
            Fecha = datos.Fecha,
            Estadio = datos.Estadio,
            EquipoLocal = datos.EquipoLocal,
            EquipoVisitante = datos.EquipoVisitante,
            Grupo = datos.Grupo,
            Fase = FaseTorneo.FaseDeGrupos
        };
    }
}
