using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.Dominio.DTO;

public class GrupoDTO
{
    public string Etiqueta { get; set; } = string.Empty;
    public List<EquipoDTO> Equipos { get; set; } = [];
    public List<PartidoDTO> Partidos { get; set; } = [];

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
}
