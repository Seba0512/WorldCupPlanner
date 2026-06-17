using Microsoft.EntityFrameworkCore;
using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.AccesoADatos.Repositorios;

public abstract class RepositorioBase<T> where T : class, IEntidadConId
{
    protected const int IdNoAsignado = 0;
    protected readonly SqlContext _contexto;

    protected RepositorioBase(SqlContext contexto)
    {
        _contexto = contexto;
    }

    protected void DesvincularInstanciaLocal(DbSet<T> conjunto, int id, T entidad)
    {
        var local = conjunto.Local.FirstOrDefault(e => e.Id == id);
        if (local != null && !ReferenceEquals(local, entidad))
            _contexto.Entry(local).State = EntityState.Detached;
    }

    protected Equipo? BuscarEquipoEnContexto(Equipo equipo)
    {
        if (equipo.Id > IdNoAsignado)
        {
            var localPorId = _contexto.Equipos.Local.FirstOrDefault(e => e.Id == equipo.Id);
            if (localPorId != null) return localPorId;

            var persistidoPorId = _contexto.Equipos.FirstOrDefault(e => e.Id == equipo.Id);
            if (persistidoPorId != null) return persistidoPorId;
        }

        var localPorNombre = _contexto.Equipos.Local.FirstOrDefault(e => e.Nombre == equipo.Nombre);
        if (localPorNombre != null) return localPorNombre;

        return _contexto.Equipos.FirstOrDefault(e => e.Nombre == equipo.Nombre);
    }
}
