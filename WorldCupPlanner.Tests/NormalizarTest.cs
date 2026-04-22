using WorldCupPlanner.Domain;

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
}