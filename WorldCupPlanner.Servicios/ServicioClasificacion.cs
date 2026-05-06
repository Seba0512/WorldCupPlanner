using  WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.Servicios;

public class ServicioClasificacion
{
    private const int PuntosPorVictoria = 3;
    private const int PuntosPorEmpate = 1;

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
            posiciones.Values.ToList(),
            configuracionTorneo.SemillaCrucesFase
        );
    }

    private Dictionary<Equipo, PosicionEquipo> CrearPosicionesIniciales(Grupo grupo)
    {
        var posiciones = new Dictionary<Equipo, PosicionEquipo>();

        foreach (var equipo in grupo.Equipos)
        {
            posiciones[equipo] = new PosicionEquipo
            {
                Equipo = equipo,
                Grupo = grupo
            };
        }

        return posiciones;
    }

    private static bool PartidoTieneEquipos(Partido partido) =>
        partido.EquipoLocal is not null && partido.EquipoVisitante is not null;

    private void ActualizarPosicionesConPartido(Dictionary<Equipo, PosicionEquipo> posiciones, Partido partido)
    {
        if (!PartidoTieneEquipos(partido)) return;

        var posicionLocal = posiciones[partido.EquipoLocal!];
        var posicionVisitante = posiciones[partido.EquipoVisitante!];

        int golesLocal = partido.GolesLocal!.Value;
        int golesVisitante = partido.GolesVisitante!.Value;

        posicionLocal.GolesAFavor += golesLocal;
        posicionLocal.GolesEnContra += golesVisitante;

        posicionVisitante.GolesAFavor += golesVisitante;
        posicionVisitante.GolesEnContra += golesLocal;

        ActualizarPuntos(posicionLocal, posicionVisitante, golesLocal, golesVisitante);
    }

    private void ActualizarPuntos(
        PosicionEquipo posicionLocal,
        PosicionEquipo posicionVisitante,
        int golesLocal,
        int golesVisitante)
    {
        if (golesLocal > golesVisitante)
        {
            posicionLocal.Puntos += PuntosPorVictoria;
        }
        else if (golesVisitante > golesLocal)
        {
            posicionVisitante.Puntos += PuntosPorVictoria;
        }
        else
        {
            posicionLocal.Puntos += PuntosPorEmpate;
            posicionVisitante.Puntos += PuntosPorEmpate;
        }
    }

    private List<PosicionEquipo> AplicarDesempateDeterministico(
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
            var posiciones = grupoEmpatado.ToList();

            if (posiciones.Count > 1)
            {
                posiciones = MezclarConFisherYates(posiciones, random);
            }

            tablaFinal.AddRange(posiciones);
        }

        return tablaFinal;
    }

    private List<PosicionEquipo> MezclarConFisherYates(List<PosicionEquipo> posiciones, Random random)
    {
        for (int i = posiciones.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (posiciones[i], posiciones[j]) = (posiciones[j], posiciones[i]);
        }

        return posiciones;
    }
    
    public ClasificadosFaseGrupos ObtenerClasificadosAFaseEliminatoria(
        Fixture fixture,
        ConfiguracionTorneo configuracionTorneo)
    {
        var primeros = new List<PosicionEquipo>();
        var segundos = new List<PosicionEquipo>();
        var terceros = new List<PosicionEquipo>();

        foreach (var grupo in fixture.Grupos)
        {
            var tabla = CalcularTabla(grupo, configuracionTorneo);

            primeros.Add(tabla[0]);
            segundos.Add(tabla[1]);
            terceros.Add(tabla[2]);
        }

        var primerosOrdenados = OrdenarPosicionesConDesempateDeterministico(
            primeros,
            configuracionTorneo.SemillaCrucesFase
        );

        var segundosOrdenados = OrdenarPosicionesConDesempateDeterministico(
            segundos,
            configuracionTorneo.SemillaCrucesFase
        );

        var mejoresTerceros = OrdenarPosicionesConDesempateDeterministico(
                terceros,
                configuracionTorneo.SemillaCrucesFase
            )
            .Take(8)
            .ToList();

        var clasificados = new ClasificadosFaseGrupos
        {
            Primeros = primerosOrdenados.Select(p => p.Equipo).ToList(),
            Segundos = segundosOrdenados.Select(p => p.Equipo).ToList(),
            MejoresTerceros = mejoresTerceros.Select(p => p.Equipo).ToList()
        };

        foreach (var posicion in primerosOrdenados.Concat(segundosOrdenados).Concat(mejoresTerceros))
        {
            clasificados.GruposDeOrigen[posicion.Equipo] = posicion.Grupo;
        }

        return clasificados;
    }
    
    private List<PosicionEquipo> OrdenarPosicionesConDesempateDeterministico(
        List<PosicionEquipo> posiciones,
        int semillaCrucesFase)
    {
        var posicionesOrdenadas = posiciones
            .OrderByDescending(p => p.Puntos)
            .ThenByDescending(p => p.DiferenciaGoles)
            .ThenByDescending(p => p.GolesAFavor)
            .ToList();

        return AplicarDesempateDeterministico(posicionesOrdenadas, semillaCrucesFase);
    }
}
