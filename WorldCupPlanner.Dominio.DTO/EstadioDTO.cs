using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.Dominio.DTO;

public class EstadioDTO
{
    public string Nombre { get; set; } = string.Empty;
    public string Ciudad { get; set; } = string.Empty;
    public string? Direccion { get; set; }
    public int Capacidad { get; set; }
    public string? Descripcion { get; set; }

    public Estadio ToEntity()
    {
        return new Estadio
        {
            Nombre = Nombre,
            Ciudad = Ciudad,
            Direccion = Direccion,
            Capacidad = Capacidad,
            Descripcion = Descripcion
        };
    }

    public static EstadioDTO FromEntity(Estadio estadio)
    {
        ArgumentNullException.ThrowIfNull(estadio);

        return new EstadioDTO
        {
            Nombre = estadio.Nombre,
            Ciudad = estadio.Ciudad,
            Direccion = estadio.Direccion,
            Capacidad = estadio.Capacidad,
            Descripcion = estadio.Descripcion
        };
    }
}
