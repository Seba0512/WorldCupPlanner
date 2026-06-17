using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.ServicioTests;

internal sealed class RepositorioAuditoriaFake : IRepositorioConId<Auditoria>
{
    private int _nextId = 1;
    public List<Auditoria> Entradas { get; } = [];

    public int SiguienteId() => _nextId++;
    public void Agregar(Auditoria entidad) => Entradas.Add(entidad);
    public void Actualizar(Auditoria entidad) { }
    public void Eliminar(Auditoria entidad) => Entradas.Remove(entidad);
    public List<Auditoria> Buscar(Func<Auditoria, bool> predicado) => Entradas.Where(predicado).ToList();
}
