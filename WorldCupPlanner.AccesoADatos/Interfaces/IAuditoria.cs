using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.AccesoADatos.Interfaces;

public interface IAuditoria
{
    void Agregar(Auditoria entry);
    IEnumerable<Auditoria> ObtenerTodos();
    int SiguienteId();
}