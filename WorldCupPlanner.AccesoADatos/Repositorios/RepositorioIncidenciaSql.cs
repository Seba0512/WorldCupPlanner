using Microsoft.EntityFrameworkCore;
using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Excepciones;

namespace WorldCupPlanner.AccesoADatos.Repositorios;

public class RepositorioIncidenciaSql : RepositorioBase<Incidencia>, IRepositorioIncidencia
{
    public RepositorioIncidenciaSql(SqlContext contexto) : base(contexto) { }

    public void Agregar(Incidencia entidad)
    {
        if (entidad.Equipo != null)
            entidad.Equipo = ResolverEquipoExistente(entidad.Equipo);

        _contexto.Incidencias.Add(entidad);
        _contexto.SaveChanges();
    }

    public void Actualizar(Incidencia entidad)
    {
        var existe = _contexto.Incidencias.Any(i => i.Id == entidad.Id);
        if (!existe)
            throw new EntidadInexistenteExcepcion("La incidencia no existe.");

        DesvincularInstanciaLocal(_contexto.Incidencias, entidad.Id, entidad);
        _contexto.Incidencias.Update(entidad);
        _contexto.SaveChanges();
    }

    public void Eliminar(Incidencia entidad)
    {
        var existente = _contexto.Incidencias.FirstOrDefault(i => i.Id == entidad.Id);
        if (existente == null)
            throw new EntidadInexistenteExcepcion("La incidencia no existe.");

        _contexto.Incidencias.Remove(existente);
        _contexto.SaveChanges();
    }

    public List<Incidencia> Buscar(Func<Incidencia, bool> predicado)
    {
        return _contexto.Incidencias
            .AsNoTracking()
            .Include(i => i.Equipo)
            .Where(predicado)
            .ToList();
    }

    public List<Incidencia> ObtenerPorPartido(int partidoId)
    {
        return _contexto.Incidencias
            .AsNoTracking()
            .Include(i => i.Equipo)
            .Where(i => i.PartidoId == partidoId)
            .ToList();
    }

    private Equipo ResolverEquipoExistente(Equipo equipo)
    {
        var encontrado = BuscarEquipoEnContexto(equipo);
        if (encontrado != null) return encontrado;

        _contexto.Entry(equipo).State = EntityState.Unchanged;
        return equipo;
    }
}
