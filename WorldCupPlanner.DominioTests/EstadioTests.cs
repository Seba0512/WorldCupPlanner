using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.DominioTests;

[TestClass]
public class EstadioTests
{
    [TestMethod]
    public void Estadio_ConNombreVacio_NoEsValido()
    {
        // Arrange
        var estadio = new Estadio();
        estadio.Nombre = "";

        // Act
        var resultado = estadio.EsValido();

        // Assert
        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void Estadio_NombreExcede80Caracteres_EsInvalido()
    {
        // Arrange
        var estadio = new Estadio
        {
            Nombre = new string('A', 81),
            Ciudad = "Montevideo",
            Capacidad = 20000
        };

        // Act
        var resultado = estadio.EsValido();

        // Assert
        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void Estadio_CiudadVacia_EsInvalido()
    {
        // Arrange
        var estadio = new Estadio
        {
            Nombre = "Estadio Centenario",
            Ciudad = "",
            Capacidad = 20000
        };

        // Act
        var resultado = estadio.EsValido();

        // Assert
        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void Estadio_CiudadExcede60Caracteres_EsInvalido()
    {
        // Arrange
        var estadio = new Estadio
        {
            Nombre = "Estadio Centenaario",
            Ciudad = new string('A', 61),
            Capacidad = 20000
        };

        // Act
        var resultado = estadio.EsValido();

        // Assert
        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void Estadio_CapacidadMenorA20000_EsInvalido()
    {
        // Arrange
        var estadio = new Estadio
        {
            Nombre = "Estadio Centenario",
            Ciudad = "Montevideo",
            Capacidad = 19999
        };

        // Act
        var resultado = estadio.EsValido();

        // Assert
        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void Estadio_DescripcionExcede400Caracteres_EsInvalido()
    {
        // Arrange
        var estadio = new Estadio
        {
            Nombre = "Estadio Centenario",
            Ciudad = "Montevideo",
            Capacidad = 20000,
            Descripcion = new string('A', 401)
        };

        // Act
        var resultado = estadio.EsValido();

        // Assert
        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void Estadio_ConDatosValidos_SinDescripcion_EsValido()
    {
        // Arrange
        var estadio = new Estadio
        {
            Nombre = "Estadio Centenario",
            Ciudad = "Montevideo",
            Capacidad = 20000,
            Descripcion = null
        };

        // Act
        var resultado = estadio.EsValido();

        // Assert
        Assert.IsTrue(resultado);
    }

    [TestMethod]
    public void Estadio_ConDatosValidos_ConDescripcionValida_EsValido()
    {
        // Arrange
        var estadio = new Estadio
        {
            Nombre = "Estadio Centenario",
            Ciudad = "Montevideo",
            Capacidad = 20000,
            Descripcion = "Estadio histórico de Uruguay"
        };

        // Act
        var resultado = estadio.EsValido();

        // Assert
        Assert.IsTrue(resultado);
    }

    [TestMethod]
    public void Estadio_NombreEsEspacioBlanco_EsInvalido()
    {
        // Arrange
        var estadio = new Estadio
        {
            Nombre = "   ",
            Ciudad = "Montevideo",
            Capacidad = 20000
        };

        // Act
        var resultado = estadio.EsValido();

        // Assert
        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void Estadio_CiudadEsEspacioBlanco_EsInvalido()
    {
        // Arrange
        var estadio = new Estadio
        {
            Nombre = "Estadio Centenario",
            Ciudad = "   ",
            Capacidad = 20000
        };

        // Act
        var resultado = estadio.EsValido();

        // Assert
        Assert.IsFalse(resultado);
    }
}
