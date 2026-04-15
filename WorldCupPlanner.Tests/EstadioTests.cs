using WorldCupPlanner.Domain;

namespace WorldCupPlanner.Tests;

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


    
}