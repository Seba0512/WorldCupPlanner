using Microsoft.EntityFrameworkCore;
using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Excepciones;

namespace WorldCupPlanner.AccesoADatos.Repositorios;

public class RepositorioEstadioSql : IRepositorio<Estadio>
{
    private readonly SqlContext _context;

    public RepositorioEstadioSql(SqlContext context)
    {
        _context = context;
    }

    public void Agregar(Estadio estadio)
    {
        _context.Estadios.Add(estadio);
        _context.SaveChanges();
    }

    public void Actualizar(Estadio estadio)
    {
        var existente = _context.Estadios.Find(estadio.Nombre);

        if (existente == null)
        {
            throw new EntidadInexistenteExcepcion("El estadio no existe.");
        }

        existente.Ciudad = estadio.Ciudad;
        existente.Capacidad = estadio.Capacidad;
        existente.Descripcion = estadio.Descripcion;
        _context.SaveChanges();
    }

    public void Eliminar(Estadio estadio)
    {
        var existente = _context.Estadios.Find(estadio.Nombre);

        if (existente == null)
        {
            return;
        }

        _context.Estadios.Remove(existente);
        _context.SaveChanges();
    }

    public List<Estadio> Buscar(Func<Estadio, bool> predicado)
    {
        return _context.Estadios
            .AsNoTracking()
            .ToList()
            .Where(predicado)
            .ToList();
    }
}
