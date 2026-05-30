using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.Dominio.DTO;

public class DatosUsuarioDTO
{
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FechaNacimiento { get; set; } = string.Empty;

    public DatosUsuario ToEntity()
    {
        return new DatosUsuario
        {
            Nombre = Nombre,
            Apellido = Apellido,
            Email = Email,
            FechaNacimiento = FechaNacimiento
        };
    }

    public static DatosUsuarioDTO FromEntity(DatosUsuario datos)
    {
        ArgumentNullException.ThrowIfNull(datos);

        return new DatosUsuarioDTO
        {
            Nombre = datos.Nombre,
            Apellido = datos.Apellido,
            Email = datos.Email,
            FechaNacimiento = datos.FechaNacimiento
        };
    }
}
