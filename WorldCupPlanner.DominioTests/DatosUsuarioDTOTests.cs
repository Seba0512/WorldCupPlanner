using WorldCupPlanner.Dominio.DTO;
using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.DominioTests;

[TestClass]
public class DatosUsuarioDTOTests
{
    [TestMethod]
    public void ToEntity_CopiaTodasLasPropiedades()
    {
        var dto = new DatosUsuarioDTO
        {
            Nombre = "Juan",
            Apellido = "Garcia",
            Email = "juan@example.com",
            FechaNacimiento = "01-15-1990"
        };

        var entidad = dto.ToEntity();

        Assert.AreEqual(dto.Nombre, entidad.Nombre);
        Assert.AreEqual(dto.Apellido, entidad.Apellido);
        Assert.AreEqual(dto.Email, entidad.Email);
        Assert.AreEqual(dto.FechaNacimiento, entidad.FechaNacimiento);
    }

    [TestMethod]
    public void FromEntity_CopiaTodasLasPropiedades()
    {
        var entidad = new DatosUsuario
        {
            Nombre = "Ana",
            Apellido = "Lopez",
            Email = "ana@example.com",
            FechaNacimiento = "02-20-1992"
        };

        var dto = DatosUsuarioDTO.FromEntity(entidad);

        Assert.AreEqual(entidad.Nombre, dto.Nombre);
        Assert.AreEqual(entidad.Apellido, dto.Apellido);
        Assert.AreEqual(entidad.Email, dto.Email);
        Assert.AreEqual(entidad.FechaNacimiento, dto.FechaNacimiento);
    }

    [TestMethod]
    public void Constructor_PropiedadesPorDefecto_SonStringsVacios()
    {
        var dto = new DatosUsuarioDTO();

        Assert.AreEqual(string.Empty, dto.Nombre);
        Assert.AreEqual(string.Empty, dto.Apellido);
        Assert.AreEqual(string.Empty, dto.Email);
        Assert.AreEqual(string.Empty, dto.FechaNacimiento);
    }

    [TestMethod]
    public void FromEntity_ConEntidadNula_LanzaExcepcion()
    {
        Assert.ThrowsException<ArgumentNullException>(() =>
            DatosUsuarioDTO.FromEntity(null!));
    }
}
