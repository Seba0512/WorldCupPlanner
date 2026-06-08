using WorldCupPlanner.Dominio.Enumeraciones;
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

    [TestMethod]
    public void ActualizarRanking_CuandoGanaEquipoDebil_AumentaRanking()
    {
        // Arrange
        var servicio = new ServicioRankingDinamico();
        // Act
        var rankingNuevo = servicio.ActualizarRanking(1200, 1500, 1.0, FaseTorneo.FaseDeGrupos);
        // Assert
        Assert.IsTrue(rankingNuevo > 1200);
    }

    [TestMethod]
    public void ActualizarRanking_CuandoPierdeEquipoFuerte_DisminuyeRanking()
    {
        // Arrange
        var servicio = new ServicioRankingDinamico();
        // Act
        var rankingNuevo = servicio.ActualizarRanking(1500, 1200, 0.0, FaseTorneo.FaseDeGrupos);
        // Assert
        Assert.IsTrue(rankingNuevo < 1500);
    }

    [TestMethod]
    public void ActualizarRanking_CuandoEmpate_AjustaSegunProbabilidad()
    {
        // Arrange
        var servicio = new ServicioRankingDinamico();
        // Act
        var rankingNuevo = servicio.ActualizarRanking(1500, 1200, 0.5, FaseTorneo.FaseDeGrupos);
        // Assert — era favorito (prob > 0.5), empate baja levemente
        Assert.IsTrue(rankingNuevo < 1500);
    }
}