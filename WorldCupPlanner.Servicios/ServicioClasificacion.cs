using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Excepciones;

namespace WorldCupPlanner.Servicios;

public class ServicioClasificacion
{
    public List<PosicionEquipo> CalcularTabla(Grupo grupo)
    {
        return CalcularTabla(grupo, new ConfiguracionTorneo());
    }

    public List<PosicionEquipo> CalcularTabla(Grupo grupo, ConfiguracionTorneo configuracionTorneo)
    {
        var posiciones = CrearPosicionesIniciales(grupo);

        foreach (var partido in grupo.Partidos)
        {
            if (partido.EstaJugado())
            {
                ActualizarPosicionesConPartido(posiciones, partido);
            }
        }

        return OrdenarPosicionesConDesempateDeterministico(
            new List<PosicionEquipo>(posiciones.Values),
            configuracionTorneo.SemillaCrucesFase
        );
    }

    public ClasificadosFaseGrupos ObtenerClasificadosAFaseEliminatoria(
        Fixture fixture,
        ConfiguracionTorneo configuracionTorneo)
    {
        ValidarFaseDeGruposCompleta(fixture);

        var tablas = CalcularTablasPorGrupo(fixture, configuracionTorneo);

        var primeros = OrdenarPosicionesConDesempateDeterministico(
            new List<PosicionEquipo>(tablas.Select(t => t[0])), configuracionTorneo.SemillaCrucesFase);

        var segundos = OrdenarPosicionesConDesempateDeterministico(
            new List<PosicionEquipo>(tablas.Select(t => t[1])), configuracionTorneo.SemillaCrucesFase);

        var mejoresTerceros = new List<PosicionEquipo>(
            OrdenarPosicionesConDesempateDeterministico(
                new List<PosicionEquipo>(tablas.Select(t => t[2])), configuracionTorneo.SemillaCrucesFase)
            .Take(8));

        return ConstruirClasificadosConGruposDeOrigen(primeros, segundos, mejoresTerceros);
    }

    private List<List<PosicionEquipo>> CalcularTablasPorGrupo(Fixture fixture, ConfiguracionTorneo configuracionTorneo)
    {
        return new List<List<PosicionEquipo>>(fixture.Grupos.Select(g => CalcularTabla(g, configuracionTorneo)));
    }

    private static ClasificadosFaseGrupos ConstruirClasificadosConGruposDeOrigen(
        List<PosicionEquipo> primeros,
        List<PosicionEquipo> segundos,
        List<PosicionEquipo> mejoresTerceros)
    {
        var clasificados = new ClasificadosFaseGrupos
        {
            Primeros = new List<Equipo>(primeros.Select(p => p.Equipo)),
            Segundos = new List<Equipo>(segundos.Select(p => p.Equipo)),
            MejoresTerceros = new List<Equipo>(mejoresTerceros.Select(p => p.Equipo))
        };

        foreach (var posicion in primeros.Concat(segundos).Concat(mejoresTerceros))
        {
            clasificados.GruposDeOrigen[posicion.Equipo] = posicion.Grupo;
        }

        return clasificados;
    }

    private static void ValidarFaseDeGruposCompleta(Fixture fixture)
    {
        if (fixture.Grupos.Any(g => g.Partidos.Any(p => !p.EstaJugado())))
        {
            throw new EntidadInvalidaExcepcion(
                "No se pueden obtener clasificados si hay partidos de fase de grupos sin resultado.");
        }
    }

    private static Dictionary<int, PosicionEquipo> CrearPosicionesIniciales(Grupo grupo)
    {
        var posiciones = new Dictionary<int, PosicionEquipo>();

        foreach (var equipo in grupo.Equipos)
        {
            posiciones[equipo.Id] = new PosicionEquipo
            {
                Equipo = equipo,
                Grupo = grupo
            };
        }

        return posiciones;
    }

    private static bool PartidoTieneEquipos(Partido partido) =>
        partido.EquipoLocal is not null && partido.EquipoVisitante is not null;

    private static void ActualizarPosicionesConPartido(Dictionary<int, PosicionEquipo> posiciones, Partido partido)
    {
        if (!PartidoTieneEquipos(partido)) return;

        if (!posiciones.TryGetValue(partido.EquipoLocal!.Id, out var posicionLocal) ||
            !posiciones.TryGetValue(partido.EquipoVisitante!.Id, out var posicionVisitante))
            return;

        int golesLocal = partido.GolesLocal!.Value;
        int golesVisitante = partido.GolesVisitante!.Value;

        posicionLocal.GolesAFavor += golesLocal;
        posicionLocal.GolesEnContra += golesVisitante;
        posicionLocal.PartidosJugados++;

        posicionVisitante.GolesAFavor += golesVisitante;
        posicionVisitante.GolesEnContra += golesLocal;
        posicionVisitante.PartidosJugados++;

        ActualizarPuntos(posicionLocal, posicionVisitante, partido);
    }

    private static void ActualizarPuntos(
        PosicionEquipo posicionLocal,
        PosicionEquipo posicionVisitante,
        Partido partido)
    {
        int golesLocal = partido.GolesLocal!.Value;
        int golesVisitante = partido.GolesVisitante!.Value;

        if (golesLocal > golesVisitante)
        {
            posicionLocal.Puntos += ConstantesTorneo.PuntosPorVictoria;
            posicionLocal.PartidosGanados++;
            posicionVisitante.PartidosPerdidos++;
        }
        else if (golesVisitante > golesLocal)
        {
            posicionVisitante.Puntos += ConstantesTorneo.PuntosPorVictoria;
            posicionVisitante.PartidosGanados++;
            posicionLocal.PartidosPerdidos++;
        }
        else
        {
            posicionLocal.Puntos += ConstantesTorneo.PuntosPorEmpate;
            posicionVisitante.Puntos += ConstantesTorneo.PuntosPorEmpate;
            posicionLocal.PartidosEmpatados++;
            posicionVisitante.PartidosEmpatados++;
        }
    }

    private static List<PosicionEquipo> AplicarDesempateDeterministico(
        List<PosicionEquipo> tablaOrdenada,
        int semillaCrucesFase)
    {
        var random = new Random(semillaCrucesFase);
        var tablaFinal = new List<PosicionEquipo>();

        var gruposEmpatados = tablaOrdenada
            .GroupBy(p => new
            {
                p.Puntos,
                p.DiferenciaGoles,
                p.GolesAFavor
            });

        foreach (var grupoEmpatado in gruposEmpatados)
        {
            var posiciones = new List<PosicionEquipo>(grupoEmpatado);

            if (posiciones.Count > 1)
            {
                MezclarConFisherYates(posiciones, random);
            }

            tablaFinal.AddRange(posiciones);
        }

        return tablaFinal;
    }

    private static void MezclarConFisherYates(List<PosicionEquipo> posiciones, Random random)
    {
        AlgoritmoFisherYates.Mezclar(posiciones, random);
    }

    private static List<PosicionEquipo> OrdenarPosicionesConDesempateDeterministico(
        List<PosicionEquipo> posiciones,
        int semillaCrucesFase)
    {
        var posicionesOrdenadas = new List<PosicionEquipo>(posiciones
            .OrderByDescending(p => p.Puntos)
            .ThenByDescending(p => p.DiferenciaGoles)
            .ThenByDescending(p => p.GolesAFavor));

        var resultado = AplicarDesempateDeterministico(posicionesOrdenadas, semillaCrucesFase);

        for (int i = 0; i < resultado.Count; i++)
            resultado[i].Posicion = i + 1;

        return resultado;
    }

    public Equipo? ObtenerCampeon(Partido? partidoFinal) =>
        partidoFinal?.EstaJugado() == true ? partidoFinal.Ganador() : null;
}
