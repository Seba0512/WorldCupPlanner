using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorldCupPlanner.Domain;

namespace WorldCupPlanner.Tests;

[TestClass]
public class EquipoTests
{
    [TestMethod]
    public void Equipo_ConNombreVacio_NoEsValido()
    {
        // Arrange
        var equipo = new Equipo();
        equipo.Nombre = "";

        // Act
        var resultado = equipo.EsValido();

        // Assert
        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void Equipo_ConNombreMas60Caracteres_Valido()
    {
        //Arrange
        var equipo = new Equipo();
        equipo.Nombre = new string('A', 61);
        
        //Act
        var resultado = equipo.EsValido();
        
        //Assert
        Assert.IsFalse(resultado);
    }
}