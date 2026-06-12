using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.Servicios;

public class ProveedorPartidosEstadoTorneo : IProveedorPartidos
{
    private readonly EstadoTorneo? _estadoTorneo;

    public ProveedorPartidosEstadoTorneo(EstadoTorneo? estadoTorneo)
    {
        _estadoTorneo = estadoTorneo;
    }

    public Partido? BuscarPorId(int idPartido)
    {
        return _estadoTorneo?.Fixture?.Partidos.FirstOrDefault(p => p.Id == idPartido)
               ?? _estadoTorneo?.PartidosEliminatorios.FirstOrDefault(p => p.Id == idPartido);
    }

    public List<Partido> ObtenerTodos()
    {
        var partidos = new List<Partido>();
        if (_estadoTorneo?.Fixture != null)
            partidos.AddRange(_estadoTorneo.Fixture.Partidos);
        partidos.AddRange(_estadoTorneo?.PartidosEliminatorios ?? new List<Partido>());
        return partidos;
    }

    public void Actualizar(Partido partido)
    {
        _estadoTorneo?.ActualizarPartido(partido);
    }
}
