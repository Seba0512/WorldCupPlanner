using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.Dominio.DTO;

public class EstadioDTO
{
    public string Nombre { get; set; } = string.Empty;
    public string Ciudad { get; set; } = string.Empty;
    public int Capacidad { get; set; }
    public string? Descripcion { get; set; }

    public Estadio ToEntity()
    {
        return new Estadio
        {
            Nombre = Nombre,
            Ciudad = Ciudad,
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
            Capacidad = estadio.Capacidad,
            Descripcion = estadio.Descripcion
        };
    }
}
