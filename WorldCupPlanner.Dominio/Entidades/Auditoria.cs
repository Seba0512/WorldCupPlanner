namespace WorldCupPlanner.Dominio.Entidades;

public class Auditoria
{
    public int Id { get; private set; }
    public string Timestamp { get; private set; }
    public string Usuario { get; private set; }
    public string Accion { get; private set; }
    public string Detalle { get; private set; }

    public Auditoria(int id, string usuario, string accion, string detalle)
        : this(id, DateTime.UtcNow.ToString("o"), usuario, accion, detalle)
    {
    }

    public Auditoria(int id, string timestamp, string usuario, string accion, string detalle)
    {
        Id = id;
        Timestamp = timestamp;
        Usuario = usuario;
        Accion = accion;
        Detalle = detalle;
    }
}
