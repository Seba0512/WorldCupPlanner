namespace WorldCupPlanner.Domain
{
public class Estadio
{
    public string Nombre { get; set; } = "";
    public string Ciudad { get; set; } = "";    
    public string? Direccion { get; set; } = "";
    public int Capacidad { get; set; }
    
    public bool EsValido()
    {        
        return !string.IsNullOrWhiteSpace(Nombre);
    }
}
}