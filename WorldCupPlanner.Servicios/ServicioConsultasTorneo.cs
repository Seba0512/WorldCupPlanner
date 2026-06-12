using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.Servicios;

public class ServicioConsultasTorneo
{
    public List<Partido> ObtenerTodosLosPartidos(EstadoTorneo estado)
    {
        var partidos = new List<Partido>();
        if (estado.Fixture != null)
            partidos.AddRange(estado.Fixture.Partidos);
        partidos.AddRange(estado.PartidosEliminatorios);
        return partidos;
    }

    public IEnumerable<Equipo> ObtenerTodosLosEquipos(EstadoTorneo estado)
    {
        if (estado.Fixture != null)
        {
            foreach (var equipo in estado.Fixture.Grupos.SelectMany(g => g.Equipos))
                yield return equipo;

            foreach (var equipo in ObtenerEquiposDePartidos(estado.Fixture.Partidos))
                yield return equipo;
        }

        foreach (var equipo in ObtenerEquiposDePartidos(estado.PartidosEliminatorios))
            yield return equipo;
    }

    public List<Partido> ObtenerPartidosDeFase(EstadoTorneo estado, FaseTorneo fase) =>
        estado.PartidosEliminatorios.Where(p => p.Fase == fase).ToList();

    public int ContarTotalPartidos(EstadoTorneo estado) =>
        (estado.Fixture?.Partidos.Count ?? 0) + estado.PartidosEliminatorios.Count;

    public int ContarPartidosJugados(EstadoTorneo estado) =>
        (estado.Fixture?.Partidos.Count(p => p.EstaJugado()) ?? 0)
        + estado.PartidosEliminatorios.Count(p => p.EstaJugado());

    public bool EquipoParticipaEnFixture(EstadoTorneo estado, Equipo equipo)
    {
        if (estado.Fixture == null)
            return false;

        return estado.Fixture.Grupos
                   .SelectMany(g => g.Equipos)
                   .Any(e => EsMismoEquipo(e, equipo))
               || estado.Fixture.Partidos
                   .Any(p => EsMismoEquipo(p.EquipoLocal, equipo)
                             || EsMismoEquipo(p.EquipoVisitante, equipo))
               || estado.PartidosEliminatorios
                   .Any(p => EsMismoEquipo(p.EquipoLocal, equipo)
                             || EsMismoEquipo(p.EquipoVisitante, equipo));
    }

    public bool EstadioParticipaEnFixture(EstadoTorneo estado, Estadio estadio)
    {
        if (estado.Fixture == null)
            return false;

        return estado.Fixture.Partidos
                   .Any(p => EsMismoEstadio(p.Estadio, estadio))
               || estado.PartidosEliminatorios
                   .Any(p => EsMismoEstadio(p.Estadio, estadio));
    }

    public FaseConfiguracionTorneo ObtenerFaseConfiguracion(EstadoTorneo estado, int cantidadEquiposRegistrados)
    {
        if (estado.Fixture != null) return FaseConfiguracionTorneo.FixtureGenerado;
        if (cantidadEquiposRegistrados >= ConstantesTorneo.CantidadEquipos) return FaseConfiguracionTorneo.ListoParaFixture;
        return FaseConfiguracionTorneo.Incompleto;
    }

    private static IEnumerable<Equipo> ObtenerEquiposDePartidos(IEnumerable<Partido> partidos)
    {
        foreach (var partido in partidos)
        {
            if (partido.EquipoLocal != null) yield return partido.EquipoLocal;
            if (partido.EquipoVisitante != null) yield return partido.EquipoVisitante;
            if (partido.GanadorPorPenales != null) yield return partido.GanadorPorPenales;
        }
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
