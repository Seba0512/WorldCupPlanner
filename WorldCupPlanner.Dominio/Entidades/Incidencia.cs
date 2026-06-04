namespace WorldCupPlanner.Dominio.Entidades;

public abstract class Incidencia
{
    public int Id { get; set; }
    public int MinutoDelPartido { get; set; }
    public Equipo? Equipo { get; set; }
    public int? PartidoId { get; set; }
}