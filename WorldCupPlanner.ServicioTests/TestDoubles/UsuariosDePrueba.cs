using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.ServicioTests;

internal static class UsuariosDePrueba
{
    public static Usuario ConRol(RolUsuario rol)
    {
        var usuario = SinRoles();
        usuario.AgregarRol(rol);
        return usuario;
    }

    public static Usuario SinRoles() => new(new DatosUsuario
    {
        Nombre = "Test",
        Apellido = "User",
        Email = "test@example.com",
        FechaNacimiento = "01-01-1990"
    });
}
