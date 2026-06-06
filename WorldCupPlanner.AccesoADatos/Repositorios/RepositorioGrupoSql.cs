using Microsoft.EntityFrameworkCore;
using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Excepciones;

namespace WorldCupPlanner.AccesoADatos.Repositorios;

public class RepositorioGrupoSql : IRepositorio<Grupo>
{
    private readonly SqlContext _contexto;

    public RepositorioGrupoSql(SqlContext contexto)
    {
        _contexto = contexto;
    }

    public void Agregar(Grupo grupo)
    {
        foreach (var equipo in grupo.Equipos)
        {
            if (_contexto.Entry(equipo).State == EntityState.Detached)
                _contexto.Equipos.Attach(equipo);
        }

        _contexto.Grupos.Add(grupo);
        _contexto.SaveChanges();
    }

    public void Actualizar(Grupo grupo)
    {
        var existe = _contexto.Grupos.Any(g => g.Id == grupo.Id);
        if (!existe)
            throw new EntidadInexistenteExcepcion("El grupo no existe.");

        DesvincularInstanciaLocal(grupo.Id, grupo);
        _contexto.Grupos.Update(grupo);

        foreach (var equipo in grupo.Equipos)
            _contexto.Entry(equipo).State = EntityState.Unchanged;

        _contexto.SaveChanges();
    }

    public void Eliminar(Grupo grupo)
    {
        var existente = ObtenerGrupoPersistido(grupo);
        if (existente == null)
            throw new EntidadInexistenteExcepcion("El grupo no existe.");

        _contexto.Grupos.Remove(existente);
        _contexto.SaveChanges();
    }

    public List<Grupo> Buscar(Func<Grupo, bool> predicado)
    {
        return _contexto.Grupos
            .Include(g => g.Equipos)
            .Include(g => g.Partidos)
            .AsNoTracking()
            .Where(predicado)
            .ToList();
    }

    private Grupo? ObtenerGrupoPersistido(Grupo grupo)
    {
        if (grupo.Id > 0)
            return _contexto.Grupos.Include(g => g.Equipos).FirstOrDefault(g => g.Id == grupo.Id);

        return _contexto.Grupos.Include(g => g.Equipos).FirstOrDefault(g => g.Etiqueta == grupo.Etiqueta);
    }

    private void DesvincularInstanciaLocal(int id, Grupo entidad)
    {
        var local = _contexto.Grupos.Local.FirstOrDefault(g => g.Id == id);
        if (local != null && !ReferenceEquals(local, entidad))
            _contexto.Entry(local).State = EntityState.Detached;
    }
}
