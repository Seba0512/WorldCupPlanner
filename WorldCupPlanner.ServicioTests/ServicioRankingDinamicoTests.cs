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
}