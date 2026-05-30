namespace WorldCupPlanner.Dominio.Entidades;

public class Estadio
{
    public string Nombre { get; set; } = string.Empty;
    public string Ciudad { get; set; } = string.Empty;
    public string? Direccion { get; set; }
    public int Capacidad { get; set; }
    public string? Descripcion { get; set; }

    public bool EsValido()
    {
        return !string.IsNullOrWhiteSpace(Nombre)
            && Nombre.Length <= 80
            && !string.IsNullOrWhiteSpace(Ciudad)
            && Ciudad.Length <= 60
            && Capacidad >= 20000
            && (Descripcion == null || Descripcion.Length <= 400);
    }
}