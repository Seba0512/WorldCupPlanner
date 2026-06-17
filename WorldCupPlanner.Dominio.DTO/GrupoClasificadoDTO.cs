using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.Dominio.DTO;

public class GrupoClasificadoDTO
{
    public string Etiqueta { get; set; } = string.Empty;
    public List<EquipoDTO> Equipos { get; set; } = [];

    public static GrupoClasificadoDTO FromEntity(Grupo grupo)
    {
        ArgumentNullException.ThrowIfNull(grupo);

        return new GrupoClasificadoDTO
        {
            Etiqueta = grupo.Etiqueta,
            Equipos = grupo.Equipos.Select(EquipoDTO.FromEntity).ToList()
        };
    }
}
