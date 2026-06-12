namespace WorldCupPlanner.Servicios;

using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;

public enum FaseConfiguracionTorneo { Incompleto, ListoParaFixture, FixtureGenerado }

public class EstadoTorneo
{
    private const int CantidadEquiposRequeridos = 48;

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

    public List<Partido> ObtenerTodosLosPartidos()
    {
        var partidos = new List<Partido>();
        if (Fixture != null)
            partidos.AddRange(Fixture.Partidos);
        partidos.AddRange(PartidosEliminatorios);
        return partidos;
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

    public IEnumerable<Equipo> ObtenerTodosLosEquipos()
    {
        if (Fixture != null)
        {
            foreach (var equipo in Fixture.Grupos.SelectMany(g => g.Equipos))
                yield return equipo;

            foreach (var equipo in ObtenerEquiposDePartidos(Fixture.Partidos))
                yield return equipo;
        }

        foreach (var equipo in ObtenerEquiposDePartidos(PartidosEliminatorios))
            yield return equipo;
    }

    private static void CopiarCamposPartido(Partido fuente, Partido destino)
    {
        destino.Fecha             = fuente.Fecha;
        destino.Estadio           = fuente.Estadio;
        destino.GolesLocal        = fuente.GolesLocal;
        destino.GolesVisitante    = fuente.GolesVisitante;
        destino.GanadorPorPenales = fuente.GanadorPorPenales;
    }

    private static IEnumerable<Equipo> ObtenerEquiposDePartidos(IEnumerable<Partido> partidos)
    {
        foreach (var partido in partidos)
        {
            if (partido.EquipoLocal       != null) yield return partido.EquipoLocal;
            if (partido.EquipoVisitante   != null) yield return partido.EquipoVisitante;
            if (partido.GanadorPorPenales != null) yield return partido.GanadorPorPenales;
        }
    }

    public FaseConfiguracionTorneo ObtenerFaseConfiguracion(int cantidadEquiposRegistrados)
    {
        if (Fixture != null) return FaseConfiguracionTorneo.FixtureGenerado;
        if (cantidadEquiposRegistrados >= CantidadEquiposRequeridos) return FaseConfiguracionTorneo.ListoParaFixture;
        return FaseConfiguracionTorneo.Incompleto;
    }

    public List<Partido> ObtenerPartidosDeFase(FaseTorneo fase) =>
        PartidosEliminatorios.Where(p => p.Fase == fase).ToList();

    public int ContarTotalPartidos() =>
        (Fixture?.Partidos.Count ?? 0) + PartidosEliminatorios.Count;

    public int ContarPartidosJugados() =>
        (Fixture?.Partidos.Count(p => p.EstaJugado()) ?? 0)
        + PartidosEliminatorios.Count(p => p.EstaJugado());

    public bool EquipoParticipaEnFixture(Equipo equipo)
    {
        if (Fixture == null)
            return false;

        return Fixture.Grupos
                   .SelectMany(g => g.Equipos)
                   .Any(e => EsMismoEquipo(e, equipo))
               || Fixture.Partidos
                   .Any(p => EsMismoEquipo(p.EquipoLocal, equipo)
                             || EsMismoEquipo(p.EquipoVisitante, equipo))
               || PartidosEliminatorios
                   .Any(p => EsMismoEquipo(p.EquipoLocal, equipo)
                             || EsMismoEquipo(p.EquipoVisitante, equipo));
    }

    public bool EstadioParticipaEnFixture(Estadio estadio)
    {
        if (Fixture == null)
            return false;

        return Fixture.Partidos
                   .Any(p => EsMismoEstadio(p.Estadio, estadio))
               || PartidosEliminatorios
                   .Any(p => EsMismoEstadio(p.Estadio, estadio));
    }

    private static bool EsMismoEquipo(Equipo? participante, Equipo equipo)
    {
        if (participante == null)
            return false;

        if (participante.Id > 0 && equipo.Id > 0)
            return participante.Id == equipo.Id;

        return string.Equals(participante.Nombre, equipo.Nombre, StringComparison.OrdinalIgnoreCase);
    }

    private static bool EsMismoEstadio(Estadio? estadioEnFixture, Estadio estadio)
    {
        if (estadioEnFixture == null)
            return false;

        if (estadioEnFixture.Id > 0 && estadio.Id > 0)
            return estadioEnFixture.Id == estadio.Id;

        return string.Equals(estadioEnFixture.Nombre, estadio.Nombre, StringComparison.OrdinalIgnoreCase);
    }
}