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
        AdjuntarSiDesvinculado(partido.Estadio);
        AdjuntarSiDesvinculado(partido.EquipoLocal);
        AdjuntarSiDesvinculado(partido.EquipoVisitante);
        AdjuntarSiDesvinculado(partido.GanadorPorPenales);
        if (partido.Grupo != null)
            AdjuntarSiDesvinculado(partido.Grupo);

        _contexto.Partidos.Add(partido);
        _contexto.SaveChanges();
    }

    public void Actualizar(Partido partido)
    {
        var existe = _contexto.Partidos.Any(p => p.Id == partido.Id);
        if (!existe)
            throw new EntidadInexistenteExcepcion("El partido no existe.");

        DesvincularInstanciaLocal(partido.Id, partido);
        _contexto.Partidos.Update(partido);
        MarcarRelacionesComoSinCambios(partido);

        _contexto.SaveChanges();
    }

    public void Eliminar(Partido partido)
    {
        var existente = _contexto.Partidos.FirstOrDefault(p => p.Id == partido.Id);
        if (existente == null)
            throw new EntidadInexistenteExcepcion("El partido no existe.");

        _contexto.Partidos.Remove(existente);
        _contexto.SaveChanges();
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

    private void AdjuntarSiDesvinculado<T>(T? entidad) where T : class
    {
        if (entidad == null) return;
        if (_contexto.Entry(entidad).State == EntityState.Detached)
            _contexto.Attach(entidad);
    }

    private void MarcarRelacionesComoSinCambios(Partido partido)
    {
        if (partido.Estadio != null)
            _contexto.Entry(partido.Estadio).State = EntityState.Unchanged;
        if (partido.EquipoLocal != null)
            _contexto.Entry(partido.EquipoLocal).State = EntityState.Unchanged;
        if (partido.EquipoVisitante != null)
            _contexto.Entry(partido.EquipoVisitante).State = EntityState.Unchanged;
        if (partido.GanadorPorPenales != null)
            _contexto.Entry(partido.GanadorPorPenales).State = EntityState.Unchanged;
        if (partido.Grupo != null)
            _contexto.Entry(partido.Grupo).State = EntityState.Unchanged;
    }

    private void DesvincularInstanciaLocal(int id, Partido entidad)
    {
        var local = _contexto.Partidos.Local.FirstOrDefault(p => p.Id == id);
        if (local != null && !ReferenceEquals(local, entidad))
            _contexto.Entry(local).State = EntityState.Detached;
    }
}
