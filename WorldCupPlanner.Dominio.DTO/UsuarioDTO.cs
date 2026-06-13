using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.Dominio.DTO;

public class UsuarioDTO
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FechaNacimiento { get; set; } = string.Empty;
    public bool DebeCambiarContrasenia { get; set; }
    public List<RolUsuario> Roles { get; set; } = [];

    public static UsuarioDTO FromEntity(Usuario usuario)
    {
        ArgumentNullException.ThrowIfNull(usuario);

        return new UsuarioDTO
        {
            Id = usuario.Id,
            Nombre = usuario.Nombre,
            Apellido = usuario.Apellido,
            Email = usuario.Email,
            FechaNacimiento = usuario.FechaNacimiento.ToString("MM-dd-yyyy"),
            DebeCambiarContrasenia = usuario.DebeCambiarContrasenia,
            Roles = usuario.ObtenerRoles().ToList()
        };
    }
}
