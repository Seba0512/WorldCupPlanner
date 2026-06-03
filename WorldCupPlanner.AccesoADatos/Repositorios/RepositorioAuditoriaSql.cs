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

        _contexto.Auditorias.Update(entidad);
        _contexto.SaveChanges();
    }

    public void Eliminar(Auditoria entidad)
    {
        _contexto.Auditorias.Remove(entidad);
        _contexto.SaveChanges();
    }

    public List<Auditoria> Buscar(Func<Auditoria, bool> predicado)
    {
        return _contexto.Auditorias.AsNoTracking().Where(predicado).ToList();
    }
}