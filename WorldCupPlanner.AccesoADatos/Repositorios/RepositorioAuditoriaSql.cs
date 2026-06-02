using Microsoft.EntityFrameworkCore;
using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Excepciones;

namespace WorldCupPlanner.AccesoADatos.Repositorios;

public class RepositorioAuditoriaSql : IRepositorioConId<Auditoria>
{
    private readonly SqlContext _context;

    public RepositorioAuditoriaSql(SqlContext context)
    {
        _context = context;
    }

    public int SiguienteId()
    {
        return _context.Auditorias.Any()
            ? _context.Auditorias.Max(auditoria => auditoria.Id) + 1
            : 1;
    }

    public void Agregar(Auditoria auditoria)
    {
        _context.Auditorias.Add(auditoria);
        _context.SaveChanges();
    }

    public void Actualizar(Auditoria auditoria)
    {
        var existente = _context.Auditorias.Find(auditoria.Id);

        if (existente == null)
        {
            throw new EntidadInexistenteExcepcion("El log de auditoria no existe.");
        }

        _context.Entry(existente).CurrentValues.SetValues(auditoria);
        _context.SaveChanges();
    }

    public void Eliminar(Auditoria auditoria)
    {
        var existente = _context.Auditorias.Find(auditoria.Id);

        if (existente == null)
        {
            return;
        }

        _context.Auditorias.Remove(existente);
        _context.SaveChanges();
    }

    public List<Auditoria> Buscar(Func<Auditoria, bool> predicado)
    {
        return _context.Auditorias
            .AsNoTracking()
            .ToList()
            .Where(predicado)
            .ToList();
    }
}
