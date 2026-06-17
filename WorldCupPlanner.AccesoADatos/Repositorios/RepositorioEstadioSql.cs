using Microsoft.EntityFrameworkCore;
using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Excepciones;

namespace WorldCupPlanner.AccesoADatos.Repositorios;

public class RepositorioEstadioSql : RepositorioBase<Estadio>, IRepositorio<Estadio>, IRepositorioConClaveEditable<Estadio>
{
    public RepositorioEstadioSql(SqlContext contexto) : base(contexto) { }

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

        DesvincularInstanciaLocal(_contexto.Estadios, estadio.Id, estadio);
        _contexto.Estadios.Update(estadio);
        _contexto.SaveChanges();
    }

    public void ActualizarClave(string claveOriginal, Estadio estadio)
    {
        var existente = _contexto.Estadios.FirstOrDefault(e => e.Nombre == claveOriginal);
        if (existente == null)
            throw new EntidadInexistenteExcepcion("El estadio no existe.");

        existente.Nombre = estadio.Nombre;
        existente.Ciudad = estadio.Ciudad;
        existente.Capacidad = estadio.Capacidad;
        existente.Descripcion = estadio.Descripcion;
        _contexto.SaveChanges();
    }

    public void Eliminar(Estadio estadio)
    {
        var existente = ObtenerEstadioPersistido(estadio);
        if (existente == null)
            throw new EntidadInexistenteExcepcion("El estadio no existe.");

        _contexto.Estadios.Remove(existente);
        _contexto.SaveChanges();
    }

    public List<Estadio> Buscar(Func<Estadio, bool> predicado)
    {
        return _contexto.Estadios.AsNoTracking().Where(predicado).ToList();
    }

    private Estadio? ObtenerEstadioPersistido(Estadio estadio)
    {
        if (estadio.Id > IdNoAsignado)
            return _contexto.Estadios.FirstOrDefault(e => e.Id == estadio.Id);

        return _contexto.Estadios.FirstOrDefault(e => e.Nombre == estadio.Nombre);
    }
}
