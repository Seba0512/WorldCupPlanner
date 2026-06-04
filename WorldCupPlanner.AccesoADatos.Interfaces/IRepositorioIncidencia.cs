using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.AccesoADatos.Interfaces;

public interface IRepositorioIncidencia : IRepositorio<Incidencia>
{
    List<Incidencia> ObtenerPorPartido(int partidoId);
}