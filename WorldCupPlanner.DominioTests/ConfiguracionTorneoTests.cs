using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.DominioTests;

[TestClass]
public class ConfiguracionTorneoTests
{
    [TestMethod]
    public void ConfiguracionTorneo_ValoresPorDefecto_SonCorrectos()
    {
        // Act
        var config = new ConfiguracionTorneo();

        // Assert
        Assert.AreEqual(new DateTime(2026, 6, 1), config.FechaInicioTorneo);
        Assert.AreEqual(3, config.MaxPartidosPorDia);
        Assert.AreEqual(3, config.SeparacionEntreFechas);
        Assert.AreEqual(new TimeSpan(14, 0, 0), config.HoraInicioPartidos);
        Assert.AreEqual(4, config.SeparacionHorasEntrePartidos);
    }

    [TestMethod]
    public void ConfiguracionTorneo_Semillas_PorDefectoSonCero()
    {
        // Act
        var config = new ConfiguracionTorneo();

        // Assert
        Assert.AreEqual(0, config.SemillaFixture);
        Assert.AreEqual(0, config.SemillaCompletar);
        Assert.AreEqual(0, config.SemillaCrucesFase);
        Assert.AreEqual(0, config.SemillaSimulation);
    }

    [TestMethod]
    public void ConfiguracionTorneo_PropiedadesModificables_SeActualizanCorrectamente()
    {
        // Arrange
        var config = new ConfiguracionTorneo();

        // Act
        config.SemillaFixture = 42;
        config.SemillaCompletar = 10;
        config.SemillaCrucesFase = 7;
        config.SemillaSimulation = 99;
        config.FechaInicioTorneo = new DateTime(2026, 7, 1);
        config.MaxPartidosPorDia = 4;
        config.SeparacionEntreFechas = 2;
        config.HoraInicioPartidos = new TimeSpan(16, 0, 0);
        config.SeparacionHorasEntrePartidos = 3;

        // Assert
        Assert.AreEqual(42, config.SemillaFixture);
        Assert.AreEqual(10, config.SemillaCompletar);
        Assert.AreEqual(7, config.SemillaCrucesFase);
        Assert.AreEqual(99, config.SemillaSimulation);
        Assert.AreEqual(new DateTime(2026, 7, 1), config.FechaInicioTorneo);
        Assert.AreEqual(4, config.MaxPartidosPorDia);
        Assert.AreEqual(2, config.SeparacionEntreFechas);
        Assert.AreEqual(new TimeSpan(16, 0, 0), config.HoraInicioPartidos);
        Assert.AreEqual(3, config.SeparacionHorasEntrePartidos);
    }
}
