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
        var partidoPersistible = CrearPartidoPersistible(partido);
        _context.Partidos.Add(partidoPersistible);
        AsignarClavesForaneas(partidoPersistible, partido);
        GuardarCambios();
    }

    public void Actualizar(Partido partido)
    {
        var existente = _context.Partidos
            .FirstOrDefault(p => p.Id == partido.Id);

        if (existente == null)
        {
            throw new EntidadInexistenteExcepcion("El partido no existe.");
        }

        existente.Fecha = partido.Fecha;
        existente.Fase = partido.Fase;
        existente.GolesLocal = partido.GolesLocal;
        existente.GolesVisitante = partido.GolesVisitante;
        AsignarClavesForaneas(existente, partido);
        GuardarCambios();
    }

    public void Eliminar(Partido partido)
    {
        var existente = _context.Partidos.Find(partido.Id);

        if (existente == null)
        {
            return;
        }

        _context.Partidos.Remove(existente);
        GuardarCambios();
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

    private static Partido CrearPartidoPersistible(Partido partido)
    {
        return new Partido
        {
            Id = partido.Id,
            Fecha = partido.Fecha,
            Fase = partido.Fase,
            GolesLocal = partido.GolesLocal,
            GolesVisitante = partido.GolesVisitante
        };
    }

    private void AsignarClavesForaneas(Partido partidoPersistible, Partido partidoConRelaciones)
    {
        var entrada = _context.Entry(partidoPersistible);
        entrada.Property("EstadioNombre").CurrentValue = partidoConRelaciones.Estadio?.Nombre;
        entrada.Property("EquipoLocalNombre").CurrentValue = partidoConRelaciones.EquipoLocal?.Nombre;
        entrada.Property("EquipoVisitanteNombre").CurrentValue = partidoConRelaciones.EquipoVisitante?.Nombre;
        entrada.Property("GanadorPorPenalesNombre").CurrentValue = partidoConRelaciones.GanadorPorPenales?.Nombre;
        entrada.Property("GrupoEtiqueta").CurrentValue = partidoConRelaciones.Grupo?.Etiqueta;
    }

    private void GuardarCambios()
    {
        _context.SaveChanges();
        _context.ChangeTracker.Clear();
    }
}
