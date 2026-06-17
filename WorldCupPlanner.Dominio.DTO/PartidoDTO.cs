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
    public List<IncidenciaDTO> Incidencias { get; set; } = [];

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
            GrupoEtiqueta = partido.Grupo?.Etiqueta,
            Incidencias = partido.Incidencias.Select(IncidenciaDTO.FromEntity).ToList()
        };
    }
}
