using Microsoft.EntityFrameworkCore;
using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Excepciones;

namespace WorldCupPlanner.AccesoADatos.Repositorios;

public class RepositorioGrupoSql : RepositorioBase<Grupo>, IRepositorio<Grupo>
{
    public RepositorioGrupoSql(SqlContext contexto) : base(contexto) { }

    public void Agregar(Grupo grupo)
    {
        var persistido = new Grupo(grupo.Etiqueta);
        foreach (var equipo in grupo.Equipos)
            persistido.Equipos.Add(ObtenerEquipoParaRelacion(equipo));

        _contexto.Grupos.Add(persistido);
        _contexto.SaveChanges();
        grupo.Id = persistido.Id;
        _contexto.ChangeTracker.Clear();
    }

    public void Actualizar(Grupo grupo)
    {
        var existe = _contexto.Grupos.Any(g => g.Id == grupo.Id);
        if (!existe)
            throw new EntidadInexistenteExcepcion("El grupo no existe.");

        var existente = ObtenerGrupoPersistido(grupo)!;
        existente.Equipos.Clear();

        foreach (var equipo in grupo.Equipos)
            existente.Equipos.Add(ObtenerEquipoParaRelacion(equipo));

        _contexto.SaveChanges();
        _contexto.ChangeTracker.Clear();
    }

    public void Eliminar(Grupo grupo)
    {
        var existente = ObtenerGrupoPersistido(grupo);
        if (existente == null)
            throw new EntidadInexistenteExcepcion("El grupo no existe.");

        _contexto.Grupos.Remove(existente);
        _contexto.SaveChanges();
        _contexto.ChangeTracker.Clear();
    }

    public List<Grupo> Buscar(Func<Grupo, bool> predicado)
    {
        return _contexto.Grupos
            .Include(g => g.Equipos)
            .Include(g => g.Partidos).ThenInclude(p => p.EquipoLocal)
            .Include(g => g.Partidos).ThenInclude(p => p.EquipoVisitante)
            .Include(g => g.Partidos).ThenInclude(p => p.Estadio)
            .Include(g => g.Partidos).ThenInclude(p => p.GanadorPorPenales)
            .AsNoTracking()
            .Where(predicado)
            .ToList();
    }

    private Grupo? ObtenerGrupoPersistido(Grupo grupo)
    {
        if (grupo.Id > IdNoAsignado)
            return _contexto.Grupos.Include(g => g.Equipos).FirstOrDefault(g => g.Id == grupo.Id);

        return _contexto.Grupos.Include(g => g.Equipos).FirstOrDefault(g => g.Etiqueta == grupo.Etiqueta);
    }

    private Equipo ObtenerEquipoParaRelacion(Equipo equipo)
    {
        var encontrado = BuscarEquipoEnContexto(equipo);
        if (encontrado != null) return encontrado;

        var nuevo = new Equipo
        {
            Nombre = equipo.Nombre,
            RankingFIFA = equipo.RankingFIFA,
            RankingInicial = equipo.RankingInicial,
            Confederacion = equipo.Confederacion,
            Bandera = equipo.Bandera
        };
        nuevo.InicializarRankingInicial();
        _contexto.Equipos.Add(nuevo);
        return nuevo;
    }
}
