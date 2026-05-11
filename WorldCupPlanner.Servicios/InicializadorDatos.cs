using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.Servicios;

public class InicializadorDatos
{
    public const string EmailAdministrador = "administrador@mail.com";
    public const string ContrasenaAdministrador = "Administrador1*";

    public const string EmailEditor = "editor@mail.com";
    public const string ContrasenaEditor = "Editor1*";

    public const string EmailAdminEditor = "admineditor@mail.com";
    public const string ContrasenaAdminEditor = "AdminEditor1*";

    private readonly ServicioUsuario _servicioUsuario;

    public InicializadorDatos(ServicioUsuario servicioUsuario)
    {
        _servicioUsuario = servicioUsuario;
    }

    public void InicializarUsuariosSemilla()
    {
        CrearUsuarioSiNoExiste(
            "Administrador", "Sistema",
            EmailAdministrador, ContrasenaAdministrador,
            RolUsuario.AdministradorDelSistema);

        CrearUsuarioSiNoExiste(
            "Editor", "Sistema",
            EmailEditor, ContrasenaEditor,
            RolUsuario.Editor);

        CrearUsuarioSiNoExiste(
            "AdminEditor", "Sistema",
            EmailAdminEditor, ContrasenaAdminEditor,
            RolUsuario.AdministradorDelSistema, RolUsuario.Editor);
    }

    private void CrearUsuarioSiNoExiste(string nombre, string apellido, string email, string contrasena, params RolUsuario[] roles)
    {
        if (_servicioUsuario.ObtenerPorEmail(email) != null)
            return;

        var usuario = new Usuario(new DatosUsuario
        {
            Nombre = nombre,
            Apellido = apellido,
            Email = email,
            FechaNacimiento = "01-01-1990"
        });

        usuario.EstablecerContrasenia(contrasena);

        foreach (var rol in roles)
            usuario.AgregarRol(rol);

        _servicioUsuario.AgregarUsuario(usuario);
    }
}
