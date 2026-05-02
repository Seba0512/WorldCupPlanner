using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.AccesoADatos.Interfaces;

public interface IRepositorioEquipo
{
    void Agregar(Equipo equipo);
    void Actualizar(Equipo equipo);
    void Eliminar(Equipo equipo);
    List<Equipo> ObtenerTodos();
}
