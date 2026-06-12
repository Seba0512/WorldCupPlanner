using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.Servicios;

public class ServicioEstadisticasEquipo
{
    private readonly EstadoTorneo _estadoTorneo;

    public ServicioEstadisticasEquipo(EstadoTorneo estadoTorneo)
    {
        _estadoTorneo = estadoTorneo;
    }

    public Dictionary<string, EstadisticasEquipo> ObtenerEstadisticasPorEquipo(List<Equipo> equipos)
    {
        var estadisticas = equipos.ToDictionary(
            e => e.Nombre,
            _ => new EstadisticasEquipo(),
            StringComparer.OrdinalIgnoreCase);

        var partidosJugados = (_estadoTorneo.Fixture?.Partidos ?? Enumerable.Empty<Partido>())
            .Concat(_estadoTorneo.PartidosEliminatorios)
            .Where(p => p.EstaJugado());

        foreach (var partido in partidosJugados)
        {
            var golesLocal = partido.GolesLocal ?? 0;
            var golesVisitante = partido.GolesVisitante ?? 0;
            var esFaseDeGrupos = partido.Fase == FaseTorneo.FaseDeGrupos;
            var puntosLocal = esFaseDeGrupos ? CalcularPuntos(golesLocal, golesVisitante) : 0;
            var puntosVisitante = esFaseDeGrupos ? CalcularPuntos(golesVisitante, golesLocal) : 0;

            AcumularPartido(estadisticas, partido.EquipoLocal, golesLocal, golesVisitante, puntosLocal);
            AcumularPartido(estadisticas, partido.EquipoVisitante, golesVisitante, golesLocal, puntosVisitante);
        }

        return estadisticas;
    }

    public EstadisticasEquipo ObtenerEstadisticas(Equipo equipo)
    {
        var todas = ObtenerEstadisticasPorEquipo(new List<Equipo> { equipo });
        return todas.TryGetValue(equipo.Nombre, out var resultado) ? resultado : new EstadisticasEquipo();
    }

    private static void AcumularPartido(
        Dictionary<string, EstadisticasEquipo> estadisticas,
        Equipo? equipo,
        int golesAFavor,
        int golesEnContra,
        int puntos)
    {
        if (equipo == null)
            return;

        if (!estadisticas.TryGetValue(equipo.Nombre, out var entry))
        {
            entry = new EstadisticasEquipo();
            estadisticas[equipo.Nombre] = entry;
        }

        entry.PartidosJugados++;
        entry.GolesAFavor += golesAFavor;
        entry.GolesEnContra += golesEnContra;
        entry.Puntos += puntos;
    }

    private static int CalcularPuntos(int golesPropios, int golesRivales)
    {
        if (golesPropios > golesRivales)
            return ConstantesTorneo.PuntosPorVictoria;

        return golesPropios == golesRivales ? ConstantesTorneo.PuntosPorEmpate : 0;
    }
}
