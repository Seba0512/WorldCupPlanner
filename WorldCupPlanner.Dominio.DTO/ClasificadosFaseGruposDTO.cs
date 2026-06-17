using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.Dominio.DTO;

public class ClasificadosFaseGruposDTO
{
    public List<EquipoDTO> Primeros { get; set; } = [];
    public List<EquipoDTO> Segundos { get; set; } = [];
    public List<EquipoDTO> MejoresTerceros { get; set; } = [];
    public List<GrupoOrigenClasificadoDTO> GruposDeOrigen { get; set; } = [];

    public List<EquipoDTO> Todos =>
        Primeros
            .Concat(Segundos)
            .Concat(MejoresTerceros)
            .ToList();

    public static ClasificadosFaseGruposDTO FromEntity(ClasificadosFaseGrupos clasificados)
    {
        ArgumentNullException.ThrowIfNull(clasificados);

        return new ClasificadosFaseGruposDTO
        {
            Primeros = clasificados.Primeros.Select(EquipoDTO.FromEntity).ToList(),
            Segundos = clasificados.Segundos.Select(EquipoDTO.FromEntity).ToList(),
            MejoresTerceros = clasificados.MejoresTerceros.Select(EquipoDTO.FromEntity).ToList(),
            GruposDeOrigen = clasificados.GruposDeOrigen
                .Select(equipoConGrupo => GrupoOrigenClasificadoDTO.FromEntity(equipoConGrupo.Key, equipoConGrupo.Value))
                .ToList()
        };
    }
}
