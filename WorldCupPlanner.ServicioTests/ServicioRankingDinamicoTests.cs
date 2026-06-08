using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.ServicioTests;

[TestClass]
public class ServicioRankingDinamicoTests
{
    [TestMethod]
    public void CalcularProbabilidad_CuandoRankingsIguales_RetornaCincoPorciento()
    {
        // Arrange
        var servicio = new ServicioRankingDinamico();
        // Act
        var resultado = servicio.CalcularProbabilidad(1000, 1000);
        // Assert
        Assert.AreEqual(0.5, resultado, 0.001);
    }

    [TestMethod]
    public void CalcularProbabilidad_CuandoEquipoTieneMayorRanking_RetornaMayorA0punto5()
    {
        // Arrange
        var servicio = new ServicioRankingDinamico();
        // Act
        var resultado = servicio.CalcularProbabilidad(1500, 1000);
        // Assert
        Assert.IsTrue(resultado > 0.5);
    }

    [TestMethod]
    public void CalcularProbabilidad_CuandoEquipoTieneMenorRanking_RetornaMenorA0punto5()
    {
        // Arrange
        var servicio = new ServicioRankingDinamico();
        // Act
        var resultado = servicio.CalcularProbabilidad(1000, 1500);
        // Assert
        Assert.IsTrue(resultado < 0.5);
    }
}