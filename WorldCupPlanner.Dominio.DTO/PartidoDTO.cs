using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.Dominio.DTO;

public class PartidoDTO
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public EstadioDTO? Estadio { get; set; }
    public EquipoDTO? EquipoLocal { get; set; }
    public EquipoDTO? EquipoVisitante { get; set; }
    public FaseTorneo Fase { get; set; }
    public int? GolesLocal { get; set; }
    public int? GolesVisitante { get; set; }
    public EquipoDTO? GanadorPorPenales { get; set; }
    public string? GrupoEtiqueta { get; set; }

    public Partido ToEntity(Dictionary<string, Equipo>? equiposPorNombre = null, Grupo? grupo = null)
    {
        equiposPorNombre ??= new Dictionary<string, Equipo>();

        return new Partido
        {
            Id = Id,
            Fecha = Fecha,
            Estadio = Estadio?.ToEntity(),
            EquipoLocal = MaterializarEquipo(EquipoLocal, equiposPorNombre),
            EquipoVisitante = MaterializarEquipo(EquipoVisitante, equiposPorNombre),
            Fase = Fase,
            GolesLocal = GolesLocal,
            GolesVisitante = GolesVisitante,
            GanadorPorPenales = MaterializarEquipo(GanadorPorPenales, equiposPorNombre),
            Grupo = grupo ?? CrearGrupoDesdeEtiqueta()
        };
    }

    public static PartidoDTO FromEntity(Partido partido)
    {
        ArgumentNullException.ThrowIfNull(partido);

        return new PartidoDTO
        {
            Id = partido.Id,
            Fecha = partido.Fecha,
            Estadio = partido.Estadio == null ? null : EstadioDTO.FromEntity(partido.Estadio),
            EquipoLocal = partido.EquipoLocal == null ? null : EquipoDTO.FromEntity(partido.EquipoLocal),
            EquipoVisitante = partido.EquipoVisitante == null ? null : EquipoDTO.FromEntity(partido.EquipoVisitante),
            Fase = partido.Fase,
            GolesLocal = partido.GolesLocal,
            GolesVisitante = partido.GolesVisitante,
            GanadorPorPenales = partido.GanadorPorPenales == null ? null : EquipoDTO.FromEntity(partido.GanadorPorPenales),
            GrupoEtiqueta = partido.Grupo?.Etiqueta
        };
    }

    private static Equipo? MaterializarEquipo(EquipoDTO? equipoDto, Dictionary<string, Equipo> equiposPorNombre)
    {
        if (equipoDto == null)
            return null;

        if (equiposPorNombre.TryGetValue(equipoDto.Nombre, out var existente))
            return existente;

        var equipo = equipoDto.ToEntity();
        equiposPorNombre[equipo.Nombre] = equipo;
        return equipo;
    }

    private Grupo? CrearGrupoDesdeEtiqueta()
    {
        return string.IsNullOrWhiteSpace(GrupoEtiqueta)
            ? null
            : new Grupo(GrupoEtiqueta);
    }
}
