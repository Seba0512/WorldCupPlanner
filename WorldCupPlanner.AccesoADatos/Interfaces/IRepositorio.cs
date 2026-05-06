
namespace WorldCupPlanner.AccesoADatos.Interfaces;

public interface IRepositorio<T>
{
    void Agregar(T entidad);
    void Actualizar(T entidad);
    void Eliminar(T entidad);
    List<T> Buscar(Func<T,bool> predicado);
}
