namespace WorldCupPlanner.Servicios;

using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;

public class EstadoTorneo
{

    public Fixture? Fixture { get; set; }
    public List<Partido> PartidosEliminatorios { get; set; } = new();
    public bool TorneoFinalizado { get; set; }
    public Fixture? FixtureAnterior { get; set; }
    public List<Partido> PartidosEliminatoriosAnteriores { get; set; } = new();

    public void ArchivarTorneoActual()
    {
        FixtureAnterior = Fixture;
        PartidosEliminatoriosAnteriores = PartidosEliminatorios;
        TorneoFinalizado = true;
        Fixture = null;
        PartidosEliminatorios = new();
    }

    public void EstablecerFixture(Fixture fixture)
    {
        Fixture = fixture;
        PartidosEliminatorios = new();
        TorneoFinalizado = false;
    }

    public void EstablecerPartidosEliminatorios(List<Partido> partidos)
    {
        PartidosEliminatorios = partidos;
    }

    public void AgregarPartidosEliminatorios(IEnumerable<Partido> partidos)
    {
        PartidosEliminatorios.AddRange(partidos);
    }

    public void ActualizarEstadoFinalizacion()
    {
        TorneoFinalizado = PartidosEliminatorios.Any(p => p.Fase == FaseTorneo.Final && p.EstaJugado());
    }

    public void ActualizarPartido(Partido actualizado)
    {
        var enGrupo = Fixture?.Grupos
            .SelectMany(g => g.Partidos)
            .FirstOrDefault(p => p.Id == actualizado.Id);

        if (enGrupo != null)
        {
            CopiarCamposPartido(actualizado, enGrupo);
            return;
        }

        var enEliminatorio = PartidosEliminatorios.FirstOrDefault(p => p.Id == actualizado.Id);
        if (enEliminatorio == null) return;

        CopiarCamposPartido(actualizado, enEliminatorio);
        TorneoFinalizado = PartidosEliminatorios.Any(p => p.Fase == FaseTorneo.Final && p.EstaJugado());
    }

    private static void CopiarCamposPartido(Partido fuente, Partido destino)
    {
        destino.Fecha             = fuente.Fecha;
        destino.Estadio           = fuente.Estadio;
        destino.GolesLocal        = fuente.GolesLocal;
        destino.GolesVisitante    = fuente.GolesVisitante;
        destino.GanadorPorPenales = fuente.GanadorPorPenales;
    }
}
