using Microsoft.EntityFrameworkCore;
using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Excepciones;

namespace WorldCupPlanner.AccesoADatos.Repositorios;

public class RepositorioPartidoSql : IRepositorio<Partido>
{
    private readonly SqlContext _context;

    public RepositorioPartidoSql(SqlContext context)
    {
        _context = context;
    }

    public void Agregar(Partido partido)
    {
        AdjuntarReferencias(partido);
        _context.Partidos.Add(partido);
        _context.SaveChanges();
    }

    public void Actualizar(Partido partido)
    {
        var existente = _context.Partidos
            .FirstOrDefault(p => p.Id == partido.Id);

        if (existente == null)
        {
            throw new EntidadInexistenteExcepcion("El partido no existe.");
        }

        AdjuntarReferencias(partido);
        existente.Fecha = partido.Fecha;
        existente.Estadio = partido.Estadio;
        existente.EquipoLocal = partido.EquipoLocal;
        existente.EquipoVisitante = partido.EquipoVisitante;
        existente.Fase = partido.Fase;
        existente.GolesLocal = partido.GolesLocal;
        existente.GolesVisitante = partido.GolesVisitante;
        existente.Grupo = partido.Grupo;
        existente.GanadorPorPenales = partido.GanadorPorPenales;
        _context.SaveChanges();
    }

    public void Eliminar(Partido partido)
    {
        var existente = _context.Partidos.Find(partido.Id);

        if (existente == null)
        {
            return;
        }

        _context.Partidos.Remove(existente);
        _context.SaveChanges();
    }

    public List<Partido> Buscar(Func<Partido, bool> predicado)
    {
        return _context.Partidos
            .AsNoTracking()
            .Include(p => p.Estadio)
            .Include(p => p.EquipoLocal)
            .Include(p => p.EquipoVisitante)
            .Include(p => p.GanadorPorPenales)
            .Include(p => p.Grupo)
            .ToList()
            .Where(predicado)
            .ToList();
    }

    private void AdjuntarReferencias(Partido partido)
    {
        partido.Estadio = ObtenerReferenciaPersistida(
            partido.Estadio,
            _context.Estadios,
            estadio => estadio.Nombre);

        partido.EquipoLocal = ObtenerReferenciaPersistida(
            partido.EquipoLocal,
            _context.Equipos,
            equipo => equipo.Nombre);

        partido.EquipoVisitante = ObtenerReferenciaPersistida(
            partido.EquipoVisitante,
            _context.Equipos,
            equipo => equipo.Nombre);

        partido.GanadorPorPenales = ObtenerReferenciaPersistida(
            partido.GanadorPorPenales,
            _context.Equipos,
            equipo => equipo.Nombre);

        partido.Grupo = ObtenerReferenciaPersistida(
            partido.Grupo,
            _context.Grupos,
            grupo => grupo.Etiqueta);
    }

    private TEntity? ObtenerReferenciaPersistida<TEntity, TKey>(
        TEntity? entidad,
        DbSet<TEntity> entidades,
        Func<TEntity, TKey> obtenerClave)
        where TEntity : class
        where TKey : notnull
    {
        if (entidad == null)
        {
            return null;
        }

        var clave = obtenerClave(entidad);
        var entidadLocal = entidades.Local.FirstOrDefault(e =>
            EqualityComparer<TKey>.Default.Equals(obtenerClave(e), clave));

        if (entidadLocal != null)
        {
            return entidadLocal;
        }

        _context.Attach(entidad);
        return entidad;
    }
}
