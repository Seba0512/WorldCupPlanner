using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.Dominio.DTO;

public class AuditoriaDTO
{
    public int Id { get; set; }
    public string Timestamp { get; set; } = string.Empty;
    public string Usuario { get; set; } = string.Empty;
    public string Accion { get; set; } = string.Empty;
    public string Detalle { get; set; } = string.Empty;

    public static AuditoriaDTO FromEntity(Auditoria auditoria)
    {
        ArgumentNullException.ThrowIfNull(auditoria);

        return new AuditoriaDTO
        {
            Id = auditoria.Id,
            Timestamp = auditoria.Timestamp,
            Usuario = auditoria.Usuario,
            Accion = auditoria.Accion,
            Detalle = auditoria.Detalle
        };
    }
}
