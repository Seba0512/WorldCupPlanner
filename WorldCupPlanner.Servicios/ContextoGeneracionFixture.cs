using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.Servicios;

public class ContextoGeneracionFixture
{
    public List<Estadio> EstadiosOrdenados { get; set; } = new();
    public Dictionary<DateTime, int> CantidadPartidosPorDia { get; set; } = new();
    public int IndicePartidoGlobal { get; set; }
}
