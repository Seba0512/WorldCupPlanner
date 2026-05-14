using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.Servicios;

public class ContextoGeneracionFixture
{
    private readonly Dictionary<DateTime, int> _partidosPorDia = new();

    public IReadOnlyList<Estadio> EstadiosOrdenados { get; init; } = [];
    public ConfiguracionTorneo ConfiguracionTorneo { get; init; } = new();
    public int IndicePartidoGlobal { get; private set; }

    public void RegistrarPartido(DateTime fecha)
    {
        var dia = fecha.Date;
        _partidosPorDia[dia] = ObtenerPartidosDelDia(dia) + 1;
        IndicePartidoGlobal++;
    }

    public int ObtenerPartidosDelDia(DateTime fecha)
        => _partidosPorDia.TryGetValue(fecha.Date, out var cantidad) ? cantidad : 0;
}
