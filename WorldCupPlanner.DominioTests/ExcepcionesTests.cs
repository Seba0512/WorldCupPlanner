using WorldCupPlanner.Dominio.Excepciones;

namespace WorldCupPlanner.DominioTests;

[TestClass]
public class ExcepcionesTests
{
    [TestMethod]
    public void EntidadInvalidaExcepcion_DebeConservarMensaje()
    {
        // Arrange
        var mensaje = "La entidad es inválida";

        // Act
        var excepcion = new EntidadInvalidaExcepcion(mensaje);

        // Assert
        Assert.AreEqual(mensaje, excepcion.Message);
    }

    [TestMethod]
    public void EntidadInvalidaExcepcion_EsException()
    {
        // Act
        var excepcion = new EntidadInvalidaExcepcion("mensaje");

        // Assert
        Assert.IsInstanceOfType(excepcion, typeof(Exception));
    }

    [TestMethod]
    public void EntidadInexistenteExcepcion_DebeConservarMensaje()
    {
        // Arrange
        var mensaje = "La entidad no existe";

        // Act
        var excepcion = new EntidadInexistenteExcepcion(mensaje);

        // Assert
        Assert.AreEqual(mensaje, excepcion.Message);
    }

    [TestMethod]
    public void EntidadInexistenteExcepcion_EsException()
    {
        // Act
        var excepcion = new EntidadInexistenteExcepcion("mensaje");

        // Assert
        Assert.IsInstanceOfType(excepcion, typeof(Exception));
    }

    [TestMethod]
    public void EntidadYaExistenteExcepcion_DebeConservarMensaje()
    {
        // Arrange
        var mensaje = "La entidad ya existe";

        // Act
        var excepcion = new EntidadYaExistenteExcepcion(mensaje);

        // Assert
        Assert.AreEqual(mensaje, excepcion.Message);
    }

    [TestMethod]
    public void EntidadYaExistenteExcepcion_EsException()
    {
        // Act
        var excepcion = new EntidadYaExistenteExcepcion("mensaje");

        // Assert
        Assert.IsInstanceOfType(excepcion, typeof(Exception));
    }
}
