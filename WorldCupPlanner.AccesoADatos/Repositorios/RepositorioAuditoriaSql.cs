using Microsoft.EntityFrameworkCore;
using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Excepciones;

namespace WorldCupPlanner.AccesoADatos.Repositorios;

public class RepositorioAuditoriaSql : IRepositorioConId<Auditoria>
{
    private readonly SqlContext _contexto;

    public RepositorioAuditoriaSql(SqlContext contexto)
    {
        _contexto = contexto;
    }

    public int SiguienteId()
    {
        return 0;
    }

    public void Agregar(Auditoria entidad)
    {
        _contexto.Auditorias.Add(entidad);
        _contexto.SaveChanges();
    }

    public void Actualizar(Auditoria entidad)
    {
        var existe = _contexto.Auditorias.Any(a => a.Id == entidad.Id);
        if (!existe)
            throw new EntidadInexistenteExcepcion("El log de auditoría no existe.");

        DesvincularInstanciaLocal(entidad.Id, entidad);
        _contexto.Auditorias.Update(entidad);
        _contexto.SaveChanges();
    }

    public void Eliminar(Auditoria entidad)
    {
        var existente = _contexto.Auditorias.FirstOrDefault(a => a.Id == entidad.Id);
        if (existente == null)
            throw new EntidadInexistenteExcepcion("El log de auditoria no existe.");

        _contexto.Auditorias.Remove(existente);
        _contexto.SaveChanges();
    }

    public List<Auditoria> Buscar(Func<Auditoria, bool> predicado)
    {
        return _contexto.Auditorias.AsNoTracking().Where(predicado).ToList();
    }

    private void DesvincularInstanciaLocal(int id, Auditoria entidad)
    {
        var local = _contexto.Auditorias.Local.FirstOrDefault(a => a.Id == id);
        if (local != null && !ReferenceEquals(local, entidad))
            _contexto.Entry(local).State = EntityState.Detached;
    }
}
