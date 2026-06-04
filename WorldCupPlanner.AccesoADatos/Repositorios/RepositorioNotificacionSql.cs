using Microsoft.EntityFrameworkCore;
using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Excepciones;

namespace WorldCupPlanner.AccesoADatos.Repositorios;

public class RepositorioNotificacionSql : IRepositorioNotificacion
{
    private readonly SqlContext _contexto;

    public RepositorioNotificacionSql(SqlContext contexto)
    {
        _contexto = contexto;
    }

    public void Agregar(Notificacion entidad)
    {
        _contexto.Notificaciones.Add(entidad);
        _contexto.SaveChanges();
    }

    public void Actualizar(Notificacion entidad)
    {
        var existe = _contexto.Notificaciones.Any(n => n.Id == entidad.Id);
        if (!existe)
            throw new EntidadInexistenteExcepcion("La notificación no existe.");

        DesvincularInstanciaLocal(entidad.Id, entidad);
        _contexto.Notificaciones.Update(entidad);
        _contexto.SaveChanges();
    }

    public void Eliminar(Notificacion entidad)
    {
        var existente = _contexto.Notificaciones.FirstOrDefault(n => n.Id == entidad.Id);
        if (existente == null)
            throw new EntidadInexistenteExcepcion("La notificación no existe.");

        _contexto.Notificaciones.Remove(existente);
        _contexto.SaveChanges();
    }

    public List<Notificacion> Buscar(Func<Notificacion, bool> predicado)
    {
        return _contexto.Notificaciones.AsNoTracking().Where(predicado).ToList();
    }

    public List<Notificacion> ObtenerPorUsuario(int usuarioId)
    {
        return _contexto.Notificaciones.AsNoTracking()
            .Where(n => n.UsuarioDestinatarioId == usuarioId)
            .ToList();
    }

    private void DesvincularInstanciaLocal(int id, Notificacion entidad)
    {
        var local = _contexto.Notificaciones.Local.FirstOrDefault(n => n.Id == id);
        if (local != null && !ReferenceEquals(local, entidad))
            _contexto.Entry(local).State = EntityState.Detached;
    }
}