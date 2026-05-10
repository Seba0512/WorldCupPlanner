namespace WorldCupPlanner.Servicios;

using WorldCupPlanner.Dominio.Entidades;

public class EstadoTorneo
{
    public Fixture? Fixture { get; set; }
    public List<Partido> PartidosEliminatorios { get; set; } = new();
}