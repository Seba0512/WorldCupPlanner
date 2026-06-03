using Microsoft.EntityFrameworkCore;
using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Excepciones;

namespace WorldCupPlanner.AccesoADatos.Repositorios;

public class RepositorioEquipoSql : IRepositorio<Equipo>
{
    private readonly SqlContext _contexto;

    public RepositorioEquipoSql(SqlContext contexto)
    {
        _contexto = contexto;
    }

    public void Agregar(Equipo equipo)
    {
        _contexto.Equipos.Add(equipo);
        _contexto.SaveChanges();
    }

    public void Actualizar(Equipo equipo)
    {
        var existe = _contexto.Equipos.Any(e => e.Id == equipo.Id);
        if (!existe)
            throw new EntidadInexistenteExcepcion("El equipo no existe.");

        _contexto.Equipos.Update(equipo);
        _contexto.SaveChanges();
    }

    public void Eliminar(Equipo equipo)
    {
        var existe = _contexto.Equipos.Any(e => e.Id == equipo.Id);
        if (!existe)
            throw new EntidadInexistenteExcepcion("El equipo no existe.");

        _contexto.Equipos.Remove(equipo);
        _contexto.SaveChanges();
    }

    public List<Equipo> Buscar(Func<Equipo, bool> predicado)
    {
        return _contexto.Equipos.AsNoTracking().Where(predicado).ToList();
    }
}