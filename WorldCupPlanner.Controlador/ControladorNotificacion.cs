using WorldCupPlanner.Dominio.DTO;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Excepciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.Controladores;

public class ControladorNotificacion
{
    private readonly ServicioNotificacion _servicioNotificacion;
    private readonly ServicioUsuario? _servicioUsuario;

    public ControladorNotificacion(ServicioNotificacion servicioNotificacion, ServicioUsuario? servicioUsuario = null)
    {
        _servicioNotificacion = servicioNotificacion;
        _servicioUsuario = servicioUsuario;
    }

    public List<Notificacion> ObtenerNoLeidas(Usuario usuario) =>
        _servicioNotificacion.ObtenerNoLeidas(usuario.Id);

    public List<Notificacion> ObtenerNoLeidas(int usuarioId) =>
        _servicioNotificacion.ObtenerNoLeidas(usuarioId);

    public List<NotificacionDTO> ObtenerNoLeidasDTO(UsuarioDTO usuario) =>
        ObtenerNoLeidasDTO(usuario.Id);

    public List<NotificacionDTO> ObtenerNoLeidasDTO(int usuarioId) =>
        _servicioNotificacion.ObtenerNoLeidas(usuarioId)
            .Select(NotificacionDTO.FromEntity)
            .ToList();

    public void MarcarComoLeida(int notificacionId, Usuario usuario) =>
        _servicioNotificacion.MarcarComoLeida(notificacionId, usuario);

    public void MarcarComoLeida(int notificacionId, UsuarioDTO usuario) =>
        _servicioNotificacion.MarcarComoLeida(notificacionId, _servicioUsuario.ResolverUsuario(usuario)!);

    public void MarcarTodasComoLeidas(Usuario usuario)
    {
        foreach (var notificacion in _servicioNotificacion.ObtenerNoLeidas(usuario.Id))
            _servicioNotificacion.MarcarComoLeida(notificacion.Id, usuario);
    }

    public void MarcarTodasComoLeidas(UsuarioDTO usuario)
    {
        var entidad = _servicioUsuario.ResolverUsuario(usuario)!;
        foreach (var notificacion in _servicioNotificacion.ObtenerNoLeidas(entidad.Id))
            _servicioNotificacion.MarcarComoLeida(notificacion.Id, entidad);
    }
}
