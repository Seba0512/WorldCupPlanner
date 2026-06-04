using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.Servicios;

public class ServicioNotificacion
{
    private readonly IRepositorioNotificacion _repositorio;

    public ServicioNotificacion(IRepositorioNotificacion repositorio)
    {
        _repositorio = repositorio;
    }

    public void GenerarNotificacionesResultado(Partido partido, IEnumerable<Usuario> periodistas)
    {
        foreach (var periodista in periodistas)
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