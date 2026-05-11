using  WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Dominio.Excepciones;

namespace WorldCupPlanner.Servicios;

public class ServicioFixture
{
    private readonly ServicioEquipo _servicioEquipo;

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

    private const int EquipoUno = 0;
    private const int EquipoDos = 1;
    private const int EquipoTres = 2;
    private const int EquipoCuatro = 3;

    private const char PrimeraEtiquetaGrupo = 'A';
    private const char UltimaEtiquetaGrupo = 'L';

    public ServicioFixture(ServicioEquipo servicioEquipo)
    {
        _servicioEquipo = servicioEquipo;
    }

    public Fixture GenerarFixture(List<Estadio> estadios, ConfiguracionTorneo configuracionTorneo)
    {
        var equiposCompletos = CompletarEquipos(configuracionTorneo);

        ValidarCantidadEquipos(equiposCompletos);
        ValidarCantidadEstadios(estadios);

        for (int offset = 0; offset < 100; offset++)
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
                return CrearFixtureConDistribucionRoundRobin(equiposCompletos, estadios, config);
            }
            catch (EntidadInvalidaExcepcion) when (offset < 99)
            {
                // la semilla actual genera un ordenamiento inválido, se prueba la siguiente
            }
        }

        throw new EntidadInvalidaExcepcion(
            "No se pudo generar el fixture respetando las restricciones de confederación.");
    }

    public Fixture RegenerarFixture(
        Fixture fixtureActual,
        List<Estadio> estadios,
        ConfiguracionTorneo configuracionTorneo)
    {
        ValidarFixtureSinResultados(fixtureActual);

        return GenerarFixture(estadios, configuracionTorneo);
    }

    public List<Equipo> CompletarEquipos(ConfiguracionTorneo configuracionTorneo)
    {
        _servicioEquipo.CompletarEquipos(configuracionTorneo.SemillaCompletar);
        return _servicioEquipo.Buscar(_ => true);
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
            throw new EntidadInvalidaExcepcion("No se puede generar el fixture con menos de 48 equipos.");
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

        for (int i = 0; i < equiposOrdenados.Count; i++)
        {
            AsignarEquipoAUnGrupoValido(fixture, equiposOrdenados[i], i);
        }

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

    private void AsignarEquipoAUnGrupoValido(Fixture fixture, Equipo equipo, int indiceEquipo)
    {
        for (int intento = 0; intento < CantidadGrupos; intento++)
        {
            var grupo = ObtenerGrupoCandidato(fixture, indiceEquipo, intento);

            if (PuedeAsignarseEquipoAGrupo(equipo, grupo))
            {
                grupo.Equipos.Add(equipo);
                return;
            }
        }

        throw new EntidadInvalidaExcepcion(
            "No se pudo asignar el equipo a ningún grupo respetando las restricciones de confederación.");
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

        var estadiosOrdenados = OrdenarEstadiosPorNombreNormalizado(estadios);
        var indicePartidoGlobal = 0;
        var cantidadPartidosPorDia = new Dictionary<DateTime, int>();

        foreach (var grupo in fixture.Grupos)
        {
            GenerarPartidosDelGrupo(
                grupo,
                configuracionTorneo,
                estadiosOrdenados,
                cantidadPartidosPorDia,
                ref indicePartidoGlobal);
        }
    }

    private void GenerarPartidosDelGrupo(
        Grupo grupo,
        ConfiguracionTorneo configuracionTorneo,
        List<Estadio> estadiosOrdenados,
        Dictionary<DateTime, int> cantidadPartidosPorDia,
        ref int indicePartidoGlobal)
    {
        var equipos = grupo.Equipos;
        var fechaBaseGrupo = ObtenerFechaBaseDisponibleParaGrupo(configuracionTorneo, cantidadPartidosPorDia);

        var (fechaJornada1Partido1, fechaJornada1Partido2) = ObtenerFechasJornada1(configuracionTorneo, fechaBaseGrupo);
        var (fechaJornada2Partido1, fechaJornada2Partido2) = ObtenerFechasJornada2(configuracionTorneo, fechaBaseGrupo);
        var fechaJornada3 = ObtenerFechasJornada3(configuracionTorneo, fechaBaseGrupo);

        AgregarPartidoDeGrupo(grupo, equipos, EquipoUno, EquipoCuatro, fechaJornada1Partido1, estadiosOrdenados, cantidadPartidosPorDia, ref indicePartidoGlobal);
        AgregarPartidoDeGrupo(grupo, equipos, EquipoDos, EquipoTres, fechaJornada1Partido2, estadiosOrdenados, cantidadPartidosPorDia, ref indicePartidoGlobal);

        AgregarPartidoDeGrupo(grupo, equipos, EquipoUno, EquipoTres, fechaJornada2Partido1, estadiosOrdenados, cantidadPartidosPorDia, ref indicePartidoGlobal);
        AgregarPartidoDeGrupo(grupo, equipos, EquipoDos, EquipoCuatro, fechaJornada2Partido2, estadiosOrdenados, cantidadPartidosPorDia, ref indicePartidoGlobal);

        AgregarPartidoDeGrupo(grupo, equipos, EquipoUno, EquipoDos, fechaJornada3, estadiosOrdenados, cantidadPartidosPorDia, ref indicePartidoGlobal);
        AgregarPartidoDeGrupo(grupo, equipos, EquipoTres, EquipoCuatro, fechaJornada3, estadiosOrdenados, cantidadPartidosPorDia, ref indicePartidoGlobal);
    }

    private void AgregarPartidoDeGrupo(
        Grupo grupo,
        List<Equipo> equipos,
        int indiceEquipoLocal,
        int indiceEquipoVisitante,
        DateTime fecha,
        List<Estadio> estadiosOrdenados,
        Dictionary<DateTime, int> cantidadPartidosPorDia,
        ref int indicePartidoGlobal)
    {
        var estadio = ObtenerEstadioParaPartido(estadiosOrdenados, indicePartidoGlobal);
        int idPartido = ObtenerSiguienteIdPartido(indicePartidoGlobal);
        var partido = CrearPartidoDeGrupoConId(
            grupo,
            equipos[indiceEquipoLocal],
            equipos[indiceEquipoVisitante],
            fecha,
            estadio,
            idPartido
        );

        grupo.Partidos.Add(partido);
        RegistrarPartidoEnDia(cantidadPartidosPorDia, fecha);
        indicePartidoGlobal++;
    }

    private void ValidarMaxPartidosPorDia(ConfiguracionTorneo configuracionTorneo)
    {
        if (configuracionTorneo.MaxPartidosPorDia < CantidadPartidosPorJornadaDeGrupo)
        {
            throw new EntidadInvalidaExcepcion(
                "El máximo de partidos por día debe permitir al menos una jornada completa de grupo.");
        }
    }

    private DateTime ObtenerFechaBaseDisponibleParaGrupo(
        ConfiguracionTorneo configuracionTorneo,
        Dictionary<DateTime, int> cantidadPartidosPorDia)
    {
        var fechaBase = configuracionTorneo.FechaInicioTorneo.Date;

        while (!HayCupoParaJornadasDelGrupo(fechaBase, configuracionTorneo, cantidadPartidosPorDia))
        {
            fechaBase = fechaBase.AddDays(1);
        }

        return fechaBase;
    }

    private bool HayCupoParaJornadasDelGrupo(
        DateTime fechaBase,
        ConfiguracionTorneo configuracionTorneo,
        Dictionary<DateTime, int> cantidadPartidosPorDia)
    {
        for (int indiceJornada = 0; indiceJornada < CantidadJornadasPorGrupo; indiceJornada++)
        {
            var fechaJornada = ObtenerFechaDeJornadaDelGrupo(fechaBase, configuracionTorneo, indiceJornada).Date;

            if (ObtenerCantidadPartidosDelDia(cantidadPartidosPorDia, fechaJornada)
                + CantidadPartidosPorJornadaDeGrupo > configuracionTorneo.MaxPartidosPorDia)
            {
                return false;
            }
        }

        return true;
    }

    private int ObtenerCantidadPartidosDelDia(
        Dictionary<DateTime, int> cantidadPartidosPorDia,
        DateTime fecha)
    {
        return cantidadPartidosPorDia.TryGetValue(fecha.Date, out var cantidad)
            ? cantidad
            : 0;
    }

    private void RegistrarPartidoEnDia(
        Dictionary<DateTime, int> cantidadPartidosPorDia,
        DateTime fecha)
    {
        var dia = fecha.Date;
        cantidadPartidosPorDia[dia] = ObtenerCantidadPartidosDelDia(cantidadPartidosPorDia, dia) + 1;
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

    private Estadio ObtenerEstadioParaPartido(List<Estadio> estadiosOrdenados, int indicePartidoGlobal)
    {
        return estadiosOrdenados[indicePartidoGlobal % estadiosOrdenados.Count];
    }

    private int ObtenerSiguienteIdPartido(int indicePartidoGlobal)
    {
        return indicePartidoGlobal + 1;
    }

    private Partido CrearPartidoDeGrupoConId(
        Grupo grupo,
        Equipo local,
        Equipo visitante,
        DateTime fecha,
        Estadio estadio,
        int id)
    {
        return new Partido
        {
            Id = id,
            Fecha = fecha,
            Estadio = estadio,
            EquipoLocal = local,
            EquipoVisitante = visitante,
            Grupo = grupo,
            Fase = FaseTorneo.FaseDeGrupos
        };
    }
}
