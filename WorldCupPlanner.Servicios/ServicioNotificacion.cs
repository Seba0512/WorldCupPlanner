using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Dominio.Excepciones;

namespace WorldCupPlanner.Servicios;

public class ServicioNotificacion
{
    private readonly IRepositorioNotificacion _repositorio;
    private readonly ServicioAuditoria? _auditoria;

    public ServicioNotificacion(IRepositorioNotificacion repositorio, ServicioAuditoria? auditoria = null)
    {
        _repositorio = repositorio;
        _auditoria = auditoria;
    }

    public void MarcarComoLeida(int notificacionId, int usuarioId)
    {
        MarcarComoLeida(notificacionId, usuarioId, $"UsuarioId {usuarioId}");
    }

    public void MarcarComoLeida(int notificacionId, Usuario usuario)
    {
        var usuarioAuditoria = $"{usuario.Nombre} {usuario.Apellido} ({usuario.Email})";
        MarcarComoLeida(notificacionId, usuario.Id, usuarioAuditoria);
    }

    private void MarcarComoLeida(int notificacionId, int usuarioId, string usuarioAuditoria)
    {
        var notificacion = ObtenerNotificacionPorId(notificacionId);

        if (notificacion.UsuarioDestinatarioId != usuarioId)
            throw new AccesoDenegadoExcepcion("La notificación no pertenece al usuario.");

        notificacion.EsLeida = true;
        _repositorio.Actualizar(notificacion);
        _auditoria?.Registrar(
            usuarioAuditoria,
            "LecturaNotificacion",
            $"Notificacion {notificacion.Id} marcada como leida por usuario.");
    }

    private Notificacion ObtenerNotificacionPorId(int notificacionId)
        => _repositorio.Buscar(n => n.Id == notificacionId).FirstOrDefault()
            ?? throw new EntidadInexistenteExcepcion("La notificación no existe.");

    public List<Notificacion> ObtenerNoLeidas(int usuarioId)
        => _repositorio.Buscar(n => n.UsuarioDestinatarioId == usuarioId && !n.EsLeida);

    public void GenerarNotificacionesPorResultados(IEnumerable<Partido> partidos, IEnumerable<Usuario> usuariosNotificables)
    {
        var usuariosNotificablesList = usuariosNotificables.ToList();
        foreach (var partido in partidos.Where(p => p.EstaJugado()))
            GenerarNotificacionesResultado(partido, usuariosNotificablesList);
    }

    public void GenerarNotificacionesResultado(Partido partido, IEnumerable<Usuario> usuariosNotificables)
    {
        foreach (var usuario in usuariosNotificables.Where(EsUsuarioNotificable))
        {
            _repositorio.Agregar(new Notificacion
            {
                Mensaje = $"Resultado cargado para el partido {partido.Id}.",
                Timestamp = DateTime.UtcNow,
                UsuarioDestinatarioId = usuario.Id
            });
        }
    }

    private static bool EsUsuarioNotificable(Usuario usuario)
        => usuario.TieneRol(RolUsuario.Periodista) ||
           usuario.TieneRol(RolUsuario.AdministradorDelSistema);
}
