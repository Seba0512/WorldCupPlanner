using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Dominio.Excepciones;

namespace WorldCupPlanner.Servicios;

public class ServicioNotificacion
{
    private readonly IRepositorioNotificacion _repositorio;

    public ServicioNotificacion(IRepositorioNotificacion repositorio)
    {
        _repositorio = repositorio;
    }

    public void MarcarComoLeida(int notificacionId, int usuarioId)
    {
        var notificacion = _repositorio.Buscar(n => n.Id == notificacionId).FirstOrDefault()
            ?? throw new EntidadInexistenteExcepcion("La notificación no existe.");

        if (notificacion.UsuarioDestinatarioId != usuarioId)
            throw new AccesoDenegadoExcepcion("La notificación no pertenece al usuario.");

        notificacion.EsLeida = true;
        _repositorio.Actualizar(notificacion);
    }

    public List<Notificacion> ObtenerNoLeidas(int usuarioId)
        => _repositorio.Buscar(n => n.UsuarioDestinatarioId == usuarioId && !n.EsLeida);

    public void GenerarNotificacionesResultado(Partido partido, IEnumerable<Usuario> periodistas)
    {
        foreach (var periodista in periodistas.Where(u => u.TieneRol(RolUsuario.Periodista)))
        {
            _repositorio.Agregar(new Notificacion
            {
                Mensaje = $"Resultado cargado para el partido {partido.Id}.",
                Timestamp = DateTime.UtcNow,
                UsuarioDestinatarioId = periodista.Id
            });
        }
    }
}