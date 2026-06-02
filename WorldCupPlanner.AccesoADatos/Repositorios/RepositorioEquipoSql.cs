using Microsoft.EntityFrameworkCore;
using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Excepciones;

namespace WorldCupPlanner.AccesoADatos.Repositorios;

public class RepositorioEquipoSql : IRepositorio<Equipo>, IRepositorioConClaveEditable<Equipo>
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
        _context.ChangeTracker.Clear();
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
        _context.ChangeTracker.Clear();
    }

    public void ActualizarClave(string nombreOriginal, Equipo equipo)
    {
        if (string.Equals(nombreOriginal, equipo.Nombre, StringComparison.OrdinalIgnoreCase))
        {
            Actualizar(equipo);
            return;
        }

        var existente = _context.Equipos
            .AsNoTracking()
            .FirstOrDefault(e => e.Nombre == nombreOriginal);

        if (existente == null)
        {
            throw new EntidadInexistenteExcepcion("El equipo no existe.");
        }

        using var transaction = _context.Database.BeginTransaction();
        try
        {
            _context.Equipos.Add(equipo);
            _context.SaveChanges();

            _context.Database.ExecuteSqlInterpolated(
                $"UPDATE Partidos SET EquipoLocalNombre = {equipo.Nombre} WHERE EquipoLocalNombre = {nombreOriginal}");
            _context.Database.ExecuteSqlInterpolated(
                $"UPDATE Partidos SET EquipoVisitanteNombre = {equipo.Nombre} WHERE EquipoVisitanteNombre = {nombreOriginal}");
            _context.Database.ExecuteSqlInterpolated(
                $"UPDATE Partidos SET GanadorPorPenalesNombre = {equipo.Nombre} WHERE GanadorPorPenalesNombre = {nombreOriginal}");
            _context.Database.ExecuteSqlInterpolated(
                $"DELETE FROM Equipos WHERE Nombre = {nombreOriginal}");

            transaction.Commit();
        }
        finally
        {
            _context.ChangeTracker.Clear();
        }
    }

    public void Eliminar(Equipo equipo)
    {
        var existente = _context.Equipos.Find(equipo.Nombre);

        if (existente == null)
        {
            return;
        }

        using var transaction = _context.Database.BeginTransaction();
        try
        {
            _context.Database.ExecuteSqlInterpolated(
                $"UPDATE Partidos SET EquipoLocalNombre = NULL WHERE EquipoLocalNombre = {equipo.Nombre}");
            _context.Database.ExecuteSqlInterpolated(
                $"UPDATE Partidos SET EquipoVisitanteNombre = NULL WHERE EquipoVisitanteNombre = {equipo.Nombre}");
            _context.Database.ExecuteSqlInterpolated(
                $"UPDATE Partidos SET GanadorPorPenalesNombre = NULL WHERE GanadorPorPenalesNombre = {equipo.Nombre}");

            _context.Equipos.Remove(existente);
            _context.SaveChanges();
            transaction.Commit();
        }
        finally
        {
            _context.ChangeTracker.Clear();
        }
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
