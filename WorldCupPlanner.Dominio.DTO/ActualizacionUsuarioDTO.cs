
using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.Dominio.DTO;

public class ActualizacionUsuarioDTO
{
    public string? Nombre { get; set; }
    public string? Apellido { get; set; }
    public string? Email { get; set; }
    public string? FechaNacimiento { get; set; }
    public string? EjecutadoPor { get; set; }

    public static ActualizacionUsuarioDTO FromEntity(ActualizacionUsuario actualizacion)
    {
        ArgumentNullException.ThrowIfNull(actualizacion);

        return new ActualizacionUsuarioDTO
        {
            Nombre = actualizacion.Nombre,
            Apellido = actualizacion.Apellido,
            Email = actualizacion.Email,
            FechaNacimiento = actualizacion.FechaNacimiento,
            EjecutadoPor = actualizacion.EjecutadoPor
        };
    }
}
