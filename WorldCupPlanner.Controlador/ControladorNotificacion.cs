using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.Controladores;

public class ControladorNotificacion
{
    private readonly ServicioNotificacion _servicioNotificacion;

    public ControladorNotificacion(ServicioNotificacion servicioNotificacion)
    {
        _servicioNotificacion = servicioNotificacion;
    }

    public List<Notificacion> ObtenerNoLeidas(Usuario usuario) =>
        _servicioNotificacion.ObtenerNoLeidas(usuario.Id);

    public List<Notificacion> ObtenerNoLeidas(int usuarioId) =>
        _servicioNotificacion.ObtenerNoLeidas(usuarioId);

    public void MarcarComoLeida(int notificacionId, Usuario usuario) =>
        _servicioNotificacion.MarcarComoLeida(notificacionId, usuario);

    public void MarcarTodasComoLeidas(Usuario usuario)
    {
        foreach (var notificacion in _servicioNotificacion.ObtenerNoLeidas(usuario.Id))
            _servicioNotificacion.MarcarComoLeida(notificacion.Id, usuario);
    }
}
