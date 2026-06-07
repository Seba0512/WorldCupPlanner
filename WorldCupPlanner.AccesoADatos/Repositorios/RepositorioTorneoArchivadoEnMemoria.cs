using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.AccesoADatos.Repositorios;

public class RepositorioTorneoArchivadoEnMemoria : IRepositorioTorneoArchivado
{
    private TorneoArchivado? _archivo;

    public void Guardar(TorneoArchivado torneoArchivado) => _archivo = torneoArchivado;
    public TorneoArchivado? Obtener() => _archivo;
}
