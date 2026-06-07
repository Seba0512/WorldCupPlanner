using Microsoft.EntityFrameworkCore;
using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Excepciones;

namespace WorldCupPlanner.AccesoADatos.Repositorios;

public class RepositorioPartidoSql : IRepositorio<Partido>
{
    private readonly SqlContext _contexto;

    public RepositorioPartidoSql(SqlContext contexto)
    {
        _contexto = contexto;
    }

    public void Agregar(Partido partido)
    {
        if (_contexto.Partidos.Any(p => p.Id == partido.Id))
        {
            Actualizar(partido);
            return;
        }

        var persistido = CrearPartidoPersistible(partido);
        _contexto.Partidos.Add(persistido);
        AsignarRelaciones(persistido, partido);
        GuardarCambios();
    }

    public void Actualizar(Partido partido)
    {
        var existente = _contexto.Partidos.FirstOrDefault(p => p.Id == partido.Id);
        if (existente == null)
            throw new EntidadInexistenteExcepcion("El partido no existe.");

        existente.Fecha = partido.Fecha;
        existente.Fase = partido.Fase;
        existente.GolesLocal = partido.GolesLocal;
        existente.GolesVisitante = partido.GolesVisitante;
        AsignarRelaciones(existente, partido);
        GuardarCambios();
    }

    public void Eliminar(Partido partido)
    {
        var existente = _contexto.Partidos.FirstOrDefault(p => p.Id == partido.Id);
        if (existente == null)
            throw new EntidadInexistenteExcepcion("El partido no existe.");

        _contexto.Partidos.Remove(existente);
        GuardarCambios();
    }

    public List<Partido> Buscar(Func<Partido, bool> predicado)
    {
        return _contexto.Partidos
            .Include(p => p.Estadio)
            .Include(p => p.EquipoLocal)
            .Include(p => p.EquipoVisitante)
            .Include(p => p.GanadorPorPenales)
            .Include(p => p.Grupo)
                .ThenInclude(g => g!.Equipos)
            .AsNoTracking()
            .Where(predicado)
            .ToList();
    }

    private static Partido CrearPartidoPersistible(Partido partido) => new()
    {
        Id = partido.Id,
        Fecha = partido.Fecha,
        Fase = partido.Fase,
        GolesLocal = partido.GolesLocal,
        GolesVisitante = partido.GolesVisitante
    };

    private void AsignarRelaciones(Partido destino, Partido origen)
    {
        var entrada = _contexto.Entry(destino);
        entrada.Property("EstadioId").CurrentValue = ObtenerEstadioId(origen.Estadio);
        entrada.Property("EquipoLocalId").CurrentValue = ObtenerEquipoId(origen.EquipoLocal);
        entrada.Property("EquipoVisitanteId").CurrentValue = ObtenerEquipoId(origen.EquipoVisitante);
        entrada.Property("GanadorPorPenalesId").CurrentValue = ObtenerEquipoId(origen.GanadorPorPenales);
        entrada.Property("GrupoId").CurrentValue = ObtenerGrupoId(origen.Grupo);
    }

    private int? ObtenerEquipoId(Equipo? equipo)
    {
        if (equipo == null) return null;
        if (equipo.Id > 0) return equipo.Id;

        return _contexto.Equipos
            .AsNoTracking()
            .Where(e => e.Nombre == equipo.Nombre)
            .Select(e => (int?)e.Id)
            .FirstOrDefault();
    }

    private int? ObtenerEstadioId(Estadio? estadio)
    {
        if (estadio == null) return null;
        if (estadio.Id > 0) return estadio.Id;

        return _contexto.Estadios
            .AsNoTracking()
            .Where(e => e.Nombre == estadio.Nombre)
            .Select(e => (int?)e.Id)
            .FirstOrDefault();
    }

    private int? ObtenerGrupoId(Grupo? grupo)
    {
        if (grupo == null) return null;
        if (grupo.Id > 0) return grupo.Id;

        return _contexto.Grupos
            .AsNoTracking()
            .Where(g => g.Etiqueta == grupo.Etiqueta)
            .Select(g => (int?)g.Id)
            .FirstOrDefault();
    }

    private void GuardarCambios()
    {
        _contexto.SaveChanges();
        _contexto.ChangeTracker.Clear();
    }
}
