using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.Servicios;

public class ProveedorPartidosRepositorio : IProveedorPartidos
{
    private readonly IRepositorio<Partido> _repositorio;

    public ProveedorPartidosRepositorio(IRepositorio<Partido> repositorio)
    {
        _repositorio = repositorio;
    }

    public Partido? BuscarPorId(int idPartido)
    {
        return _repositorio.Buscar(p => p.Id == idPartido).FirstOrDefault();
    }

    public List<Partido> ObtenerTodos()
    {
        return _repositorio.Buscar(_ => true);
    }

    public void Actualizar(Partido partido)
    {
        _repositorio.Actualizar(partido);
    }
}
