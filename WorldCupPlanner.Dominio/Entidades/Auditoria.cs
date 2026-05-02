namespace WorldCupPlanner.Dominio.Entidades;

public class Auditoria
{
    public int Id { get; private set; }
    public string Timestamp { get; private set; }
    public string Usuario { get; private set; }
    public string Accion { get; private set; }
    public string Detalle { get; private set; }

    public Auditoria(int id, string usuario, string accion, string detalle)
    {
        Id = id;
        Timestamp = DateTime.UtcNow.ToString("o"); // ISO-8601
        Usuario = usuario;
        Accion = accion;
        Detalle = detalle;
    }
}
