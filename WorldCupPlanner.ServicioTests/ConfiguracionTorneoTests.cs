using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.ServicioTests;

[TestClass]
public class ConfiguracionTorneoTests
{
    [TestMethod]
    public void ConfiguracionTorneo_DebeTenerValoresPorDefecto()
    {
        // Arrange & Act
        var configuracionTorneo = new ConfiguracionTorneo();

        // Assert
        Assert.AreEqual(new DateTime(2026, 6, 1), configuracionTorneo.FechaInicioTorneo);
        Assert.AreEqual(3, configuracionTorneo.MaxPartidosPorDia);
        Assert.AreEqual(3, configuracionTorneo.SeparacionEntreFechas);
        Assert.AreEqual(new TimeSpan(14, 0, 0), configuracionTorneo.HoraInicioPartidos);
        Assert.AreEqual(4, configuracionTorneo.SeparacionHorasEntrePartidos);
    }
}