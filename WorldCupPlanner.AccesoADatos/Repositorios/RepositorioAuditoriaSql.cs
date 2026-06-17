using Microsoft.EntityFrameworkCore;
using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Excepciones;

namespace WorldCupPlanner.AccesoADatos.Repositorios;

public class RepositorioAuditoriaSql : RepositorioBase<Auditoria>, IRepositorioConId<Auditoria>
{
    public RepositorioAuditoriaSql(SqlContext contexto) : base(contexto) { }

    public int SiguienteId() => IdNoAsignado;

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

        DesvincularInstanciaLocal(_contexto.Auditorias, entidad.Id, entidad);
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
}
