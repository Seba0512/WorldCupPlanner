using System.Diagnostics.CodeAnalysis;
using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.ControladorTests;

[ExcludeFromCodeCoverage]
internal sealed class RepositorioAuditoriaFake : IRepositorioConId<Auditoria>
{
    private readonly List<Auditoria> _auditorias = new();
    private int _siguienteId;

    public int SiguienteId() => ++_siguienteId;

    public void Agregar(Auditoria auditoria) => _auditorias.Add(auditoria);

    public void Actualizar(Auditoria auditoria)
    {
        var indice = _auditorias.FindIndex(a => a.Id == auditoria.Id);
        if (indice >= 0)
            _auditorias[indice] = auditoria;
    }

    public void Eliminar(Auditoria auditoria) => _auditorias.Remove(auditoria);

    public List<Auditoria> Buscar(Func<Auditoria, bool> predicado) =>
        _auditorias.Where(predicado).ToList();
}
