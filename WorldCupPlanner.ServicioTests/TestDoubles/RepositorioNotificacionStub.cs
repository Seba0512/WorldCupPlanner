using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Excepciones;

namespace WorldCupPlanner.ServicioTests;

internal sealed class RepositorioNotificacionStub : IRepositorioNotificacion
{
    private readonly List<Notificacion> _notificaciones = new();
    private int _siguienteId = 1;

    public void Agregar(Notificacion notificacion)
    {
        notificacion.Id = _siguienteId++;
        _notificaciones.Add(notificacion);
    }

    public void Actualizar(Notificacion notificacion)
    {
        var indice = _notificaciones.FindIndex(n => n.Id == notificacion.Id);
        if (indice == -1)
            throw new EntidadInexistenteExcepcion("La notificación no existe.");
        _notificaciones[indice] = notificacion;
    }

    public void Eliminar(Notificacion notificacion) => _notificaciones.Remove(notificacion);

    public List<Notificacion> Buscar(Func<Notificacion, bool> predicado)
        => _notificaciones.Where(predicado).ToList();

    public List<Notificacion> ObtenerPorUsuario(int usuarioId)
        => _notificaciones.Where(n => n.UsuarioDestinatarioId == usuarioId).ToList();
}
