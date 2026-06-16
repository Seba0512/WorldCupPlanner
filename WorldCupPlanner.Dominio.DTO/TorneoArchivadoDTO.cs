using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.Dominio.DTO;

public class TorneoArchivadoDTO
{
    public int Id { get; set; }
    public string GruposJson { get; set; } = string.Empty;
    public string PartidosEliminatoriosJson { get; set; } = string.Empty;

    public static TorneoArchivadoDTO FromEntity(TorneoArchivado torneoArchivado)
    {
        ArgumentNullException.ThrowIfNull(torneoArchivado);

        return new TorneoArchivadoDTO
        {
            Id = torneoArchivado.Id,
            GruposJson = torneoArchivado.GruposJson,
            PartidosEliminatoriosJson = torneoArchivado.PartidosEliminatoriosJson
        };
    }
}
