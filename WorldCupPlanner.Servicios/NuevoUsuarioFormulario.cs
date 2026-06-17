using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.Servicios;

public class NuevoUsuarioFormulario
{
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime FechaNacimiento { get; set; }
    public string Contrasenia { get; set; } = string.Empty;
    public string ConfirmarContrasenia { get; set; } = string.Empty;
    public IEnumerable<RolUsuario> Roles { get; set; } = Enumerable.Empty<RolUsuario>();
}
