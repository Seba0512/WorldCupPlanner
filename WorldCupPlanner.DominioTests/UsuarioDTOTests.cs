using WorldCupPlanner.Dominio.DTO;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.DominioTests;

[TestClass]
public class UsuarioDTOTests
{
    [TestMethod]
    public void FromEntity_CopiaDatosPublicosRolesYFlagDeCambio()
    {
        var usuario = CrearUsuario();
        usuario.AgregarRol(RolUsuario.AdministradorDelSistema);
        usuario.AgregarRol(RolUsuario.Editor);
        usuario.MarcarParaCambioForzado();

        var dto = UsuarioDTO.FromEntity(usuario);

        Assert.AreEqual(usuario.Nombre, dto.Nombre);
        Assert.AreEqual(usuario.Apellido, dto.Apellido);
        Assert.AreEqual(usuario.Email, dto.Email);
        Assert.AreEqual("05-15-1990", dto.FechaNacimiento);
        Assert.IsTrue(dto.DebeCambiarContrasenia);
        CollectionAssert.AreEquivalent(
            usuario.ObtenerRoles().ToList(),
            dto.Roles);
    }

    [TestMethod]
    public void ToEntity_CopiaDatosPublicosRolesYFlagDeCambio()
    {
        var dto = new UsuarioDTO
        {
            Nombre = "Ana",
            Apellido = "Silva",
            Email = "ana.silva@example.com",
            FechaNacimiento = "07-20-1992",
            DebeCambiarContrasenia = true,
            Roles = [RolUsuario.Editor, RolUsuario.AdministradorDelSistema]
        };

        var usuario = dto.ToEntity();

        Assert.AreEqual(dto.Nombre, usuario.Nombre);
        Assert.AreEqual(dto.Apellido, usuario.Apellido);
        Assert.AreEqual(dto.Email, usuario.Email);
        Assert.AreEqual(new DateTime(1992, 7, 20), usuario.FechaNacimiento);
        Assert.IsTrue(usuario.DebeCambiarContrasenia);
        Assert.IsTrue(usuario.TieneRol(RolUsuario.Editor));
        Assert.IsTrue(usuario.TieneRol(RolUsuario.AdministradorDelSistema));
    }

    [TestMethod]
    public void FromEntity_NoExponeContraseniaCifrada()
    {
        var usuario = CrearUsuario();
        usuario.EstablecerContrasenia("Password123!");

        var dto = UsuarioDTO.FromEntity(usuario);
        var propiedades = dto.GetType().GetProperties().Select(propiedad => propiedad.Name).ToList();

        Assert.IsNotNull(usuario.ContraseniaCifrada);
        CollectionAssert.DoesNotContain(propiedades, nameof(usuario.ContraseniaCifrada));
    }

    [TestMethod]
    public void ToEntity_NoRestauraContraseniaNiHash()
    {
        var dto = new UsuarioDTO
        {
            Nombre = "Ana",
            Apellido = "Silva",
            Email = "ana.silva@example.com",
            FechaNacimiento = "07-20-1992"
        };

        var usuario = dto.ToEntity();

        Assert.IsNull(usuario.ContraseniaCifrada);
        Assert.IsFalse(usuario.VerificarContrasenia("Password123!"));
    }

    [TestMethod]
    public void Constructor_PropiedadesPorDefecto_SonValoresSeguros()
    {
        var dto = new UsuarioDTO();

        Assert.AreEqual(string.Empty, dto.Nombre);
        Assert.AreEqual(string.Empty, dto.Apellido);
        Assert.AreEqual(string.Empty, dto.Email);
        Assert.AreEqual(string.Empty, dto.FechaNacimiento);
        Assert.IsFalse(dto.DebeCambiarContrasenia);
        Assert.IsNotNull(dto.Roles);
        Assert.AreEqual(0, dto.Roles.Count);
    }

    [TestMethod]
    public void ToEntity_ConRolesNulos_NoLanzaYCreaUsuarioSinRoles()
    {
        var dto = new UsuarioDTO
        {
            Nombre = "Ana",
            Apellido = "Silva",
            Email = "ana.silva@example.com",
            FechaNacimiento = "07-20-1992",
            Roles = null!
        };

        var usuario = dto.ToEntity();

        Assert.AreEqual(0, usuario.ObtenerRoles().Count());
    }

    [TestMethod]
    public void FromEntity_ConEntidadNula_LanzaExcepcion()
    {
        Assert.ThrowsException<ArgumentNullException>(() => UsuarioDTO.FromEntity(null!));
    }

    private static Usuario CrearUsuario()
    {
        return new Usuario(new DatosUsuario
        {
            Nombre = "Juan",
            Apellido = "Perez",
            Email = "juan.perez@example.com",
            FechaNacimiento = "05-15-1990"
        });
    }
}
