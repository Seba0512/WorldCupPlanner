using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Excepciones;

namespace WorldCupPlanner.AccesoADatos.Repositorios;

public class RepositorioPartidoSql : IRepositorio<Partido>
{
    private const int IdNoAsignado = 0;
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

    private int? ObtenerEquipoId(Equipo? equipo) =>
        ObtenerId(_contexto.Equipos, equipo, e => e.Nombre == equipo!.Nombre);

    private int? ObtenerEstadioId(Estadio? estadio) =>
        ObtenerId(_contexto.Estadios, estadio, e => e.Nombre == estadio!.Nombre);

    private int? ObtenerGrupoId(Grupo? grupo) =>
        ObtenerId(_contexto.Grupos, grupo, g => g.Etiqueta == grupo!.Etiqueta);

    private int? ObtenerId<T>(DbSet<T> conjunto, T? entidad, Expression<Func<T, bool>> predicadoClave)
        where T : class, IEntidadConId
    {
        if (entidad == null) return null;
        if (entidad.Id > IdNoAsignado) return entidad.Id;

        return conjunto
            .AsNoTracking()
            .Where(predicadoClave)
            .Select(e => (int?)e.Id)
            .FirstOrDefault();
    }

    private void GuardarCambios()
    {
        _contexto.SaveChanges();
        _contexto.ChangeTracker.Clear();
    }
}
