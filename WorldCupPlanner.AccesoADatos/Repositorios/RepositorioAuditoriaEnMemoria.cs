using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.AccesoADatos.Repositorios;

public class RepositorioAuditoriaEnMemoria : IAuditoria
{
    private readonly List<Auditoria> _logs = new();
    private int _contador = 0;

    public void Agregar(Auditoria entry) => _logs.Add(entry);
    public IEnumerable<Auditoria> ObtenerTodos() => _logs.AsReadOnly();
    public int SiguienteId() => ++_contador;
}
