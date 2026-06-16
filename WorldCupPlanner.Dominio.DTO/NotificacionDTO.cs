using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.Dominio.DTO;

public class NotificacionDTO
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public string Mensaje { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public bool Leida { get; set; }

    public static NotificacionDTO FromEntity(Notificacion notificacion)
    {
        ArgumentNullException.ThrowIfNull(notificacion);

        return new NotificacionDTO
        {
            Id = notificacion.Id,
            UsuarioId = notificacion.UsuarioDestinatarioId,
            Mensaje = notificacion.Mensaje,
            Fecha = notificacion.Timestamp,
            Leida = notificacion.EsLeida
        };
    }
}
