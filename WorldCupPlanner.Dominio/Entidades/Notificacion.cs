namespace WorldCupPlanner.Dominio.Entidades;

public class Notificacion : IEntidadConId
{
    public int Id { get; set; }
    public string Mensaje { get; set; } = "";
    public DateTime Timestamp { get; set; }
    public int UsuarioDestinatarioId { get; set; }
    public bool EsLeida { get; set; }
}