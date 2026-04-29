using WorldCupPlanner.Domain;

namespace WorldCupPlanner.Services;

public class ServicioFixture
{
    private const int CantidadMinimaEquipos = 48;
    private const int CantidadMinimaEstadios = 4;
    private const int CantidadGrupos = 12;
    private const int CantidadEquiposPorGrupo = 4;
    private const int MaximoEquiposUefaPorGrupo = 2;
    private const int MaximoEquiposOtrasConfederacionesPorGrupo = 1;

    private const int IndiceJornadaUno = 0;
    private const int IndiceJornadaDos = 1;
    private const int IndiceJornadaTres = 2;
    private const int CantidadJornadasPorGrupo = 3;

    private const int EquipoUno = 0;
    private const int EquipoDos = 1;
    private const int EquipoTres = 2;
    private const int EquipoCuatro = 3;

    private const char PrimeraEtiquetaGrupo = 'A';
    private const char UltimaEtiquetaGrupo = 'L';

    public Fixture GenerarFixture(List<Equipo> equipos, List<Estadio> estadios, ConfiguracionTorneo configuracionTorneo)
    {
        ValidarCantidadEquipos(equipos);
        ValidarCantidadEstadios(estadios);

        return CrearFixtureConDistribucionRoundRobin(equipos, estadios, configuracionTorneo);
    }

    private void ValidarCantidadEquipos(List<Equipo> equipos)
    {
        ValidarCantidadExactaDeEquipos(equipos);
        ValidarTodosLosEquiposSonValidos(equipos);
    }

    private void ValidarCantidadExactaDeEquipos(List<Equipo> equipos)
    {
        if (equipos.Count < CantidadMinimaEquipos)
        {
            throw new InvalidOperationException("No se puede generar el fixture con menos de 48 equipos.");
        }

        if (equipos.Count > CantidadMinimaEquipos)
        {
            throw new InvalidOperationException("No se puede generar el fixture con más de 48 equipos.");
        }
    }

    private void ValidarTodosLosEquiposSonValidos(List<Equipo> equipos)
    {
        if (equipos.Any(e => !e.EsValido()))
        {
            throw new InvalidOperationException("No se puede generar el fixture con equipos inválidos.");
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
            throw new InvalidOperationException("No se puede generar el fixture con menos de 4 estadios.");
        }
    }

    private void ValidarTodosLosEstadiosSonValidos(List<Estadio> estadios)
    {
        if (estadios.Any(e => !e.EsValido()))
        {
            throw new InvalidOperationException("No se puede generar el fixture con estadios inválidos.");
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
            var equiposMezclados = MezclarEquiposConFisherYates(grupoDeRanking.ToList(), random);
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
        for (int i = equipos.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (equipos[i], equipos[j]) = (equipos[j], equipos[i]);
        }

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

        throw new InvalidOperationException(
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
        var estadiosOrdenados = OrdenarEstadiosPorNombreNormalizado(estadios);
        var indicePartidoGlobal = 0;

        for (int indiceGrupo = 0; indiceGrupo < fixture.Grupos.Count; indiceGrupo++)
        {
            var grupo = fixture.Grupos[indiceGrupo];
            GenerarPartidosDelGrupo(grupo, configuracionTorneo, estadiosOrdenados, indiceGrupo, ref indicePartidoGlobal);
        }
    }

    private void GenerarPartidosDelGrupo(
        Grupo grupo,
        ConfiguracionTorneo configuracionTorneo,
        List<Estadio> estadiosOrdenados,
        int indiceGrupo,
        ref int indicePartidoGlobal)
    {
        var equipos = grupo.Equipos;
        var fechaBaseGrupo = ObtenerFechaBaseDelGrupo(configuracionTorneo, indiceGrupo);

        var (fechaJornada1Partido1, fechaJornada1Partido2) = ObtenerFechasJornada1(configuracionTorneo, fechaBaseGrupo);
        var (fechaJornada2Partido1, fechaJornada2Partido2) = ObtenerFechasJornada2(configuracionTorneo, fechaBaseGrupo);
        var fechaJornada3 = ObtenerFechasJornada3(configuracionTorneo, fechaBaseGrupo);

        AgregarPartidoDeGrupo(grupo, equipos, EquipoUno, EquipoCuatro, fechaJornada1Partido1, estadiosOrdenados, ref indicePartidoGlobal);
        AgregarPartidoDeGrupo(grupo, equipos, EquipoDos, EquipoTres, fechaJornada1Partido2, estadiosOrdenados, ref indicePartidoGlobal);

        AgregarPartidoDeGrupo(grupo, equipos, EquipoUno, EquipoTres, fechaJornada2Partido1, estadiosOrdenados, ref indicePartidoGlobal);
        AgregarPartidoDeGrupo(grupo, equipos, EquipoDos, EquipoCuatro, fechaJornada2Partido2, estadiosOrdenados, ref indicePartidoGlobal);

        AgregarPartidoDeGrupo(grupo, equipos, EquipoUno, EquipoDos, fechaJornada3, estadiosOrdenados, ref indicePartidoGlobal);
        AgregarPartidoDeGrupo(grupo, equipos, EquipoTres, EquipoCuatro, fechaJornada3, estadiosOrdenados, ref indicePartidoGlobal);
    }

    private void AgregarPartidoDeGrupo(
        Grupo grupo,
        List<Equipo> equipos,
        int indiceEquipoLocal,
        int indiceEquipoVisitante,
        DateTime fecha,
        List<Estadio> estadiosOrdenados,
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
        indicePartidoGlobal++;
    }

    private DateTime ObtenerFechaBaseDelGrupo(ConfiguracionTorneo configuracionTorneo, int indiceGrupo)
    {
        return configuracionTorneo.FechaInicioTorneo
            .AddDays(indiceGrupo * configuracionTorneo.SeparacionEntreFechas * CantidadJornadasPorGrupo);
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
        return estadios
            .OrderBy(e => Normalizar.NormalizarTexto(e.Nombre))
            .ToList();
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