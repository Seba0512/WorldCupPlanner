using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.ServicioTests;

[TestClass]
public class NormalizarTests
{
    [TestMethod]
    public void Normalizar_TextoConMayusculas_RetornaMinusculas()
    {
        // Arrange
        var texto = "URUGUAY";

        // Act
        var resultado = Normalizar.NormalizarTexto(texto);

        // Assert
        Assert.AreEqual("uruguay", resultado);
    }

    [TestMethod]
    public void Normalizar_TextoConTildes_EliminaTildes()
    {
        // Arrange
        var texto = "París";

        // Act
        var resultado = Normalizar.NormalizarTexto(texto);

        // Assert
        Assert.AreEqual("paris", resultado);
    }

    [TestMethod]
    public void Normalizar_TextoConCaracteresEspeciales_ReemplazaPorEspacio()
    {
        // Arrange
        var texto = "estadio#central";

        // Act
        var resultado = Normalizar.NormalizarTexto(texto);

        // Assert
        Assert.AreEqual("estadio central", resultado);
    }

    [TestMethod]
    public void Normalizar_TextoConEspaciosMultiples_ColapsaEspacios()
    {
        // Arrange
        var texto = "estadio   central";

        // Act
        var resultado = Normalizar.NormalizarTexto(texto);

        // Assert
        Assert.AreEqual("estadio central", resultado);
    }

    [TestMethod]
    public void Normalizar_TextoConEspaciosAlRededor_EliminaEspacios()
    {
        // Arrange
        var texto = "  uruguay  ";

        // Act
        var resultado = Normalizar.NormalizarTexto(texto);

        // Assert
        Assert.AreEqual("uruguay", resultado);
    }
}
