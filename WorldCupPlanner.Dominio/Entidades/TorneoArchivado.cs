namespace WorldCupPlanner.Dominio.Entidades;

public class TorneoArchivado
{
    public int Id { get; set; }
    public string GruposJson { get; set; } = string.Empty;
    public string PartidosEliminatoriosJson { get; set; } = string.Empty;
}
