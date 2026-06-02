using Microsoft.EntityFrameworkCore;
using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Excepciones;

namespace WorldCupPlanner.AccesoADatos.Repositorios;

public class RepositorioEquipoSql : IRepositorio<Equipo>
{
    private readonly SqlContext _context;

    public RepositorioEquipoSql(SqlContext context)
    {
        _context = context;
    }

    public void Agregar(Equipo equipo)
    {
        _context.Equipos.Add(equipo);
        _context.SaveChanges();
    }

    public void Actualizar(Equipo equipo)
    {
        var existente = _context.Equipos.Find(equipo.Nombre);

        if (existente == null)
        {
            throw new EntidadInexistenteExcepcion("El equipo no existe.");
        }

        existente.RankingFIFA = equipo.RankingFIFA;
        existente.Confederacion = equipo.Confederacion;
        _context.SaveChanges();
    }

    public void Eliminar(Equipo equipo)
    {
        var existente = _context.Equipos.Find(equipo.Nombre);

        if (existente == null)
        {
            return;
        }

        _context.Equipos.Remove(existente);
        _context.SaveChanges();
    }

    public List<Equipo> Buscar(Func<Equipo, bool> predicado)
    {
        return _context.Equipos
            .AsNoTracking()
            .ToList()
            .Where(predicado)
            .ToList();
    }
}
