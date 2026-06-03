using Microsoft.EntityFrameworkCore;
using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Excepciones;

namespace WorldCupPlanner.AccesoADatos.Repositorios;

public class RepositorioEstadioSql : IRepositorio<Estadio>
{
    private readonly SqlContext _contexto;

    public RepositorioEstadioSql(SqlContext contexto)
    {
        _contexto = contexto;
    }

    public void Agregar(Estadio estadio)
    {
        _contexto.Estadios.Add(estadio);
        _contexto.SaveChanges();
    }

    public void Actualizar(Estadio estadio)
    {
        var existe = _contexto.Estadios.Any(e => e.Id == estadio.Id);
        if (!existe)
            throw new EntidadInexistenteExcepcion("El estadio no existe.");

        _contexto.Estadios.Update(estadio);
        _contexto.SaveChanges();
    }

    public void Eliminar(Estadio estadio)
    {
        var existe = _contexto.Estadios.Any(e => e.Id == estadio.Id);
        if (!existe)
            throw new EntidadInexistenteExcepcion("El estadio no existe.");

        _contexto.Estadios.Remove(estadio);
        _contexto.SaveChanges();
    }

    public List<Estadio> Buscar(Func<Estadio, bool> predicado)
    {
        return _contexto.Estadios.AsNoTracking().Where(predicado).ToList();
    }
}