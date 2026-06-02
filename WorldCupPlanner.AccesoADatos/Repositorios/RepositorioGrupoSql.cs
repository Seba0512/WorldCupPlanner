using Microsoft.EntityFrameworkCore;
using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Excepciones;

namespace WorldCupPlanner.AccesoADatos.Repositorios;

public class RepositorioGrupoSql : IRepositorio<Grupo>
{
    private readonly SqlContext _context;

    public RepositorioGrupoSql(SqlContext context)
    {
        _context = context;
    }

    public void Agregar(Grupo grupo)
    {
        _context.Grupos.Add(grupo);
        _context.SaveChanges();
        _context.ChangeTracker.Clear();
    }

    public void Actualizar(Grupo grupo)
    {
        var existente = _context.Grupos.Find(grupo.Etiqueta);

        if (existente == null)
        {
            throw new EntidadInexistenteExcepcion("El grupo no existe.");
        }
    }

    public void Eliminar(Grupo grupo)
    {
        var existente = _context.Grupos.Find(grupo.Etiqueta);

        if (existente == null)
        {
            return;
        }

        _context.Grupos.Remove(existente);
        _context.SaveChanges();
        _context.ChangeTracker.Clear();
    }

    public List<Grupo> Buscar(Func<Grupo, bool> predicado)
    {
        return _context.Grupos
            .AsNoTracking()
            .ToList()
            .Where(predicado)
            .ToList();
    }
}
