using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.DominioTests;

[TestClass]
public class ActualizacionUsuarioTests
{
    [TestMethod]
    public void ActualizacionUsuario_Propiedades_SeEstablecenCorrectamente()
    {
        // Arrange & Act
        var actualizacion = new ActualizacionUsuario
        {
            Nombre = "Juan",
            Apellido = "García",
            Email = "juan@example.com",
            FechaNacimiento = "01-15-1990",
            EjecutadoPor = "admin@example.com"
        };

        // Assert
        Assert.AreEqual("Juan", actualizacion.Nombre);
        Assert.AreEqual("García", actualizacion.Apellido);
        Assert.AreEqual("juan@example.com", actualizacion.Email);
        Assert.AreEqual("01-15-1990", actualizacion.FechaNacimiento);
        Assert.AreEqual("admin@example.com", actualizacion.EjecutadoPor);
    }

    [TestMethod]
    public void ActualizacionUsuario_PropiedadesNulas_PorDefectoSonNull()
    {
        // Arrange & Act
        var actualizacion = new ActualizacionUsuario();

        // Assert
        Assert.IsNull(actualizacion.Nombre);
        Assert.IsNull(actualizacion.Apellido);
        Assert.IsNull(actualizacion.Email);
        Assert.IsNull(actualizacion.FechaNacimiento);
        Assert.IsNull(actualizacion.EjecutadoPor);
    }

    [TestMethod]
    public void ActualizacionUsuario_SoloParcial_OtrasPropiedadesSigueNulas()
    {
        // Arrange & Act
        var actualizacion = new ActualizacionUsuario
        {
            Nombre = "NuevoNombre"
        };

        // Assert
        Assert.AreEqual("NuevoNombre", actualizacion.Nombre);
        Assert.IsNull(actualizacion.Apellido);
        Assert.IsNull(actualizacion.Email);
    }
}
