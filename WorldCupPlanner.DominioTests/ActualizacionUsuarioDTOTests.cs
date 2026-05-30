using WorldCupPlanner.Dominio.DTO;
using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.DominioTests;

[TestClass]
public class ActualizacionUsuarioDTOTests
{
    [TestMethod]
    public void ToEntity_CopiaTodasLasPropiedades()
    {
        var dto = new ActualizacionUsuarioDTO
        {
            Nombre = "Juan",
            Apellido = "Garcia",
            Email = "juan@example.com",
            FechaNacimiento = "01-15-1990",
            EjecutadoPor = "admin@example.com"
        };

        var entidad = dto.ToEntity();

        Assert.AreEqual(dto.Nombre, entidad.Nombre);
        Assert.AreEqual(dto.Apellido, entidad.Apellido);
        Assert.AreEqual(dto.Email, entidad.Email);
        Assert.AreEqual(dto.FechaNacimiento, entidad.FechaNacimiento);
        Assert.AreEqual(dto.EjecutadoPor, entidad.EjecutadoPor);
    }

    [TestMethod]
    public void ToEntity_PreservaPropiedadesNulas()
    {
        var dto = new ActualizacionUsuarioDTO { Nombre = "Nuevo nombre" };

        var entidad = dto.ToEntity();

        Assert.AreEqual("Nuevo nombre", entidad.Nombre);
        Assert.IsNull(entidad.Apellido);
        Assert.IsNull(entidad.Email);
        Assert.IsNull(entidad.FechaNacimiento);
        Assert.IsNull(entidad.EjecutadoPor);
    }

    [TestMethod]
    public void FromEntity_CopiaTodasLasPropiedades()
    {
        var entidad = new ActualizacionUsuario
        {
            Nombre = "Ana",
            Apellido = "Lopez",
            Email = "ana@example.com",
            FechaNacimiento = "02-20-1992",
            EjecutadoPor = "admin@example.com"
        };

        var dto = ActualizacionUsuarioDTO.FromEntity(entidad);

        Assert.AreEqual(entidad.Nombre, dto.Nombre);
        Assert.AreEqual(entidad.Apellido, dto.Apellido);
        Assert.AreEqual(entidad.Email, dto.Email);
        Assert.AreEqual(entidad.FechaNacimiento, dto.FechaNacimiento);
        Assert.AreEqual(entidad.EjecutadoPor, dto.EjecutadoPor);
    }

    [TestMethod]
    public void FromEntity_ConEntidadNula_LanzaExcepcion()
    {
        Assert.ThrowsException<ArgumentNullException>(() =>
            ActualizacionUsuarioDTO.FromEntity(null!));
    }
}
