using Microsoft.EntityFrameworkCore;
using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Excepciones;

namespace WorldCupPlanner.AccesoADatos.Repositorios;

public class RepositorioEstadioSql : IRepositorio<Estadio>, IRepositorioConClaveEditable<Estadio>
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
        _context.ChangeTracker.Clear();
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
        _context.ChangeTracker.Clear();
    }

    public void ActualizarClave(string nombreOriginal, Estadio estadio)
    {
        if (string.Equals(nombreOriginal, estadio.Nombre, StringComparison.OrdinalIgnoreCase))
        {
            Actualizar(estadio);
            return;
        }

        var existente = _context.Estadios
            .AsNoTracking()
            .FirstOrDefault(e => e.Nombre == nombreOriginal);

        if (existente == null)
        {
            throw new EntidadInexistenteExcepcion("El estadio no existe.");
        }

        using var transaction = _context.Database.BeginTransaction();
        try
        {
            _context.Estadios.Add(estadio);
            _context.SaveChanges();

            _context.Database.ExecuteSqlInterpolated(
                $"UPDATE Partidos SET EstadioNombre = {estadio.Nombre} WHERE EstadioNombre = {nombreOriginal}");
            _context.Database.ExecuteSqlInterpolated(
                $"DELETE FROM Estadios WHERE Nombre = {nombreOriginal}");

            transaction.Commit();
        }
        finally
        {
            _context.ChangeTracker.Clear();
        }
    }

    public void Eliminar(Estadio estadio)
    {
        var existente = _context.Estadios.Find(estadio.Nombre);

        if (existente == null)
        {
            return;
        }

        using var transaction = _context.Database.BeginTransaction();
        try
        {
            _context.Database.ExecuteSqlInterpolated(
                $"UPDATE Partidos SET EstadioNombre = NULL WHERE EstadioNombre = {estadio.Nombre}");

            _context.Estadios.Remove(existente);
            _context.SaveChanges();
            transaction.Commit();
        }
        finally
        {
            _context.ChangeTracker.Clear();
        }
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
