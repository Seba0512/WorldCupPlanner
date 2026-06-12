using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.Servicios;

public interface IProveedorPartidos
{
    Partido? BuscarPorId(int idPartido);
    List<Partido> ObtenerTodos();
    void Actualizar(Partido partido);
}
