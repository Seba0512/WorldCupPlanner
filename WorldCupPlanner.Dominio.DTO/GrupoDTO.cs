using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.Dominio.DTO;

public class GrupoDTO
{
    public string Etiqueta { get; set; } = string.Empty;
    public List<EquipoDTO> Equipos { get; set; } = [];
    public List<PartidoDTO> Partidos { get; set; } = [];

    public Grupo ToEntity(Dictionary<string, Equipo>? equiposPorNombre = null)
    {
        equiposPorNombre ??= new Dictionary<string, Equipo>();
        var equipos = Equipos.Select(equipoDto => MaterializarEquipo(equipoDto, equiposPorNombre)).ToList();
        var grupo = new Grupo(Etiqueta, equipos);

        grupo.Partidos.AddRange(Partidos.Select(partidoDto => partidoDto.ToEntity(equiposPorNombre, grupo)));

        return grupo;
    }

    public static GrupoDTO FromEntity(Grupo grupo)
    {
        ArgumentNullException.ThrowIfNull(grupo);

        return new GrupoDTO
        {
            Etiqueta = grupo.Etiqueta,
            Equipos = grupo.Equipos.Select(EquipoDTO.FromEntity).ToList(),
            Partidos = grupo.Partidos.Select(PartidoDTO.FromEntity).ToList()
        };
    }

    private static Equipo MaterializarEquipo(EquipoDTO equipoDto, Dictionary<string, Equipo> equiposPorNombre)
    {
        if (equiposPorNombre.TryGetValue(equipoDto.Nombre, out var existente))
            return existente;

        var equipo = equipoDto.ToEntity();
        equiposPorNombre[equipo.Nombre] = equipo;
        return equipo;
    }
}
