using WorldCupPlanner.Dominio.DTO;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.ServicioTests;

[TestClass]
public class UsuarioDTOTests
{
    [TestMethod]
    public void FromEntity_NoExponeContraseniaNiHash()
    {
        var usuario = CrearUsuario("usuario@mail.com");
        usuario.EstablecerContrasenia("Passw0rd!");

        var dto = UsuarioDTO.FromEntity(usuario);
        var propiedades = typeof(UsuarioDTO).GetProperties().Select(p => p.Name).ToList();

        Assert.IsFalse(propiedades.Contains("Contrasenia"));
        Assert.IsFalse(propiedades.Contains("ContraseniaCifrada"));
        Assert.IsFalse(propiedades.Contains("Password"));
        Assert.IsFalse(propiedades.Contains("Hash"));
        Assert.IsNull(typeof(UsuarioDTO).GetMethod("ToEntity"));
        Assert.AreNotEqual(usuario.ContraseniaCifrada, dto.Email);
    }

    [TestMethod]
    public void FromEntity_CopiaDatosPublicos()
    {
        var usuario = CrearUsuario("usuario@mail.com");
        usuario.Id = 10;
        usuario.AgregarRol(RolUsuario.Periodista);
        usuario.MarcarParaCambioForzado();

        var dto = UsuarioDTO.FromEntity(usuario);

        Assert.AreEqual(10, dto.Id);
        Assert.AreEqual(usuario.Nombre, dto.Nombre);
        Assert.AreEqual(usuario.Apellido, dto.Apellido);
        Assert.AreEqual(usuario.Email, dto.Email);
        Assert.AreEqual("01-01-1990", dto.FechaNacimiento);
        Assert.IsTrue(dto.DebeCambiarContrasenia);
        CollectionAssert.Contains(dto.Roles, RolUsuario.Periodista);
    }

    private static Usuario CrearUsuario(string email) => new(new DatosUsuario
    {
        Nombre = "Nombre",
        Apellido = "Apellido",
        Email = email,
        FechaNacimiento = "01-01-1990"
    });
}
