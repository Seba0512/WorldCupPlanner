using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.AccesoADatos.Interfaces;

public interface IRepositorioTorneoArchivado
{
    void Guardar(TorneoArchivado torneoArchivado);
    TorneoArchivado? Obtener();
}
