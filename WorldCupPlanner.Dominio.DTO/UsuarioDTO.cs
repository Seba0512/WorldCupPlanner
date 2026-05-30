using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.Dominio.DTO;

public class UsuarioDTO
{
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FechaNacimiento { get; set; } = string.Empty;
    public bool DebeCambiarContrasenia { get; set; }
    public List<RolUsuario> Roles { get; set; } = [];

    public Usuario ToEntity()
    {
        var usuario = new Usuario(new DatosUsuario
        {
            Nombre = Nombre,
            Apellido = Apellido,
            Email = Email,
            FechaNacimiento = FechaNacimiento
        });

        foreach (var rol in Roles)
        {
            usuario.AgregarRol(rol);
        }

        if (DebeCambiarContrasenia)
            usuario.MarcarParaCambioForzado();

        return usuario;
    }

    public static UsuarioDTO FromEntity(Usuario usuario)
    {
        ArgumentNullException.ThrowIfNull(usuario);

        return new UsuarioDTO
        {
            Nombre = usuario.Nombre,
            Apellido = usuario.Apellido,
            Email = usuario.Email,
            FechaNacimiento = usuario.FechaNacimiento.ToString("MM-dd-yyyy"),
            DebeCambiarContrasenia = usuario.DebeCambiarContrasenia,
            Roles = usuario.ObtenerRoles().ToList()
        };
    }
}
