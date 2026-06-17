using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.Dominio.DTO;

public class GrupoOrigenClasificadoDTO
{
    public EquipoDTO Equipo { get; set; } = new();
    public GrupoClasificadoDTO Grupo { get; set; } = new();

    public static GrupoOrigenClasificadoDTO FromEntity(Equipo equipo, Grupo grupo)
    {
        ArgumentNullException.ThrowIfNull(equipo);
        ArgumentNullException.ThrowIfNull(grupo);

        return new GrupoOrigenClasificadoDTO
        {
            Equipo = EquipoDTO.FromEntity(equipo),
            Grupo = GrupoClasificadoDTO.FromEntity(grupo)
        };
    }
}
